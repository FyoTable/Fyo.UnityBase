using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGUpdateMsg : JSONObject {
    public int SocketGamepadID = -1;
    public float[] inputs = new float[SocketGamepad.GamepadInputCount];

    public SGUpdateMsg() : base() {
        InitializeKeys();
        Serialize();
    }

    public SGUpdateMsg(JSONObject msgObject) : base() {
        InitializeKeys();

        if (msgObject.HasField("SocketGamepadID")) {
            if (!int.TryParse(msgObject.GetField("SocketGamepadID").str, out SocketGamepadID)) {
                Debug.LogError("Socket Gamepad Message missing id");
            }

            SocketGamepadID = int.Parse(msgObject.GetField("SocketGamepadID").str);
            JSONObject inputJson = msgObject.GetField("inputs");
            string inputStr = inputJson.str;
            string[] strArr = inputStr.Split(',');
            if (strArr.Length > 0) {
                for (int i = 0; i < strArr.Length; i++) {
                    if (!float.TryParse(strArr[i], out inputs[i])) {
                        Debug.LogError("Unable to deserialize Malformed JSON String");
                    }
                }
            }
        }
    }

    public SGUpdateMsg(SocketGamepad gamepad) : base() {
        InitializeKeys();
        SocketGamepadID = gamepad.ID;
        inputs = gamepad.inputs;
        Serialize();
    }

    void InitializeKeys() {
        AddField("SocketGamepadID", -1);
        AddField("inputs", "{" +
        "0.0f, 0.0f, 0.0f, 0.0f, 0.0f," +
        "0.0f, 0.0f, 0.0f, 0.0f, 0.0f," +
        "0.0f, 0.0f, 0.0f, 0.0f, 0.0f," +
        "0.0f, 0.0f, 0.0f, 0.0f, 0.0f" +
        "}");
    }

    public string Serialize() {
        string r = string.Empty;

        if (keys.Count == 0) {
            InitializeKeys();
        }

        SetField("SocketGamepadID", SocketGamepadID);

        string strInputs = "{";
        for (int i = 0; i < inputs.Length; i++) {
            strInputs += inputs[i].ToString();
            if (i != inputs.Length - 1) {
                strInputs += ",";
            }
        }
        strInputs += "}";

        SetField("inputs", new JSONObject(strInputs));

        return r;
    }

    void Deserialize() {
        if (keys.Count == 0) {
            InitializeKeys();
        }

        SocketGamepadID = int.Parse(GetField("SocketGamepadID").str);
        JSONObject inputJson = GetField("inputs");
        string inputStr = inputJson.str;
        string[] strArr = inputStr.Split(',');
        if (strArr.Length > 0) {
            for (int i = 0; i < strArr.Length; i++) {
                if (!float.TryParse(strArr[i], out inputs[i])) {
                    Debug.LogError("Unable to deserialize Malformed JSON String");
                }
            }
        }
    }
}