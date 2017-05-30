using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fyo;

namespace PaddleBricks {
    public class Goal : MonoBehaviour {
        public FyoPlayer Player;
        protected BoxCollider2D collider;

        protected void Start() {
            collider = GetComponent<BoxCollider2D>();
        }
    }
}
