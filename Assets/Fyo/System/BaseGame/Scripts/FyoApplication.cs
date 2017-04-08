using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.IO;

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

    /// <summary>
    /// Assign additional socket message handlers before starting the connection
    /// </summary>
    protected abstract void AssignExtraHandlers();

    protected virtual void OnStart() {
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
        socket.On("connect", HandleConnectedToSGServer);        //Connected, Send App Handshake with App Info
        socket.On("disconnect", HandleDisconnected);        //Connected, Send App Handshake with App Info
        //Handshake Accepted
        socket.On("AppHandshakeMsg", HandleAppHandshake);       //App Handshake Accepted
        //Gamepad Connected
        socket.On("SGHandshakeMsg", HandleGamepadHandshake);    //Handshake a new Gamepad
        //Gamepad Update
        socket.On("SGUpdateMsg", HandleGamepadUpdate);          //Message for a single gamepad
        //Gamepad Disconnected
        socket.On("SGDisconnectMsg", HandleGamepadDisconnected);//Gamepad Disconnected
        //Latency Calculation
        socket.On("app-ping", HandlePing); //Ping Request from Server
        socket.On("app-latency", HandleLatency); //Ping Calculation from server
        #endregion

        AssignExtraHandlers();

        socket.Connect();
        Debug.Log("Connecting...");
    }

    #region Debug
    //Used for debugging
    public GameObject InputIndicatorPrefab = null;
    protected Dictionary<int, GameObject> Indicators = new Dictionary<int, GameObject>();
    #endregion

    #region Gamepad Management
    /// <summary>
    /// Creates a virtual object to represent a connected SocketGamepad
    /// </summary>
    /// <param name="PlayerId">SocketGamepad Identifier as it relates to the SocketGamepadManager</param>
    /// <returns></returns>
    public SocketGamepad GetOrReconnectGamepad(int PlayerId) {
        SocketGamepad gamepad = null;
        if (Gamepads.Count > 0) {
            for (int g = 0; g < Gamepads.Count; g++) {
                //TODO: Handle Same Gamepad different device id?
                if (Gamepads[g].PlayerId == PlayerId) {
                    gamepad = Gamepads[g];
                }
            }
        }

        if (gamepad == null) {
            gamepad = gameObject.AddComponent<SocketGamepad>();
            gamepad.PlayerId = PlayerId;
            Gamepads.Add(gamepad);
        }

        OnGamepadPluggedIn(gamepad);
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
                OnGamepadPluggedIn(gamepad);
            } else {
                Debug.LogError("Tried to add duplicate gamepad PlayerId:" + gamepad.PlayerId);
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
            if (ActiveGamepads.ContainsKey(gamepad)) {
                Gamepads.Remove(gamepad);
                OnGamepadUnplugged(gamepad);
                DestroyImmediate(gamepad);
            } else {
                Debug.LogError("Tried to remove an unregistered gamepad PlayerId: " + gamepad.PlayerId);
            }
        } else {
            Debug.LogError("Null gamepad passed to RemoveGamepad()");
        }
    }

    public bool HasGamepad(SocketGamepad gamepad) {
        return Gamepads.Contains(gamepad);
    }
    
    public SocketGamepad GetGamepad(int PlayerId) {
        return Gamepads.Find(g => g.PlayerId == PlayerId);
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
    /// </summary>
    /// <param name="gamepad">Virtual SocketGamepad that was plugged in</param>
    protected abstract void OnGamepadPluggedIn(SocketGamepad gamepad);

    /// <summary>
    /// Processes a SocketGamepad Update from the Socket Gamepad Manager
    /// </summary>
    protected abstract void OnUpdateGamepad(SocketGamepad gamepad);

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
    /// for use by the Socket Gamepad Server
    /// </summary>
    /// <param name="e"></param>
    protected void HandleConnectedToSGServer(SocketIOEvent e) {
        string Payload = string.Empty;
        string ControllerPath = Fyo.Paths.Controllers + ControllerPayload;
        if (File.Exists(ControllerPath)) {
            byte[] data = File.ReadAllBytes(ControllerPath);
            Payload = Convert.ToBase64String(data);
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
        Debug.Log("Disconnect Received: " + e.data);
        OnDisconnected();
    }
    
    /// <summary>
    /// Handles a new Gamepad being "plugged in" to the Socket Gamepad Manager
    /// </summary>
    /// <param name="e"></param>
    protected void HandleGamepadHandshake(SocketIOEvent e) {
        //Upon Handshake, create the Gamepad
        SGHandshakeMsg gamepadHandshake = new SGHandshakeMsg(e.data);
        SocketGamepad gamepad = GetOrReconnectGamepad(gamepadHandshake.PlayerId);
        Debug.Log("Gamepad Handshake: " + e.data);

#if UNITY_EDITOR
        if (InputIndicatorPrefab != null) {
            Debug.LogWarning("Showing Indicator Prefab | This will not occur in a standalone build");
            SocketGamepadTestIndicator Tester = Instantiate(InputIndicatorPrefab).GetComponent<SocketGamepadTestIndicator>();
            Tester.Gamepad = gamepad;
            Indicators.Add(gamepad.PlayerId, Tester.gameObject);
        }
#endif
    }

    #region Local Testing
    /// <summary>
    /// Used for local testing, injects a manually created SGHandshakeMsg
    /// into the system as if it were from the Socket Gamepad Manager
    /// </summary>
    /// <param name="handshakeMsg">Manually created Gamepad Handshake Message</param>
    public void InjectGamepadHandshake(SGHandshakeMsg handshakeMsg) {
        HandleGamepadHandshake(new SocketIOEvent("SGHandshakeMsg", handshakeMsg));
    }

    /// <summary>
    /// Used for local testing, injects a manually created SGUpdateMsg
    /// into the system as if it were from the Socket Gamepad Manager
    /// </summary>
    /// <param name="updateMsg"></param>
    public void InjectGamepadUpdate(SGUpdateMsg updateMsg) {
        HandleGamepadUpdate(new SocketIOEvent("SGUpdateMsg",updateMsg));
    }
    #endregion

    //Change Input array to Dictionary of named delegates instead of an array of 20 floats
    protected void HandleGamepadUpdate(SocketIOEvent e) {
        SGUpdateMsg UpdateMsg = new SGUpdateMsg(e.data);

        if (UpdateMsg.PlayerId > -1) {
            SocketGamepad gamepad = GetGamepad(UpdateMsg.PlayerId);
            if (gamepad == null) {
                Debug.Log("Controller " + UpdateMsg.PlayerId + " sending Updates without handshake!");
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
        int PlayerId = 0;
        e.data.GetField(ref PlayerId, "PlayerId");
        if (PlayerId > -1 && PlayerId < ActiveGamepads.Count) {
            if (Indicators.ContainsKey(PlayerId)) {
                GameObject g = Indicators[PlayerId];
                Indicators.Remove(PlayerId);
                Destroy(g);
            }

            SocketGamepad gamepad = Gamepads.Find(g => g.PlayerId == PlayerId);
            if (gamepad != null) {
                Debug.Log("Removing Gamepad " + PlayerId.ToString());
                Gamepads.Remove(gamepad);
                OnGamepadUnplugged(gamepad);
                DestroyImmediate(gamepad);
            }
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
