using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fyo;

namespace InputTestApp {
    /// <summary>
    /// Demonstrates basic SocketGamepad functionality
    /// </summary>
    public class InputTestApplication : FyoApplication {
        public GameObject PlayerPrefab;

        public List<Transform> PlayerStart = new List<Transform>();

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
                Destroy(Player);
            }
        }

        protected override void OnHandshake(AppHandshakeMsg handshakeMsg) {
        }

        protected override void OnUpdateGamepad(SocketGamepad gamepad) {
        }

    }
}
