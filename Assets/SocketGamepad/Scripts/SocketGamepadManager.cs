using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

[RequireComponent(typeof(SocketIOComponent))]
public class SocketGamepadManager : MonoBehaviour {
    protected SocketIOComponent socket;

    public Dictionary<SocketGamepad, GamePlayer> Gamepads = new Dictionary<SocketGamepad, GamePlayer>();
    protected Queue<SGUpdateMsg> InputMessageQueue = new Queue<SGUpdateMsg>();

    protected int LatencyRoundTrip;
    protected int LatencyToWebSocket = 0;

    public string AppIdString = "Unknown";

    public virtual void Start() {
        socket = GetComponent<SocketIOComponent>();

        #region Message Handlers
        //Connection Accepted
        socket.On("connect", HandleConnectedToSGServer);        //Connected, Send App Handshake with App Info
        //Handshake Accepted
        socket.On("AppHandshakeMsg", HandleAppHandshake);       //App Handshake Accepted
        //Gamepad Connected
        socket.On("SGHandshakeMsg", HandleGamepadHandshake);    //Handshake a Gamepad
        //Gamepad Update
        socket.On("SGUpdate", HandleGamepadUpdate);             //Message for a single gamepad
        //Gamepad Disconnected
        socket.On("SGDisconnectMsg", HandleGamepadDisconnected);//Gamepad Disconnected
        #endregion

        socket.Connect();
        Debug.Log("Connecting...");
    }

    /* public void SendHandshakeTest() {
        SocketGamepadManagerHandshakeMessage msg = new SocketGamepadManagerHandshakeMessage();
        socket.Emit("SocketGamepadManagerHandshakeMessage", msg);
    }*/

    #region Utility
    protected int GetGamepadIdFromEvent(SocketIOEvent e) {
        int r = -1;
        if (e.data != null) {
            if (e.data.HasField("SocketGamepadID")) {
                if (!int.TryParse(e.data.GetField("SocketGamepadID").ToString(), out r)) {
                    Debug.LogError("[SocketGamepadManager] Invalid Id Field: \"" + e.data.GetField("SocketGamepadID").ToString() + "\"");
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
        
        JSONObject InputFields = e.data.GetField("inputs");
        int b;
        for (b = 0; b < 20; b++) {
            r[b] = InputFields.list[b].f;
        }

        return r;
    }
    #endregion

    public delegate void OnAddGamepadDelegate(int GamepadId);
    public OnAddGamepadDelegate dAddGamepad = null;
    
    public SocketGamepad CreateGamepad(GamePlayer player) {
        SocketGamepad gamepad = gameObject.AddComponent<SocketGamepad>();
        gamepad.LocalId = Gamepads.Count;
        Gamepads.Add(gamepad, player);

        if (dAddGamepad != null)
            dAddGamepad(gamepad.ID);

        return gamepad;
    }

    public virtual void AddExistingGamepad(SocketGamepad gamepad) {
        gamepad.LocalId = Gamepads.Count;
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

    protected void RenumberLocalGamepads() {
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
            if (e.Current.Key.ID == GamepadId)
                return e.Current.Key;
        }

        return null;
    }

    public virtual void HandleConnectedToSGServer(SocketIOEvent e) {
        AppHandshakeMsg GameInfoMsg = new AppHandshakeMsg();
        GameInfoMsg.AppIDString = AppIdString;

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
        SocketGamepad gamepad = CreateGamepad(null); //Create unassociated Gamepad
        gamepad.ID = handshakeMsg.SocketGamepadID;
        
        if (InputIndicatorPrefab != null) {
            SocketGamepadTestIndicator Tester = ((GameObject)Instantiate(InputIndicatorPrefab)).GetComponent<SocketGamepadTestIndicator>();
            Tester.Gamepad = gamepad;
        }

        Debug.Log("Gamepad Handshake: " + gamepad.ID.ToString());
    }

    public virtual void HandleGamepadUpdate(SocketIOEvent e) {
        int gid = 0;
        e.data.GetField(ref gid, "SocketGamepadID");

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
        e.data.GetField(ref gid, "SocketGamepadID");
        if (gid > -1 && gid < Gamepads.Count) {
            SocketGamepad gamepad = GetGamepad(gid);
            Debug.Log("Removing Gamepad " + gid.ToString());
            Gamepads.Remove(gamepad);
            Destroy(gamepad);
            RenumberLocalGamepads();
        }
    }
    
    //Local, Manual update for testing
    public void UpdateInput(SocketIOEvent e) {
        int gid = GetGamepadIdFromEvent(e);
        if (gid < Gamepads.Count) {
            SocketGamepad pad = GetGamepad(gid);
            SGUpdateMsg msg = new SGUpdateMsg(e.data);
            pad.inputs = msg.inputs;
            //pad.Color = msg.color;
        } else {
            Debug.LogError("[SocketGamepadManager] Error - Gamepad Index out of range");
        }
    }
}
