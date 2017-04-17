using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Whole Application Handshake
public class AppHandshakeMsg : JSONObject {
    public string AppIDString = "Marquee";

    public string Controller = string.Empty;
    public string BinaryData = null;

    public AppHandshakeMsg() : base() {
        AddField("AppIDString", AppIDString);
        AddField("Controller", Controller);
        AddField("BinaryData", BinaryData);
    }

    public AppHandshakeMsg(string idStr, string binStr, string controller) : base() {
        AppIDString = idStr;
        Controller = controller;
        BinaryData = binStr;
        Serialize();
    }
    
    public AppHandshakeMsg(JSONObject clone) : base() {
        clone.GetField(ref AppIDString, "AppIDString");
        clone.GetField(ref Controller, "Controller");
        clone.GetField(ref BinaryData, "BinaryData");
        Serialize();
    }

    public void Serialize() {
        SetField("AppIDString", CreateStringObject(AppIDString));
        SetField("Controller", CreateStringObject(Controller));
        SetField("BinaryData", CreateStringObject(BinaryData));
    }

    public void Deserialize() {
        GetField(ref AppIDString, "AppIDString");
        GetField(ref Controller, "Controller");
        GetField(ref BinaryData, "BinaryData");
    }
}
