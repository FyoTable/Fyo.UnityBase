using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fyo {
    public class CameraFraming : MonoBehaviour {
        public List<GameObject> TrackedObjects = new List<GameObject>();
        public bool AlignWithWorldCenterOfObjects = true;
        public bool ZoomToFit = true;
        public float ZoomFactor = 5.0f;
        public float MinimumZoomHeight = 6.0f;
        public float MaximumZoomHeight = 10.0f;
        public float MinimumOrthoSize = 6.0f;


        Vector3 WorldCenterOfObjects = Vector3.zero;
        Vector3 AxisAlignedMinimumBound = Vector3.zero;
        Vector3 AxisAlignedMaximumBound = Vector3.zero;
        
        void Update() {
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

            WorldCenterOfObjects = AxisAlignedMinimumBound + ((AxisAlignedMaximumBound - AxisAlignedMinimumBound) * 0.5f);
            if (ZoomToFit) {
                Camera camera = GetComponent<Camera>();
                if (camera != null) {
                    float Width = (Mathf.Abs(AxisAlignedMaximumBound.x) - Mathf.Abs(AxisAlignedMinimumBound.x)) * 0.5f;
                    float Depth = (Mathf.Abs(AxisAlignedMaximumBound.z) - Mathf.Abs(AxisAlignedMinimumBound.z)) * 0.5f;

                    float Height = Mathf.Sqrt(Mathf.Pow(Width, 2) + Mathf.Pow(Depth, 2)) * ZoomFactor;

                    WorldCenterOfObjects.y = Mathf.Clamp(MinimumZoomHeight + Height, MinimumZoomHeight, MaximumZoomHeight);
                    //transform.Translate(Vector3.up * Height);
                    //camera.fieldOfView = ((Mathf.Max(0.0f, (Width / (float)Screen.width))) / (1.0f + (Depth / (float)Screen.height))) * 45.0f;
                }
            } else
                WorldCenterOfObjects.y = transform.position.y;

            transform.position = WorldCenterOfObjects;

            if (AlignWithWorldCenterOfObjects && TrackedObjects.Count > 0) {
                WorldCenterOfObjects.y = transform.position.y;
                transform.position = WorldCenterOfObjects;
            }


        }
    }
}
