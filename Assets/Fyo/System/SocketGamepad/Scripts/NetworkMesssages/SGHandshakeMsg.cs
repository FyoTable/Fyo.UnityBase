using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGHandshakeMsg : JSONObject {
    public int PlayerId;
    public bool IsMaster = false;

    public SGHandshakeMsg() {
        AddField("PlayerId", PlayerId);
    }

    public SGHandshakeMsg(JSONObject clone) {
        clone.GetField(ref PlayerId, "PlayerId");
        Serialize();
    }

    public void Serialize() {
        SetField("PlayerId", PlayerId);
    }

    public void Deserialize() {
        GetField(ref PlayerId, "PlayerId");
    }
}
