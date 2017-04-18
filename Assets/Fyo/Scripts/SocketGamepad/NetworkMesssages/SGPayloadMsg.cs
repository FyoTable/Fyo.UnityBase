using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fyo {
    public class SGPayloadMsg : JSONObject {
        public string Filename = "index.zip";
        public string BinaryData = string.Empty;

        private void Setup() {
            AddField("BinaryData", new JSONObject());
        }

        public SGPayloadMsg() : base() {
            Setup();
        }

        public SGPayloadMsg(string filename) : base() {
            Filename = filename;
            Serialize();
        }

        void Start() {
            AddField("BinaryData", new JSONObject());
        }

        public void Serialize() {
            string FilePath = Fyo.DefaultPaths.Controllers + Filename;
            if (File.Exists(FilePath)) {
                byte[] data = File.ReadAllBytes(FilePath);
                BinaryData = System.Convert.ToBase64String(data);
                SetField("BinaryData", JSONObject.CreateStringObject(BinaryData));
            } else
                Debug.LogWarning(FilePath + " does not exist!");
        }
    }
}