using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaddleBricks {

    public class Ball : MonoBehaviour {
        public Rigidbody2D body;
        new private CircleCollider2D collider;

        public bool RandomizeStartAngle = true;
        public float StartAngle = 0.0f;
        public bool RandomizeStartVelocity = false;
        [Range(0.0f, 100.0f)]
        public float StartVelocity = 1.0f;

        public Paddle LastPlayerHit = null;

        public static Vector2 RadianToVector2(float radian) {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree) {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }

        private void Awake() {
            if (RandomizeStartAngle) {
                StartAngle = Random.Range(0.0f, 360.0f);
            }

            if (RandomizeStartVelocity) {
                StartVelocity = Random.Range(1.0f, StartVelocity);
            }

            body = GetComponent<Rigidbody2D>();
            collider = GetComponent<CircleCollider2D>();

            body.velocity = DegreeToVector2(StartAngle) * StartVelocity;
            
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            Paddle paddle = collision.collider.GetComponent<Paddle>();
            if (paddle != null) {
                LastPlayerHit = paddle;
            }
        }
    }
}
