using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fyo;

namespace InputTestApp {
    public class InputTestPlayer : FyoPlayer {
        public List<GameObject> Indicators = new List<GameObject>(18);

        public float[] InputDebug = new float[18];

        private void Start() {
            for (b = 0; b < 10; b++) {
                Indicators[b].SetActive(false);
            }
        }

        int a = 0, b = 0;
        bool button = false;
        float axis = 0.0f;
        GameObject Indicator;
        private void LateUpdate() {
            if (Gamepad != null) {
                for (b = 0; b < 10; b++) {
                    Indicator = Indicators[b];
                    button = Gamepad.GetButton("button " + b.ToString());
                    Indicator.SetActive(button);
                    InputDebug[b] = button ? 1.0f : 0.0f;
                }

                for (a = 0; a < 8; a++) {
                    Indicator = Indicators[10 + a];
                    axis = Gamepad.GetAxis("axis " + a.ToString());
                    Indicator.transform.localPosition = Indicator.transform.right * axis;
                    InputDebug[10 + a] = axis;
                }
            }
        }
    }
}
