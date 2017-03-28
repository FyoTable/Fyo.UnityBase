using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketGamepadDisconnectMessage : JSONObject {
    public int SocketGamepadID;

    public SocketGamepadDisconnectMessage() {
        AddField("SocketGamepadID", SocketGamepadID);
    }

    public SocketGamepadDisconnectMessage(JSONObject clone) {
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
