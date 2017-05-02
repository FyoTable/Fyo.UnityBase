using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fyo {
    public class FyoPlayer : MonoBehaviour {
        public FyoApplication FyoApp;
        public SocketGamepad Gamepad;
        public int PlayerId;
        public bool Ready = false;
        public long InputWait = 0;
    }
}
