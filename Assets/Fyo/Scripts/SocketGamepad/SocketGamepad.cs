using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace Fyo {
    /// <summary>
    /// A Virtual Gamepad which is managed by a remote Node.JS endpoint server
    /// 
    /// 
    /// </summary>
    public class SocketGamepad : MonoBehaviour {
        public int SGID = -1;
        public const int GamepadInputCount = 20;
        public int LocalId = -1;
        public string DeviceId = string.Empty;

        public JSONObject InputData = new JSONObject();
        public float[] DataView = new float[20];

        public float GetAxis(string InputName) {
            return InputData.HasField(InputName) ?
                InputData.GetField(InputName).n :
                0.0f;
        }

        public bool GetButton(string InputName) {
            return InputData.HasField(InputName) ?
                InputData.GetField(InputName).b :
                false;
        }

        public string GetString(string InputName) {
            return InputData.HasField(InputName) ?
                InputData.GetField(InputName).str :
                string.Empty;
        }

        public JSONObject GetObject(string InputName) {
            return InputData.HasField(InputName) ?
                InputData.GetField(InputName) :
                null;
        }
    }
}
