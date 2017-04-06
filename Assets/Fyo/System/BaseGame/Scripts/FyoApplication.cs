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
public class FyoApplication : MonoBehaviour {
    public string AppIdString = "Unknown";
    public int MaxPlayers = 8;
    public Dictionary<SocketGamepad, FyoPlayer> Gamepads = new Dictionary<SocketGamepad, FyoPlayer>();

    protected SocketIOComponent socket;
    protected List<FyoPlayer> LocalPlayers = new List<FyoPlayer>();
    protected float ServerLatency;
    
    protected virtual void OnStart() {
    }

    protected virtual void Start() {
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

    #region Debug
    //Used for debugging
    public GameObject InputIndicatorPrefab = null;
    protected Dictionary<int, GameObject> Indicators = new Dictionary<int, GameObject>();
    #endregion

    #region Utility
    protected int GetPlayerIdFromEvent(SocketIOEvent e) {
        int r = -1;
        if (e.data != null) {
            if (e.data.HasField("PlayerId")) {
                if (!int.TryParse(e.data.GetField("PlayerId").ToString(), out r)) {
                    Debug.LogError("[SocketGamepadManager] Invalid Id Field: \"" + e.data.GetField("PlayerId").ToString() + "\"");
                }
            } else {
                Debug.LogError("[SocketGamepadManager] PlayerId missing in message");
            }
        } else {
            Debug.LogError("[SocketGamepadManager] Data missing in Message");
        }

        return r;
    }
    
    #endregion

    #region Gamepad Management
    protected virtual int AddPlayerToFreeSlot() {
        if (LocalPlayers.Count < MaxPlayers) {
            GameObject PlayerObj = new GameObject("Player " + LocalPlayers.Count.ToString());

            if (PlayerObj != null) {
                FyoPlayer Player = PlayerObj.AddComponent<FyoPlayer>();
                if (Player != null) {
                    Player.PlayerId = LocalPlayers.Count;
                    LocalPlayers.Add(Player);
                    return Player.PlayerId;
                } else {
                    Debug.LogError("Player Prefab is missing a FyoPlayer derived Component!");
                }
            } else {
                Debug.LogError("Player Prefab is missing!");
            }
        }

        Debug.Log("No available Player Slots");
        return -1;
    }

    protected int GetNextFreePlayerSlotIndex() {
        if (LocalPlayers.Count < MaxPlayers) {
            return LocalPlayers.Count;
        }

        Debug.Log("No available Player Slots");
        return -1;
    }

    protected virtual void OnAddGamepad(int PlayerId) {}

    public SocketGamepad CreateGamepad(FyoPlayer player, int PlayerId) {
        SocketGamepad gamepad = gameObject.AddComponent<SocketGamepad>();
        gamepad.PlayerId = PlayerId;
        gamepad.LocalId = GetNextFreePlayerSlotIndex();
        
        Gamepads.Add(gamepad, player);
        OnAddGamepad(gamepad.PlayerId);
        
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
        Dictionary<SocketGamepad, FyoPlayer>.Enumerator en = Gamepads.GetEnumerator();
        while (en.MoveNext()) {
            en.Current.Key.LocalId = id;
            id++;
        }
    }
    
    public SocketGamepad GetGamepad(int PlayerId) {
        Dictionary<SocketGamepad, FyoPlayer>.Enumerator e = Gamepads.GetEnumerator();
        while (e.MoveNext()) {
            if (e.Current.Key.PlayerId == PlayerId)
                return e.Current.Key;
        }

        return null;
    }
    #endregion

    #region Socket Handlers
    protected virtual void HandleConnectedToSGServer(SocketIOEvent e) {
        AppHandshakeMsg GameInfoMsg = new AppHandshakeMsg();
        GameInfoMsg.AppIDString = AppIdString;
        GameInfoMsg.Serialize();

        //Identify App to Node Server
        Debug.Log("Handshake Accepted, sending Game Info\n" + GameInfoMsg.ToString());
        socket.Emit("AppHandshakeMsg", GameInfoMsg);
        
        socket.Emit("SGPayloadMsg", new SGPayloadMsg("BaseController.zip"));
    }

    protected void HandleDisconnected(SocketIOEvent e) {
        Debug.Log("Disconnect Received: " + e.data);
    }

    protected virtual void HandleAppHandshake(SocketIOEvent e) {
        //Request Connected Gamepads
        Debug.Log("Application Handshake Accepted");
    }

    public void InjectGamepadHandshake(SocketIOEvent e) {
        HandleGamepadHandshake(e);
    }

    protected virtual void HandleGamepadHandshake(SocketIOEvent e) {
        //Upon Handshake, create the Gamepad
        SGHandshakeMsg handshakeMsg = new SGHandshakeMsg(e.data);

        int NextPlayerId = AddPlayerToFreeSlot();
        SocketGamepad gamepad = CreateGamepad((NextPlayerId >= 0) ? LocalPlayers[NextPlayerId] : null, handshakeMsg.PlayerId); //Create unassociated Gamepad
        gamepad.PlayerId = handshakeMsg.PlayerId;

        if (InputIndicatorPrefab != null) {
            SocketGamepadTestIndicator Tester = ((GameObject)Instantiate(InputIndicatorPrefab)).GetComponent<SocketGamepadTestIndicator>();
            Tester.Gamepad = gamepad;
            Indicators.Add(gamepad.PlayerId, Tester.gameObject);
        }

        Debug.Log("Gamepad Handshake: " + gamepad.PlayerId.ToString());
    }

    public void InjectGamepadUpdate(SocketIOEvent e) {
        HandleGamepadUpdate(e);
    }

    //Change Input array to Dictionary of named delegates instead of an array of 20 floats
    protected virtual void HandleGamepadUpdate(SocketIOEvent e) {
        int gid = -1;
        e.data.GetField(ref gid, "PlayerId");

        if (gid > -1) {
            SocketGamepad gamepad = GetGamepad(gid);
            if (gamepad == null) {
                Debug.Log("Controller " + gid + " sending Updates without handshake!");
            } else {
                gamepad.InputData = e.data.GetField("InputData");
            }
        } else {
            //Hack Attempt?
            Debug.LogError("STOP IT, I'M NOT A DOCTOR!!");
        }
    }

    protected virtual void HandleGamepadDisconnected(SocketIOEvent e) {
        int gid = 0;
        e.data.GetField(ref gid, "PlayerId");
        if (gid > -1 && gid < Gamepads.Count) {
            if (Indicators.ContainsKey(gid)) {
                GameObject g = Indicators[gid];
                Indicators.Remove(gid);
                Destroy(g);
            }

            SocketGamepad gamepad = GetGamepad(gid);
            Debug.Log("Removing Gamepad " + gid.ToString());
            Gamepads.Remove(gamepad);
            Destroy(gamepad);
            RenumberConnectedGamepads();
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

    //Local, Manual update for testing
    /*
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
    */
}
