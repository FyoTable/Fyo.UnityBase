using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaddleBricks {
    public class Brick : MonoBehaviour {
        public int Hits = 1;
        public int ScoreValue = 1;
        UnityEngine.UI.Image img;
        public Color color {
            set {
                if(img == null)
                    img = GetComponent<UnityEngine.UI.Image>();
                img.color = value;
            }
            get {
                if (img == null)
                    img = GetComponent<UnityEngine.UI.Image>();
                return img.color;
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision) {
            Ball ball = collision.collider.GetComponent<Ball>();
            if (ball != null) {
                Hits--;
                if (Hits <= 0) {
                    if (ball.LastPlayerHit != null) {
                        ball.LastPlayerHit.Score += ScoreValue;
                    }

                    gameObject.SetActive(false);
                }
            }
        }
    }
}
