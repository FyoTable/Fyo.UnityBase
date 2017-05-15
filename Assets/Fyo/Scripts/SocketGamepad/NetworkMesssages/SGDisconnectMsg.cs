using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fyo {
    public class SGDisconnectMsg : JSONObject {

        public int SGID = -1;
        public string MessageType = "";
        public JSONObject Data = new JSONObject();

        private void Setup() {
            AddField("PlayerId", -1);
            AddField("MessageType", "");
            AddField("data", Data);
        }

        public SGDisconnectMsg() : base() {
            Setup();
        }

        public SGDisconnectMsg(SocketGamepad gamepad) : base() {
            Setup();
            SGID = gamepad.SGID;
            MessageType = "input";
            Data = gamepad.InputData;
            Serialize();
        }

        public SGDisconnectMsg(JSONObject clone) : base() {
            Setup();
            if (clone.HasField("PlayerId"))
                clone.GetField(ref SGID, "PlayerId");
            if (clone.HasField("MessageType"))
                clone.GetField(ref MessageType, "MessageType");
            if (clone.HasField("data"))
                Data = clone["data"];
            Serialize();
        }

        public void Serialize() {
            SetField("PlayerId", SGID);
            SetField("MessageType", CreateStringObject(MessageType));
            SetField("data", Data);
        }

        void Deserialize() {
            SGID = int.Parse(GetField("PlayerId").str);
            MessageType = GetField("MessageType").str;
            Data = this["data"];
        }
    }
}
