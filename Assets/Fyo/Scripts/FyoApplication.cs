using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.IO;
using System.Linq;

namespace Fyo {
    /// <summary>
    /// FyoApplication connects to the Node.js SocketGamepadServer
    /// It then handles handshake of the application and gamepads
    /// It is up to the developer to determine actions after handshake and updates occur
    /// through use of FyoApplication.On* methods
    /// 
    /// Order:
    /// Connected
    /// - Server has confirmed connection, this triggers the FyoApplication to identify itself and send controller payloads for use
    /// Handshake Accepted
    /// - Server has accepted identification, processed payloads, and is ready to begin managing SocketGamepads for the application
    /// Gamepad Handshake
    /// - A Gamepad has been connected with the SocketGamepadManager and would like to connect
    /// Gamepad Update
    /// - A Gamepad's normal update, to be handled by the application as a controller
    /// - FyoApplication may send an SGUpdateMsg back to the SocketGamepadManager to trigger effects on the controller
    /// Gamepad Disconnect
    /// - 
    /// 
    /// Virtuals:
    /// HandleConnectedToSGServer -- Connected to Node.js Server
    /// - Fires OnConnected()
    /// HandleAppHandshake -- Handshake Accepted by Node.js Server
    /// - Fires OnAppHandshake()
    /// HandleGamepadHandshake -- New gamepad from Node.js Server
    /// - Fires OnGamepadHandshake(SocketGamepad)
    /// HandleGamepadUpdate -- Frame update for an individual SocketGamepad from Node.js Server
    /// - Fires OnGamepadUpdate(SocketGamepad)
    /// HandleGamepadDisconnected -- Gamepad Disconnection notification from Node.js Server
    /// - Fires OnGamepadDisconnect(SocketGamepad)
    /// 
    /// </summary>
    [RequireComponent(typeof(SocketIOComponent))]
    public abstract class FyoApplication : MonoBehaviour {
        public string AppIdString = "Unknown";
        public int MaxPlayers = 8;
        public string DefaultController = string.Empty;
        public string ControllerPayload = string.Empty;

        public Dictionary<SocketGamepad, FyoPlayer> ActiveGamepads = new Dictionary<SocketGamepad, FyoPlayer>();
        public List<SocketGamepad> Gamepads = new List<SocketGamepad>();

        protected SocketIOComponent socket;
        public List<FyoPlayer> LocalPlayers = new List<FyoPlayer>();
        protected float ServerLatency;

        public bool ConnectLocalGamepads = false;

        /// <summary>
        /// Assign additional socket message handlers before starting the connection
        /// </summary>
        protected abstract void AssignExtraHandlers();

        Dictionary<string, SocketGamepadLocalInputAdapter> LocalGamepads = new Dictionary<string, SocketGamepadLocalInputAdapter>();
        protected virtual void OnStart() {
            if (ConnectLocalGamepads) {
                string[] strJoysticks = Input.GetJoystickNames();
                SocketGamepadLocalInputAdapter Adapter;
                for (int j = 0; j < strJoysticks.Length; j++) {
                    Adapter = gameObject.AddComponent<SocketGamepadLocalInputAdapter>();
                    Adapter.LocalInputIndex = j;
                }
            }
        }

        void CheckLocalGamepads() {

        }

