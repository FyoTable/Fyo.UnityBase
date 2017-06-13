using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fyo;
using System;

namespace InputTestApp {
    public class LocalInputTestApplication : FyoApplication {
        public GameObject PlayerPrefab;
        public List<Transform> PlayerStart = new List<Transform>();

        string[] Joysticks;

        protected override void OnStart() {        

        }

        protected override void AssignExtraHandlers() {
        }

        protected override void OnConnected() {
        }

        protected override void OnDisconnected() {
        }

        protected override void OnGamepadPluggedIn(SocketGamepad gamepad) {
            if (!ActiveGamepads.ContainsKey(gamepad)) {
                GameObject PlayerObj = Instantiate(PlayerPrefab);
                InputTestPlayer Player = PlayerObj.GetComponent<InputTestPlayer>();
                Player.FyoApp = this;
                Player.Gamepad = gamepad;
                Player.PlayerId = ActiveGamepads.Count;
                ActiveGamepads.Add(gamepad, Player);

                if (Player.PlayerId < PlayerStart.Count) {
                    Transform t = PlayerStart[Player.PlayerId];
                    Player.transform.position = t.position;
                    Player.transform.rotation = t.rotation;
                    Player.transform.localScale = t.localScale;
                }
            }
        }

        protected override void OnGamepadUnplugged(SocketGamepad gamepad) {
            if (ActiveGamepads.ContainsKey(gamepad)) {
                FyoPlayer Player = ActiveGamepads[gamepad];
                ActiveGamepads.Remove(gamepad);
                Destroy(Player.gameObject);
            }
        }

        protected override void OnGamepadReconnect(SocketGamepad gamepad) {
            Debug.Log("Reconnected " + gamepad.SGID);
        }

        protected override void OnGamepadTimingOut(SocketGamepad gamepad) {
            Debug.Log("Timing out " + gamepad.SGID);
        }
        
        protected override void OnHandshake(AppHandshakeMsg handshakeMsg) {
        }

        protected override void OnUpdateGamepad(SocketGamepad gamepad) {
        }

        long UpdateRate = 1000;
        System.DateTime NextUpdate = System.DateTime.Now;
        private void Update() {
            if (System.DateTime.Now >= NextUpdate) {
                NextUpdate.AddTicks(System.TimeSpan.TicksPerMillisecond * UpdateRate);

                Joysticks = Input.GetJoystickNames();
                if (Joysticks.Length > 0) {
                    for (int j = 0; j < Joysticks.Length; j++) {
                    }
                }
            }
        }
    }
}