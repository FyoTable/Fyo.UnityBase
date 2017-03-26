using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGHandshakeMsg : JSONObject {
    public int SocketGamepadID;
    public bool IsMaster = false;

    public SGHandshakeMsg() {
        AddField("SocketGamepadID", SocketGamepadID);
    }

    public SGHandshakeMsg(JSONObject clone) {
        clone.GetField(ref SocketGamepadID, "SocketGamepadID");
        Serialize();
    }

    public void Serialize() {
        SetField("SocketGamepadID", SocketGamepadID);
    }

    public void Deserialize() {
        GetField(ref SocketGamepadID, "SocketGamepadID");
    }
}
