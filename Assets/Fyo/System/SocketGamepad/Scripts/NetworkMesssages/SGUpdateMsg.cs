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

        if (msgObject.HasField("PlayerId")) {
            if (!int.TryParse(msgObject.GetField("PlayerId").str, out SocketGamepadID)) {
                Debug.LogError("Socket Gamepad Message missing id");
            }

            SocketGamepadID = int.Parse(msgObject.GetField("PlayerId").str);
            JSONObject inputJson = msgObject.GetField("data");
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
        SocketGamepadID = gamepad.PlayerId;
        inputs = gamepad.inputs;
        Serialize();
    }

    void InitializeKeys() {
        AddField("PlayerId", -1);
        AddField("data", "{" +
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

        SetField("PlayerId", SocketGamepadID);

        string strInputs = "{";
        for (int i = 0; i < inputs.Length; i++) {
            strInputs += inputs[i].ToString();
            if (i != inputs.Length - 1) {
                strInputs += ",";
            }
        }
        strInputs += "}";

        SetField("data", new JSONObject(strInputs));

        return r;
    }

    void Deserialize() {
        if (keys.Count == 0) {
            InitializeKeys();
        }

        SocketGamepadID = int.Parse(GetField("PlayerId").str);
        JSONObject inputJson = GetField("data");
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