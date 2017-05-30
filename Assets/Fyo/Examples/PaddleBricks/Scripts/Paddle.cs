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
        public float BallLaunchVelocity = 540.0f;

        RectTransform rt;
        private void Start() {
            rt = (RectTransform)transform;
        }
        
        float CurrentAccel = 0.0f;
        float TotalAccel = 0.0f;
        float Direction = 0.0f;
        
        private void Update() {
            if (Gamepad != null) {// && Ready) {
                //Acceleration = Distance / Time
                //Distance = Acceleration * Time

                CurrentAccel = Gamepad.GetAxis("axis 0") * Time.deltaTime;
                TotalAccel += CurrentAccel;

                if (Direction == 0) {
                    if (TotalAccel > 0)
                        Direction = 1.0f;
                    else if (TotalAccel < 0)
                        Direction = -1.0f;
                    
                }

                PaddleDistance += TotalAccel * PaddleSpeed;
                if (Mathf.Abs(PaddleDistance) > PaddleMove.x) {
                    PaddleDistance = Mathf.Sign(PaddleDistance) * PaddleMove.x;
                }
                rt.localPosition = new Vector3(PaddleDistance, rt.localPosition.y, rt.localPosition.z);
                
                if (AttachedBall != null) {
                    if (Gamepad.GetButton("button 0")) {
                        AttachedBall.body.bodyType = RigidbodyType2D.Dynamic;
                        AttachedBall.body.velocity = transform.up * BallLaunchVelocity;
                        AttachedBall.transform.parent = transform.parent;
                        AttachedBall = null;
                    }
                }
            } else {
                if (PlayerId == 0) {
                    //Master Controller
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            Ball b = collision.gameObject.GetComponent<Ball>();
            if(b != null) {
                b.body.velocity = (collision.transform.position - transform.position).normalized * b.body.velocity.magnitude;
            }
        }
    }
}