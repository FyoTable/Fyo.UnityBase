using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Whole Application Handshake
public class AppHandshakeMsg : JSONObject {
    public string AppIDString = "Marquee";
    public string BinaryData = null;

    public AppHandshakeMsg() {
        AddField("AppIDString", AppIDString);
        AddField("BinaryData", BinaryData);
    }
    
    public AppHandshakeMsg(JSONObject clone) {
        clone.GetField(ref AppIDString, "AppIDString");
        clone.GetField(ref BinaryData, "BinaryData");
        Serialize();
    }

    public void Serialize() {
        SetField("AppIDString", AppIDString);
        SetField("BinaryData", BinaryData);
        //SetField("Colors");
    }

    public void Deserialize() {
        GetField(ref AppIDString, "AppIDString");
        GetField(ref BinaryData, "BinaryData");
    }
}
