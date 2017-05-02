using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fyo;

namespace PaddleBricks {

    public class Paddle : FyoPlayer {
        //Direction a Paddle may move
        public Vector3 PaddleMove = new Vector3(50, 0, 0);
        protected Vector3 CurrentOffset = Vector3.zero;
        public long Score = 0;

        private void Update() {
            if (Gamepad != null) {
                Vector3.ClampMagnitude(CurrentOffset += Gamepad.GetAxis("axis 0") * PaddleMove * Time.deltaTime, PaddleMove.magnitude);
                transform.localPosition = CurrentOffset;
            }
        }
    }
}