        protected void Start() {
            socket = GetComponent<SocketIOComponent>();
            OnStart();

#if UNITY_EDITOR
            if (socket.IsConnected) {
                //Complain at developer and select offending object
                Debug.LogError("Turn off AutoConnect on SocketIOComponent or it will break a Standalone build");
                UnityEditor.Selection.activeGameObject = socket.gameObject;

                socket.Close();
                socket.autoConnect = false;
            }
#endif

            #region Message Handlers
            //Connection Accepted
            socket.On("connect", HandleConnectedToSGServer);            //Connected, Send App Handshake with App Info
            socket.On("AppEndMsg", HandleDisconnected);                //Connected, Send App Handshake with App Info
                                                                        //Handshake Accepted
            socket.On("AppHandshakeMsg", HandleAppHandshake);           //App Handshake Accepted
                                                                        //Gamepad Connected
            socket.On("SGHandshakeMsg", HandleGamepadHandshake);        //Handshake a new Gamepad
                                                                        //Gamepad Update
            socket.On("SGUpdateMsg", HandleGamepadUpdate);              //Message for a single gamepad
                                                                        //Gamepad Timing Out
            socket.On("SGTimingOutMsg", HandleGamepadTimingOut);        //Gamepad Timeout Timer has begun
                                                                        //Gamepad Reconnect
            socket.On("SGReconnectMsg", HandleGamepadReconnect);        //Gamepad Reconnected
                                                                        //Gamepad Disconnected
            socket.On("SGDisconnectMsg", HandleGamepadDisconnected);    //Gamepad Disconnected
                                                                        //Latency Calculation
            socket.On("app-ping", HandlePing);                          //Ping Request from Server
            socket.On("app-latency", HandleLatency);                    //Ping Calculation from server
            #endregion

            AssignExtraHandlers();

            socket.Connect();
            Debug.Log("Connecting...");
        }

        #region Gamepad Management
        /// <summary>
        /// Creates a virtual object to represent a connected SocketGamepad
        /// </summary>
        /// <param name="SGID">SocketGamepad Identifier as it relates to the SocketGamepadManager</param>
        /// <returns></returns>
        public SocketGamepad CreateOrReconnectGamepad(int SGID) {
            SocketGamepad gamepad = null;
            if (Gamepads.Count > 0) {
                for (int g = 0; g < Gamepads.Count; g++) {
                    //TODO: Handle Same Gamepad different device id?
                    if (Gamepads[g].SGID == SGID) {
                        gamepad = Gamepads[g];
                    }
                }
            }

            if (gamepad == null) {
                gamepad = gameObject.AddComponent<SocketGamepad>();
                gamepad.SGID = SGID;
                gamepad.LocalId = Gamepads.Count;
                
                Gamepads.Add(gamepad);

                if (gamepad.Controller != DefaultController) {
                    JSONObject msg = JSONObject.CreateStringObject(DefaultController);
                    socket.Emit("SGRedirectMsg", msg);
                }

                OnGamepadPluggedIn(gamepad);
            } else {
                OnGamepadReconnect(gamepad);
            }

            return gamepad;
        }

        /// <summary>
        /// Adds a SocketGamepad for local testing (input communication only, through use of SocketGamepadLocalInputAdapter)
        /// </summary>
        /// <param name="gamepad">SocketGamepad virtual object</param>
        public void AddExistingGamepad(SocketGamepad gamepad) {
            if (gamepad != null) {
                if (!Gamepads.Contains(gamepad)) {
                    Gamepads.Add(gamepad);
                    
                    if (gamepad.Controller != DefaultController) {
                        JSONObject msg = JSONObject.CreateStringObject(DefaultController);
                        socket.Emit("SGRedirectMsg", msg);
                    }

                    OnGamepadPluggedIn(gamepad);
                } else {
                    Debug.LogError("Tried to add duplicate SGID:" + gamepad.SGID);
                }
            } else {
                Debug.LogError("Null gamepad passed to AddExistingGamepad()");
            }
        }

        /// <summary>
        /// Removes a SocketGamepad, used as an "unplug"
        /// </summary>
        /// <param name="gamepad"></param>
        public void RemoveGamepad(SocketGamepad gamepad) {
            if (gamepad != null) {
                OnGamepadUnplugged(gamepad);
                if (ActiveGamepads.ContainsKey(gamepad)) {
                    ActiveGamepads.Remove(gamepad);
                }
                Gamepads.Remove(gamepad);
                DestroyImmediate(gamepad);
            } else {
                Debug.LogError("Null gamepad passed to RemoveGamepad()");
            }
        }

        public bool HasGamepad(SocketGamepad gamepad) {
            return Gamepads.Contains(gamepad);
        }

        public SocketGamepad GetGamepad(int SGID) {
            return Gamepads.Find(g => g.SGID == SGID);
        }
        #endregion

        #region Developer Callbacks
        /// <summary>
        /// Called after a connection is established with the Socket Gamepad Manager
        /// before an AppHandshakeMsg is sent back to the Socket Gamepad Manager
        /// </summary>
        protected abstract void OnConnected();

