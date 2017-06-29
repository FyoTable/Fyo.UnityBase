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
        public Color color = Color.green;

        RectTransform rt;
        private void Start() {
            rt = (RectTransform)transform;
        }
        
        float CurrentDistance = 0.0f;
        float TotalDistance = 0.0f;
        float Direction = 0.0f;
        
        private void Update() {
            if (Gamepad != null) {// && Ready) {
                //Acceleration = Distance / (Time * Time)
                //Distance = 0.5f * Acceleration * (Time * Time)
                //Distance = Speed * Time
                //Speed = Distance / Time
                //Time = Distance / Speed

                CurrentDistance = 0.5f * Gamepad.GetAxis("axis 0") * (Time.deltaTime * Time.deltaTime);
                TotalDistance += CurrentDistance;

                if (Direction == 0) {
                    if (TotalDistance > 0)
                        Direction = 1.0f;
                    else if (TotalDistance < 0)
                        Direction = -1.0f;
                    
                }

                PaddleDistance += TotalDistance * PaddleSpeed;
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