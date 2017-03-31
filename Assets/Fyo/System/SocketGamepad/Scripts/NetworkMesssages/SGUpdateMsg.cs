using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGUpdateMsg : JSONObject {
    public int PlayerId = -1;
    public JSONObject InputData = new JSONObject();

    public SGUpdateMsg() : base() {
        AddField("PlayerId", -1);

        //Allocate Axes
        for (int a = 0; a < 9; a++)
            InputData.AddField("axis " + a.ToString(), 0.0f);

        //Allocate Buttons
        for (int b = 0; b < 10; b++)
            InputData.AddField("button " + b.ToString(), false);

        AddField("InputData", InputData);
    }

    public SGUpdateMsg(SocketGamepad gamepad) : base() {
        PlayerId = gamepad.PlayerId;
        InputData = gamepad.InputData;
        Serialize();
    }
    
    public void Serialize() {
        SetField("PlayerId", PlayerId);
        SetField("InputData", InputData);
    }

    void Deserialize() {
        PlayerId = int.Parse(GetField("PlayerId").str);
        InputData = GetField("InputData");
    }
}