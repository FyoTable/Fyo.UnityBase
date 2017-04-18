using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExampleTicTacToe {
    public class TicTacToeCell : MonoBehaviour {
        public Animator animator;
        public GameObject X;
        public GameObject O;

        public int CurrentMark {
            get {
                if (X != null && X.activeSelf)
                    return 1;
                if (O != null && O.activeSelf)
                    return 2;
                return 0;
            }
        }

        public void Start() {
            X = transform.GetChild(0).gameObject;
            O = transform.GetChild(1).gameObject;
        }
    }
}