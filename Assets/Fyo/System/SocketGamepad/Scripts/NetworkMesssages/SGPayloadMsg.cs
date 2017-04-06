using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SGPayloadMsg : JSONObject {
    public string Filename = "index.zip";

    public SGPayloadMsg(string filename) : base() {
        Filename = filename;
        Serialize();
    }

    public SGPayloadMsg() : base() {
    }

    // Use this for initialization
    void Start() {
        AddField("BinaryData", new JSONObject());
    }

    public void Serialize() {
        string FilePath = Fyo.Paths.Controllers + Filename;
        if (File.Exists(FilePath)) {
            byte[] data = File.ReadAllBytes(FilePath);
            string strData = System.Convert.ToBase64String(data);
            SetField("BinaryData", JSONObject.CreateStringObject(strData));
        } else
            Debug.LogWarning(FilePath + " does not exist!");
    }
}
