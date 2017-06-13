using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    namespace AI {
        [RequireComponent(typeof(SphereCollider))]
        public class Sight : MonoBehaviour {
            public List<string> VisibleTags = new List<string>();
            public List<GameObject> Seen = new List<GameObject>();

            protected SphereCollider VisionSphere;
            public Vector2 VisionFOV = new Vector2(45.0f, 10.0f);

            private void Start() {
                VisionSphere = GetComponent<SphereCollider>();

                if (VisionSphere.isTrigger != true) {
                    Debug.LogWarning("VisionSphere should be a trigger, fixing.");
                    VisionSphere.isTrigger = true;
                }
            }

            private void OnTriggerEnter(Collider other) {
                if(VisibleTags.Count > 0) {
                    for(int t = 0; t < VisibleTags.Count; t++) {
                        if(other.CompareTag(VisibleTags[t])) {
                            if(!Seen.Contains(other.gameObject)) {
                                Seen.Add(other.gameObject);
                            }
                        }
                    }
                } else {
                    Debug.LogWarning("Eye with no visible tags, is this intentional?");
                }
            }

            private void OnTriggerExit(Collider other) {
                if(Seen.Contains(other.gameObject)) {
                    Seen.Remove(other.gameObject);
                }
            }
        }
    }
}

