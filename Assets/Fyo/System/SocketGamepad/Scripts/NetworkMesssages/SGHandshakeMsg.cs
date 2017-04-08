using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGHandshakeMsg : JSONObject {
    public string DeviceId;
    public int PlayerId;
    public string Controller;

    private void Setup() {
        AddField("PlayerId", PlayerId);
        AddField("DeviceId", CreateStringObject(DeviceId));
        AddField("Controller", CreateStringObject(Controller));
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
        if (clone.HasField("Controller"))
            clone.GetField(ref DeviceId, "Controller");
        Serialize();
    }

    public void Serialize() {
        SetField("PlayerId", PlayerId);
        SetField("DeviceId", CreateStringObject(DeviceId));
        SetField("Controller", CreateStringObject(Controller));
    }

    public void Deserialize() {
        GetField(ref PlayerId, "PlayerId");
        GetField(ref DeviceId, "DeviceId");
        GetField(ref Controller, "Controller");
    }
}
