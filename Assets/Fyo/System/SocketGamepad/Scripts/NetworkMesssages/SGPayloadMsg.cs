using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SGPayloadMsg : JSONObject {
    public string Filename = "index.zip";

    // Use this for initialization
    void Start() {
        AddField("BinaryData", new JSONObject());
    }

    public void Serialize() {
        if (File.Exists(Filename)) {
            byte[] data = File.ReadAllBytes(Filename);
            string strData = System.Convert.ToBase64String(data);
            SetField("BinaryData", JSONObject.CreateStringObject(strData));
        } else
            Debug.LogWarning("\"" + Filename + "\" does not exist!");
    }
}
