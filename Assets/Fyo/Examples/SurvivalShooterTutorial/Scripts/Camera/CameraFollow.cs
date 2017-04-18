using UnityEngine;
using System.Collections;

namespace SurvivalShooterExampleGame {
    public class CameraFollow : MonoBehaviour
    {
        public GameObject target;            // The position that that camera will be following.
        public float smoothing = 5f;        // The speed with which the camera will be following.
        Vector3 offset;                     // The initial offset from the target.

        void Start () {
            // Calculate the initial offset.
            if (target != null)
                offset = transform.position - target.transform.position;
        }


        void FixedUpdate () {
            if (target != null) {
                // Create a postion the camera is aiming for based on the offset from the target.
                Vector3 targetCamPos = target.transform.position + offset;

                // Smoothly interpolate between the camera's current position and it's target position.
                transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
            }
        }
    }
}