        /// <summary>
        /// Called after the Socket Gamepad Manager accepts Handshake
        /// </summary>
        /// <param name="handshakeMsg">Data sent from server for handshake acknowledgement, handshakeMsg.BinaryData may be developer defined payload(s)</param>
        protected abstract void OnHandshake(AppHandshakeMsg handshakeMsg);

        /// <summary>
        /// Called if the Socket Gamepad Manager sends a 'disconnect' message
        /// </summary>
        protected abstract void OnDisconnected();

        /// <summary>
        /// Called after a gamepad is registered with the system, passed the a virtual representation of a browser based controller functions.
        /// This is when the developer could use the ActiveGamepads Dictionary to associate active player objects with plugged-in gamepads
        /// </summary>
        /// <param name="gamepad">Virtual SocketGamepad that was plugged in</param>
        protected abstract void OnGamepadPluggedIn(SocketGamepad gamepad);

        /// <summary>
        /// Processes a SocketGamepad Update from the Socket Gamepad Manager
        /// </summary>
        protected abstract void OnUpdateGamepad(SocketGamepad gamepad);

        /// <summary>
        /// Triggered when a SocketGamepad begins Timing out on the Fyo server
        /// </summary>
        /// <param name="gamepad"></param>
        protected abstract void OnGamepadTimingOut(SocketGamepad gamepad);

        /// <summary>
        /// Triggered when a SocketGamepad Reconnects
        /// </summary>
        /// <param name="gamepad"></param>
        protected abstract void OnGamepadReconnect(SocketGamepad gamepad);

        /// <summary>
        /// Called after a gamepad is removed from the system
        /// </summary>
        /// <param name="gamepad">Virtual SocketGamepad object which was unplugged</param>
        protected abstract void OnGamepadUnplugged(SocketGamepad gamepad);
        #endregion

        #region Socket Handlers
        /// <summary>
        /// Handler for 'connect' message from Socket Gamepad Manager
        /// Generates an AppHandshakeMsg with Controller Payloads
        /// if any and sends it to the server, as well as the default controller name
        /// </summary>
        /// <param name="e"></param>
        protected void HandleConnectedToSGServer(SocketIOEvent e) {
            string Payload = string.Empty;
            string ControllerPath = Fyo.DefaultPaths.Controllers + ControllerPayload;

            if (File.Exists(ControllerPath)) {
                Debug.LogWarning("Payload File \"" + ControllerPath + "\"");
                byte[] data = File.ReadAllBytes(ControllerPath);
                Payload = Convert.ToBase64String(data);
                Debug.LogWarning("Payload Data\"" + Payload + "\"");
            } else {
                Debug.LogWarning("No Payload, Path: \"" + ControllerPath + "\"");
            }

            AppHandshakeMsg GameInfoMsg = new AppHandshakeMsg(AppIdString, Payload, DefaultController);
            OnConnected();

            //Identify App to Node Server
            Debug.Log("Handshake Accepted, sending Game Info: " + GameInfoMsg.ToString());
            socket.Emit("AppHandshakeMsg", GameInfoMsg);
        }

        /// <summary>
        /// Handles AppHandshakeMsg response from server after AppHandshakeMsg is sent
        /// Acknowledgement of Handshake, 
        /// TODO: otherwise a Disconnect message will be sent from the server?
        /// </summary>
        /// <param name="e"></param>
        protected void HandleAppHandshake(SocketIOEvent e) {
            AppHandshakeMsg handshakeMsg = new AppHandshakeMsg(e.data);
            OnHandshake(handshakeMsg);
            Debug.Log("Application Handshake Accepted");
        }
        /// <summary>
        /// Handles 'disconnect' from Socket Gamepad Manager
        /// </summary>
        /// <param name="e"></param>
        protected void HandleDisconnected(SocketIOEvent e) {
            Debug.Log("Disconnect Received from Server: " + e.data);
            OnDisconnected();
        }

