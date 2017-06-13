using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    [RequireComponent(typeof(Animator))]
    public class CharacterMovement : MonoBehaviour {
        public Animator animator;
        Rigidbody body;
        public float Speed;
        public Vector3 Move = Vector3.zero;
        protected float MoveMag = 0;
        protected Vector3 FrameMove = Vector3.zero;
        
        private void Start() {
            animator = GetComponent<Animator>();
            body = GetComponent<Rigidbody>();
        }
        
        private void FixedUpdate() {
            FrameMove = Move * Speed;// * Time.deltaTime;
            body.AddForce(FrameMove, ForceMode.VelocityChange);
            MoveMag = FrameMove.magnitude;
            if(Mathf.Abs(body.velocity.x) + Mathf.Abs(body.velocity.z) > Speed) {
                Vector3 FlatVelocity = body.velocity;
                FlatVelocity.y = 0;
                FlatVelocity = (FlatVelocity.normalized * Speed) + (Vector3.up * body.velocity.y);
                body.velocity = FlatVelocity;
            }

            if(Mathf.Approximately(MoveMag, 0.0f)) {
                Vector3 FlatVelocity = body.velocity;
                body.velocity = (Vector3.up * body.velocity.y);
            }

            animator.SetBool("IsWalking", (MoveMag > 0.0f));
        }
    }
}
