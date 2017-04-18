using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fyo {
    public class CameraFraming : MonoBehaviour {
        public List<GameObject> TrackedObjects = new List<GameObject>();
        public Vector3 MinimumVariance = Vector3.zero;
        public Vector3 MaximumVariance = Vector3.one * 0.1f;
        public bool AlignWithWorldCenterOfObjects = true;
        public bool ZoomToFit = true;
        public float MinimumOrthoSize = 6.0f;

        Vector3 WorldCenterOfObjects = Vector3.zero;
        Vector3 AxisAlignedMinimumBound = Vector3.zero;
        Vector3 AxisAlignedMaximumBound = Vector3.zero;

        private void LateUpdate() {
            WorldCenterOfObjects = Vector3.zero;
            AxisAlignedMinimumBound = Vector3.zero;
            AxisAlignedMaximumBound = Vector3.zero;
            if (TrackedObjects.Count > 0) {
                for (int i = 0; i < TrackedObjects.Count; i++) {
                    WorldCenterOfObjects += TrackedObjects[i].transform.position;

                    if (TrackedObjects[i].transform.position.x < AxisAlignedMinimumBound.x)
                        AxisAlignedMinimumBound.x = TrackedObjects[i].transform.position.x;
                    if (TrackedObjects[i].transform.position.x > AxisAlignedMaximumBound.x)
                        AxisAlignedMaximumBound.x = TrackedObjects[i].transform.position.x;

                    if (TrackedObjects[i].transform.position.z < AxisAlignedMinimumBound.z)
                        AxisAlignedMinimumBound.z = TrackedObjects[i].transform.position.z;
                    if (TrackedObjects[i].transform.position.z > AxisAlignedMaximumBound.z)
                        AxisAlignedMaximumBound.z = TrackedObjects[i].transform.position.z;
                }
            }

            AxisAlignedMinimumBound = new Vector3(
                AxisAlignedMinimumBound.x < MinimumVariance.x ? MinimumVariance.x : AxisAlignedMinimumBound.x,
                AxisAlignedMinimumBound.y < MinimumVariance.y ? MinimumVariance.y : AxisAlignedMinimumBound.y,
                AxisAlignedMinimumBound.z < MinimumVariance.z ? MinimumVariance.z : AxisAlignedMinimumBound.z);

            AxisAlignedMaximumBound = new Vector3(
                AxisAlignedMaximumBound.x > MaximumVariance.x ? MaximumVariance.x : AxisAlignedMaximumBound.x,
                AxisAlignedMaximumBound.y > MaximumVariance.y ? MaximumVariance.y : AxisAlignedMaximumBound.y,
                AxisAlignedMaximumBound.z > MaximumVariance.z ? MaximumVariance.z : AxisAlignedMaximumBound.z);

            WorldCenterOfObjects = (AxisAlignedMaximumBound - AxisAlignedMinimumBound) * 0.5f;
            WorldCenterOfObjects.y = transform.position.y;
            transform.position = WorldCenterOfObjects;

            if (AlignWithWorldCenterOfObjects && TrackedObjects.Count > 0) {
                WorldCenterOfObjects.y = transform.position.y;
                transform.position = WorldCenterOfObjects;
            }

            if (ZoomToFit) {
                Camera camera = GetComponent<Camera>();
                if (camera != null) {
                    float Width = Mathf.Abs((AxisAlignedMaximumBound.x - AxisAlignedMinimumBound.x) * 0.5f);
                    float Depth = Mathf.Abs((AxisAlignedMaximumBound.z - AxisAlignedMinimumBound.z) * 0.5f);
                    camera.orthographicSize = Mathf.Clamp(Mathf.Min(Width, Depth), MinimumOrthoSize, Mathf.Max(Width, Depth));
                    Debug.Log("Ortho " + camera.orthographicSize);
                }
            }
        }
    }
}
