using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fyo;

namespace PaddleBricks {

    public class Paddle : FyoPlayer {
        //Direction a Paddle may move
        public float PaddleSpeed = 1000;
        public Vector3 PaddleMove = new Vector3(50, 0, 0);
        protected float PaddleDistance = 0;
        public long Score = 0;
        public Ball AttachedBall;
        public float BallLaunchVelocity = 2.0f;

        RectTransform rt;
        private void Start() {
            rt = (RectTransform)transform;
        }
        private void Update() {
            if (Gamepad != null) {// && Ready) {
                PaddleDistance += Gamepad.GetAxis("axis 0") * PaddleSpeed * Time.deltaTime;
                if (Mathf.Abs(PaddleDistance) > PaddleMove.x) {
                    PaddleDistance = PaddleMove.x * Mathf.Sign(PaddleDistance);
                }
                rt.localPosition = new Vector3(PaddleDistance, rt.localPosition.y, rt.localPosition.z) ;

                if (AttachedBall != null) {
                    if (Gamepad.GetButton("button 0")) {
                        AttachedBall.body.bodyType = RigidbodyType2D.Dynamic;
                        AttachedBall.body.velocity = transform.up * BallLaunchVelocity;
                        AttachedBall = null;
                    }
                }
            } else {
                if (PlayerId == 0) {
                    //Master Controller
                }
            }
        }
    }
}