        /// <summary>
        /// Handles a new Gamepad being "plugged in" to the Socket Gamepad Manager
        /// </summary>
        /// <param name="e"></param>
        protected void HandleGamepadHandshake(SocketIOEvent e) {
            //Upon Handshake, create the Gamepad
            SGHandshakeMsg gamepadHandshake = new SGHandshakeMsg(e.data);
            SocketGamepad gamepad = CreateOrReconnectGamepad(gamepadHandshake.SGID);
            gamepad.Controller = gamepadHandshake.Controller;

            if (gamepad != null) {
                Debug.Log("Gamepad Handshake: " + e.data);
            } else {
                Debug.LogError("Gamepad Handshake failed: " + e.data);
            }
        }

        protected void HandleGamepadTimingOut(SocketIOEvent e) {
            SGUpdateMsg gamepadTimingOut = new SGUpdateMsg(e.data);
            Debug.Log("Gamepad " + gamepadTimingOut.SGID + " Timing out");
            SocketGamepad gamepad = GetGamepad(gamepadTimingOut.SGID);
            OnGamepadTimingOut(gamepad);
        }

        protected void HandleGamepadReconnect(SocketIOEvent e) {
            SGHandshakeMsg gamepadHandshake = new SGHandshakeMsg(e.data);
            Debug.Log("Gamepad " + gamepadHandshake.SGID + " Reconnected " + gamepadHandshake.Controller);
            SocketGamepad gamepad = CreateOrReconnectGamepad(gamepadHandshake.SGID);
            gamepad.Controller = gamepadHandshake.Controller;
            //OnGamepadReconnect(gamepad) handled in CreateOrReconnectGamepad
        }

        #region Local Testing
        /// <summary>
        /// Used for local testing, injects a manually created SGHandshakeMsg
        /// into the system as if it were from the Socket Gamepad Manager
        /// </summary>
        /// <param name="handshakeMsg">Manually created Gamepad Handshake Message</param>
        public void InjectGamepadHandshake(SGHandshakeMsg handshakeMsg) {
            handshakeMsg.Serialize();
            HandleGamepadHandshake(new SocketIOEvent("SGHandshakeMsg", handshakeMsg));
        }

        /// <summary>
        /// Used for local testing, injects a manually created SGUpdateMsg
        /// into the system as if it were from the Socket Gamepad Manager
        /// </summary>
        /// <param name="updateMsg"></param>
        public void InjectGamepadUpdate(SGUpdateMsg updateMsg) {
            updateMsg.Serialize();
            HandleGamepadUpdate(new SocketIOEvent("SGUpdateMsg", updateMsg));
        }
        #endregion

        //Change Input array to Dictionary of named delegates instead of an array of 20 floats
        protected void HandleGamepadUpdate(SocketIOEvent e) {
            SGUpdateMsg UpdateMsg = new SGUpdateMsg(e.data);
            //Debug.Log("Gamepad Update Received for " + UpdateMsg.SGID.ToString());
            if (UpdateMsg.SGID > -1) {
                SocketGamepad gamepad = GetGamepad(UpdateMsg.SGID);
                if (gamepad == null) {
                    Debug.Log("Controller " + UpdateMsg.SGID + " sending Updates without handshake!");
                } else {
                    gamepad.InputData = UpdateMsg.Data;
                    OnUpdateGamepad(gamepad);
                }
            } else {
                //Hack Attempt?
                Debug.LogError("STOP IT, I'M NOT A DOCTOR!!");
            }
        }

        protected void HandleGamepadDisconnected(SocketIOEvent e) {
            Debug.Log("Gamepad Disconnect Received: " + e.data + "");
            SGDisconnectMsg DisconnectMsg = new SGDisconnectMsg(e.data);
            Debug.Log("Controller " + DisconnectMsg.SGID + " Disconnected.");

            SocketGamepad gamepad = Gamepads.Find(g => g.SGID == DisconnectMsg.SGID);
            if (gamepad != null) {
                Debug.Log("Removing Gamepad " + DisconnectMsg.SGID.ToString());
                RemoveGamepad(gamepad);
            }
        }

        protected void HandlePing(SocketIOEvent e) {
            //Respond to Ping from Server
            socket.Emit("app-pong", e.data);
        }

        protected void HandleLatency(SocketIOEvent e) {
            ServerLatency = e.data.GetField("average").f;
            //Debug.Log("Ping: " + LatencyRoundTripFromServer);
        }
        #endregion
    }
}
