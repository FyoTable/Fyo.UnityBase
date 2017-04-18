using System;
using System.Collections;
using System.Collections.Generic;
using SurvivalShooterExampleGame;
using UnityEngine;

namespace Fyo {
    public class ObjectPool : MonoBehaviour {
        public GameObject Template;
        public int Count = 1;
        public bool TruncateExistingObjects = true;
        public bool AutomaticallyResize = false;
        public int ResizeBlockSize = 1;
        protected GameObject[] Entries;
        protected int EntryPointer = 0;
        protected GameObject t;

        private void Resize(int NewSize) {
            if (Entries.Length > NewSize) {
                //Reduce Size
                GameObject[] e = Entries;
                Entries = new GameObject[NewSize];
                for (int i = 0; i < Entries.Length; i++) {
                    Entries[i] = e[i]; //eio
                }
            } else if(Entries.Length < NewSize) {
                //Increase Size
                GameObject[] e = Entries;
                Entries = new GameObject[NewSize];
                for (int i = 0; i < e.Length; i++) {
                    Entries[i] = e[i]; //eio
                }
            }
        }

        private void Start() {
            Entries = new GameObject[Count];
            for (EntryPointer = 0; EntryPointer < Count; EntryPointer++) {
                t = Entries[EntryPointer] = Instantiate(Template);
                t.SetActive(false);
            }
            t = null;
            EntryPointer = 0;
        }

        protected int NextSpawnIdx {
            get {
                if (Entries.Length > 0) {
                    for (int i = 0; i < Entries.Length; i++) {
                        if (!Entries[i].activeSelf)
                            return i;
                    }
                }
                return -1;
            }
        }

        public GameObject GetNext(Vector3 Position, Quaternion Rotation, Vector3 Scale) {
            EntryPointer = NextSpawnIdx;
            if (EntryPointer < 0) {
                if (AutomaticallyResize) {
                    Resize(Entries.Length + ResizeBlockSize);
                    EntryPointer = NextSpawnIdx;
                }

                if (EntryPointer < 0) {
                    if (TruncateExistingObjects) {
                        EntryPointer = 0;
                    } else {
                        return null;
                    }
                }
            }

            t = Entries[EntryPointer];

            if (t.activeSelf) {
                Debug.Log("[Pool \"" + name + "\"] Pooled Object \"" + t.name + "\" is already active, consider increasing pool size or turning on autoresize.");
                t.SetActive(false);
            }

            t.transform.position = Position;
            t.transform.rotation = Rotation;
            t.transform.localScale = Scale;
            t.SetActive(true);

            return t;
        }

        public GameObject Spawn(Vector3 position) {
            return Spawn(position, Quaternion.identity);
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation) {
            return Spawn(position, rotation, Vector3.one);
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation, Vector3 scale) {
            GameObject PooledObject = null;
            if (Template != null && Entries.Length > 0) {
                PooledObject = GetNext(position, rotation, scale);
            }

            //Steal object
            if (PooledObject != null) {
                if (PooledObject.activeSelf) {
                    PooledObject.SetActive(false);
                }

                PooledObject.SetActive(true);
            }
            return PooledObject;
        }
    }
}
