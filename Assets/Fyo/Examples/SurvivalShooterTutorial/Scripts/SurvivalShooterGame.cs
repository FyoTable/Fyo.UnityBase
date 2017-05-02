using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fyo;

namespace SurvivalShooterExampleGame {
    public class SurvivalShooterGame : FyoApplication {
        public ObjectPool PlayerPool = null;
        public GameObject PlayerSpawnArea = null;
        public CameraFraming CamFrame;
        public List<GameObject> MonsterSpawners = new List<GameObject>();

        Dictionary<SocketGamepad, PlayerMovement> Players = new Dictionary<SocketGamepad, PlayerMovement>();

        protected override void AssignExtraHandlers() {
        }

        protected override void OnConnected() {
            CamFrame = FindObjectOfType<CameraFraming>();
        }

        protected override void OnDisconnected() {
        }

        protected override void OnGamepadPluggedIn(SocketGamepad gamepad) {
            if (ActiveGamepads.Count == 0) {
                //First Gamepad
                if (MonsterSpawners.Count > 0) {
                    for (int m = 0; m < MonsterSpawners.Count; m++) {
                        MonsterSpawners[m].gameObject.SetActive(true);
                    }
                }
            }

            if (!ActiveGamepads.ContainsKey(gamepad)) {
                PlayerMovement Player = SpawnPlayer(gamepad);
                if (Player != null) {
                    ActiveGamepads.Add(gamepad, Player);
                    //Delay input tracking for 1000 ms
                    Player.InputWait = DateTime.Now.Ticks + (5000 * TimeSpan.TicksPerMillisecond);
                    CamFrame.TrackedObjects.Add(Player.gameObject);
                    Debug.Log("Added Player to Framing " + Player.name);
                }
            } else {
                //TODO: Send state message to controller
            }
        }

        protected override void OnGamepadUnplugged(SocketGamepad gamepad) {
            if (ActiveGamepads.ContainsKey(gamepad)) {
                PlayerMovement p = (PlayerMovement)ActiveGamepads[gamepad];
                ActiveGamepads.Remove(gamepad);
                CamFrame.TrackedObjects.Remove(p.gameObject);
                p.gameObject.SetActive(false);
            }

            if (ActiveGamepads.Count == 0) {
                //Last Gamepad
                if (MonsterSpawners.Count > 0) {
                    for (int m = 0; m < MonsterSpawners.Count; m++) {
                        MonsterSpawners[m].gameObject.SetActive(false);
                    }

                    EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
                    for (int e = 0; e < enemies.Length; e++) {
                        enemies[e].currentHealth = enemies[e].startingHealth;
                        enemies[e].gameObject.SetActive(false);
                    }
                }
            }
        }

        protected override void OnHandshake(AppHandshakeMsg handshakeMsg) {
        }

        protected override void OnUpdateGamepad(SocketGamepad gamepad) {
            if (ActiveGamepads.ContainsKey(gamepad)) {
                PlayerMovement Player = (PlayerMovement)ActiveGamepads[gamepad];
                if (Player != null) {

                }
            }
        }

        protected PlayerMovement SpawnPlayer(SocketGamepad gamepad) {
            GameObject pObj = null;
            PlayerMovement p = null;

            if (PlayerPool != null) {
                if (PlayerSpawnArea != null) {
                    SphereCollider sphere = PlayerSpawnArea.GetComponent<SphereCollider>();
                    if (sphere != null) {
                        float Angle = UnityEngine.Random.Range(0.0f, 180.0f);
                        float Distance = sphere.radius * UnityEngine.Random.Range(-1.0f, 1.0f);
                        
                        Vector3 pos = PlayerSpawnArea.transform.position + (PlayerSpawnArea.transform.rotation * Quaternion.AngleAxis(Angle, PlayerSpawnArea.transform.up)) * (Vector3.forward * Distance);
                        pObj = PlayerPool.Spawn(pos, PlayerSpawnArea.transform.rotation, PlayerSpawnArea.transform.localScale);
                    } else {
                        pObj = PlayerPool.Spawn(PlayerSpawnArea.transform.position, PlayerSpawnArea.transform.rotation, PlayerSpawnArea.transform.localScale);
                    }
                } else {
                    pObj = PlayerPool.Spawn(transform.position);
                }

                if (pObj != null) {
                    if ((p = pObj.GetComponent<PlayerMovement>()) != null) {
                        p.FyoApp = this;
                        p.Gamepad = gamepad;
                    }
                } else {
                    Debug.LogError("Bad Object returned from Player Pool");
                }
            } else {
                Debug.LogError("Missing Player ObjectPool");
            }

            return p;
        }
    }
}