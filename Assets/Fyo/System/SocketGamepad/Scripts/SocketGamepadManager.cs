using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

/// <summary>
/// Handles management of local and remote SocketGamepad objects
/// Base class for Application functionality
/// 
/// Virtuals:
/// HandleConnectedToSGServer -- Connected to Node.js Server
/// HandleAppHandshake -- Handshake Accepted by Node.js Server
/// HandleGamepadHandshake -- New gamepad from Node.js Server
/// HandleGamepadUpdate -- Frame update for an individual SocketGamepad from Node.js Server
/// HandleGamepadDisconnected -- Gamepad Disconnection notification from Node.js Server
/// </summary>
[RequireComponent(typeof(SocketIOComponent))]
public class SocketGamepadManager : MonoBehaviour {
    protected SocketIOComponent socket;

    public Dictionary<SocketGamepad, GamePlayer> Gamepads = new Dictionary<SocketGamepad, GamePlayer>();
    protected Queue<SGUpdateMsg> InputMessageQueue = new Queue<SGUpdateMsg>();

    protected GamePlayer[] LocalPlayers;

    protected List<float> LatencyRecord = new List<float>();
    protected int LatencyInd = 0;
    
    protected float LatencyRoundTripFromServer;
    protected float AverageRoundTrip;
    protected float Latency = 0;
    protected float PingFromServer = 0;
    protected string PingFromServerStr = "";

    public string AppIdString = "Unknown";

    protected virtual void OnStart() {
        LocalPlayers = new GamePlayer[2];
        LocalPlayers[0] = new GamePlayer();
        LocalPlayers[1] = new GamePlayer();
    }

    public virtual void Start() {
        OnStart();
        socket = GetComponent<SocketIOComponent>();

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

        socket.Connect();
        Debug.Log("Connecting...");
    }

    public void HandleDisconnected(SocketIOEvent e) {
        Debug.Log("Disconnect Received: " + e.data);
    }

    /* public void SendHandshakeTest() {
        SocketGamepadManagerHandshakeMessage msg = new SocketGamepadManagerHandshakeMessage();
        socket.Emit("SocketGamepadManagerHandshakeMessage", msg);
    }*/

    #region Utility
    protected int GetGamepadIdFromEvent(SocketIOEvent e) {
        int r = -1;
        if (e.data != null) {
            if (e.data.HasField("PlayerId")) {
                if (!int.TryParse(e.data.GetField("PlayerId").ToString(), out r)) {
                    Debug.LogError("[SocketGamepadManager] Invalid Id Field: \"" + e.data.GetField("PlayerId").ToString() + "\"");
                }
            } else {
                Debug.LogError("[SocketGamepadManager] Id missing in message");
            }
        } else {
            Debug.LogError("[SocketGamepadManager] Data missing in Message");
        }

        return r;
    }

    protected float[] GetGamepadInputsFromEvent(SocketIOEvent e) {
        //Debug.Log("Getting Inputs from Event");
        float[] r = new float[SocketGamepad.GamepadInputCount];
        
        JSONObject InputFields = e.data.GetField("data");
        int b;
        for (b = 0; b < 20; b++) {
            r[b] = InputFields.list[b].f;
        }

        return r;
    }
        
    public void HandlePing(SocketIOEvent e) {
        //Respond to Ping from Server
        socket.Emit("app-pong", e.data);
    }

    public void HandleLatency(SocketIOEvent e) {
        LatencyRoundTripFromServer = e.data.GetField("average").f;
        //Debug.Log("Ping: " + LatencyRoundTripFromServer);
    }
    #endregion

    public delegate void OnAddGamepadDelegate(int GamepadId);
    public OnAddGamepadDelegate dAddGamepad = null;

    protected int GetNextAvailableLocalPlayerId() {
        for (int p = 0; p < LocalPlayers.Length; p++) {
            if (LocalPlayers[p].Gamepad == null)
                return p;
        }
        Debug.Log("No available Player Slots");
        return -1;
    }

    public SocketGamepad CreateGamepad(GamePlayer player, int PlayerId) {
        SocketGamepad gamepad = gameObject.AddComponent<SocketGamepad>();
        gamepad.PlayerId = PlayerId;
        gamepad.LocalId = GetNextAvailableLocalPlayerId();
        
        Gamepads.Add(gamepad, player);

        if (dAddGamepad != null)
            dAddGamepad(gamepad.PlayerId);
        
        return gamepad;
    }

