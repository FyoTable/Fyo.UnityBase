using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGHandshakeMsg : JSONObject {
    public string DeviceId;
    public int PlayerId;

    private void Setup() {
        AddField("PlayerId", PlayerId);
        AddField("DeviceId", CreateStringObject(DeviceId));
    }

    public SGHandshakeMsg() : base() {
        Setup();
    }

    public SGHandshakeMsg(JSONObject clone) : base() {
        Setup();
        Clone(clone);
    }

    public void Clone(JSONObject clone) {
        if (clone.HasField("PlayerId"))
            clone.GetField(ref PlayerId, "PlayerId");
        if(clone.HasField("DeviceId"))
            clone.GetField(ref DeviceId, "DeviceId");
        Serialize();
    }

    public void Serialize() {
        SetField("PlayerId", PlayerId);
        SetField("DeviceId", CreateStringObject(DeviceId));
    }

    public void Deserialize() {
        GetField(ref PlayerId, "PlayerId");
        GetField(ref DeviceId, "DeviceId");
    }
}
