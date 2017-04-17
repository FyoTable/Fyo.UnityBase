using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketGamepadDisconnectMessage : JSONObject {
    public int SocketGamepadID;

    public SocketGamepadDisconnectMessage() {
        AddField("PlayerId", SocketGamepadID);
    }

    public SocketGamepadDisconnectMessage(JSONObject clone) {
        clone.GetField(ref SocketGamepadID, "PlayerId");
        Serialize();
    }

    public void Serialize() {
        SetField("PlayerId", SocketGamepadID);
    }

    public void Deserialize() {
        GetField(ref SocketGamepadID, "PlayerId");
    }
}