    public virtual void AddExistingGamepad(SocketGamepad gamepad) {
        Gamepads.Add(gamepad, null);
        if(socket != null)
            socket.Emit("ready", new SGUpdateMsg(gamepad));
    }

    protected void DestroyGamepad(int gid) {
        SocketGamepad gamepad = GetGamepad(gid);
        if (gamepad != null) {
            Gamepads.Remove(gamepad);
            DestroyImmediate(gamepad);
        } else {
            Debug.Log("[SocketGamepadManager] Requested Gamepad [" + gid.ToString() + "] has already been destroyed!");
        }
    }

    public virtual bool HasGamepad(SocketGamepad gamepad) {
        return Gamepads.ContainsKey(gamepad);
    }

    protected void RenumberConnectedGamepads() {
        int id = 0;
        Dictionary<SocketGamepad, GamePlayer>.Enumerator en = Gamepads.GetEnumerator();
        while (en.MoveNext()) {
            en.Current.Key.LocalId = id;
            id++;
        }
    }
    
    public SocketGamepad GetGamepad(int GamepadId) {
        Dictionary<SocketGamepad, GamePlayer>.Enumerator e = Gamepads.GetEnumerator();
        while (e.MoveNext()) {
            if (e.Current.Key.PlayerId == GamepadId)
                return e.Current.Key;
        }

        return null;
    }

    public virtual void HandleConnectedToSGServer(SocketIOEvent e) {
        AppHandshakeMsg GameInfoMsg = new AppHandshakeMsg();
        GameInfoMsg.AppIDString = AppIdString;
        GameInfoMsg.Serialize();

        //Identify App to Node Server
        Debug.Log("Handshake Accepted, sending Game Info\n" + GameInfoMsg.ToString());
        socket.Emit("AppHandshakeMsg", GameInfoMsg);
    }
    
    public virtual void HandleAppHandshake(SocketIOEvent e) {
        //Request Connected Gamepads
        Debug.Log("Application Handshake Accepted");
    }

    //Used for debugging
    public GameObject InputIndicatorPrefab = null;
    public virtual void HandleGamepadHandshake(SocketIOEvent e) {
        //Upon Handshake, create the Gamepad
        SGHandshakeMsg handshakeMsg = new SGHandshakeMsg(e.data);

        SocketGamepad gamepad = CreateGamepad(null, handshakeMsg.PlayerId); //Create unassociated Gamepad
        gamepad.PlayerId = handshakeMsg.PlayerId;

        handshakeMsg.Serialize();

        if (InputIndicatorPrefab != null) {
            SocketGamepadTestIndicator Tester = ((GameObject)Instantiate(InputIndicatorPrefab)).GetComponent<SocketGamepadTestIndicator>();
            Tester.Gamepad = gamepad;
        }

        Debug.Log("Gamepad Handshake: " + gamepad.PlayerId.ToString());
    }

    //Change Input array to Dictionary of named delegates instead of an array of 20 floats
    public virtual void HandleGamepadUpdate(SocketIOEvent e) {
        int gid = -1;
        e.data.GetField(ref gid, "PlayerId");

        if (gid > -1) {
            SocketGamepad gamepad = GetGamepad(gid);
            if (gamepad == null) {
                Debug.Log("Controller " + gid + " sending Updates without handshake!");
            }

            gamepad.inputs = GetGamepadInputsFromEvent(e);

            //NOTE: Send effects and events here
        } else {
            //Hack Attempt?
            Debug.LogError("STOP IT, I'M NOT A DOCTOR!!");
        }
    }

    public virtual void HandleGamepadDisconnected(SocketIOEvent e) {
        int gid = 0;
        e.data.GetField(ref gid, "PlayerId");
        if (gid > -1 && gid < Gamepads.Count) {
            SocketGamepad gamepad = GetGamepad(gid);
            Debug.Log("Removing Gamepad " + gid.ToString());
            Gamepads.Remove(gamepad);
            Destroy(gamepad);
            RenumberConnectedGamepads();
        }
    }
    
    //Local, Manual update for testing
    protected void UpdateInput(SocketIOEvent e) {
        int gid = GetGamepadIdFromEvent(e);
        if (gid < Gamepads.Count) {
            SocketGamepad pad = GetGamepad(gid);
            SGUpdateMsg msg = new SGUpdateMsg(e.data);
            pad.inputs = msg.inputs;
            msg.Serialize();
        } else {
            Debug.LogError("[SocketGamepadManager] Error - Gamepad Index out of range");
        }
    }
    
}
