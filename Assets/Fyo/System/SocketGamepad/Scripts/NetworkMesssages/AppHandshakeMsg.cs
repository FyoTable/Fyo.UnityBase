using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Whole Application Handshake
public class AppHandshakeMsg : JSONObject {
    public string AppIDString = "Marquee";
    public string BinaryData = null;

    public AppHandshakeMsg() : base() {
        AddField("AppIDString", AppIDString);
        AddField("BinaryData", BinaryData);
    }

    public AppHandshakeMsg(string idStr, string binStr) : base() {
        AppIDString = idStr;
        binStr = BinaryData;
        Serialize();
    }
    
    public AppHandshakeMsg(JSONObject clone) : base() {
        clone.GetField(ref AppIDString, "AppIDString");
        clone.GetField(ref BinaryData, "BinaryData");
        Serialize();
    }

    public void Serialize() {
        SetField("AppIDString", CreateStringObject(AppIDString));
        SetField("BinaryData", CreateStringObject(BinaryData));
    }

    public void Deserialize() {
        GetField(ref AppIDString, "AppIDString");
        GetField(ref BinaryData, "BinaryData");
    }
}
