using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    namespace AI {
        [RequireComponent(typeof(SphereCollider))]
        public class Touch : MonoBehaviour {
            public SphereCollider TouchSphere;
            public List<string> TouchTags = new List<string>();
            public List<GameObject> Touching = new List<GameObject>();

            private void Start() {
                TouchSphere = GetComponent<SphereCollider>();
                if(!TouchSphere.isTrigger) {
                    Debug.LogWarning("Touch Radius should always be a Trigger. Fixing.");
                    TouchSphere.isTrigger = true;
                }
            }

            private void OnTriggerEnter(Collider other) {
                if (TouchTags.Count > 0) {
                    string strTag;
                    for(int t = 0; t < TouchTags.Count; t++) {
                        strTag = TouchTags[t];
                        if (other.CompareTag(strTag) && !Touching.Contains(other.gameObject)) {
                            Touching.Add(other.gameObject);
                            return;
                        }
                    }
                }
            }

            private void OnTriggerStay(Collider other) {
                //During Fixed Update
            }

            private void OnTriggerExit(Collider other) {
                if (Touching.Contains(other.gameObject))
                    Touching.Remove(other.gameObject);
            }
        }
    }
}
