using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fyo {
    public class UICameraSet : MonoBehaviour {
        Quaternion DefaultRotation;
        Canvas canvas;
        // Use this for initialization
        void Start() {
            DefaultRotation = transform.rotation;
            canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
        }

        private void FixedUpdate() {
            transform.rotation = DefaultRotation;
        }
        private void Update() {
            transform.rotation = DefaultRotation;
        }
        private void LateUpdate() {
            transform.rotation = DefaultRotation;
        }
        private void OnGUI() {
            transform.rotation = DefaultRotation;
        }
    }
}
