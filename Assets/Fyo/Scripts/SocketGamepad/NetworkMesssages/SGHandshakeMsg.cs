using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fyo {
    public class SGHandshakeMsg : JSONObject {
        public string DeviceId;
        public int SGID;
        public string Controller;

        private void Setup() {
            AddField("PlayerId", SGID);
            AddField("DeviceId", CreateStringObject(DeviceId));
            AddField("Controller", CreateStringObject(Controller));
        }

        public SGHandshakeMsg() : base() {
            Setup();
        }

        public SGHandshakeMsg(JSONObject clone) : base() {
            Setup();
            Clone(clone);
        }

        public void Clone(JSONObject clone) {
            if (clone.HasField("PlayerId"))
                clone.GetField(ref SGID, "PlayerId");
            if (clone.HasField("DeviceId"))
                clone.GetField(ref DeviceId, "DeviceId");
            if (clone.HasField("Controller"))
                clone.GetField(ref DeviceId, "Controller");
            Serialize();
        }

        public void Serialize() {
            SetField("PlayerId", SGID);
            SetField("DeviceId", CreateStringObject(DeviceId));
            SetField("Controller", CreateStringObject(Controller));
        }

        public void Deserialize() {
            GetField(ref SGID, "PlayerId");
            GetField(ref DeviceId, "DeviceId");
            GetField(ref Controller, "Controller");
        }
    }
}
