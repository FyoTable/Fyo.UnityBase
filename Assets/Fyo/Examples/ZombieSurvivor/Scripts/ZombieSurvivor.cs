using System;
using System.Collections;
using System.Collections.Generic;
using Fyo;
using UnityEngine;

namespace ZombieSurvivor {
    public class ZombieSurvivor : Fyo.FyoApplication {
        public List<Fyo.ObjectPool> PlayerSpawn = new List<Fyo.ObjectPool>();
        public int NextSpawn = 0;

        public CameraFraming Framing;

        public enum GameState {
            None,
            CharSelect,
            WaveReady,
            Wave,
            Countdown
        }
        GameState CurrentGameState = GameState.None;

        protected override void AssignExtraHandlers() {
        }

        protected override void OnConnected() {
        }

        protected override void OnDisconnected() {
        }

        protected override void OnGamepadPluggedIn(SocketGamepad gamepad) {
            //Temporary until Acquire Caleb's menu system to select player

            if(PlayerSpawn.Count > 0) {
                //Debug.Log("Next Player: " + NextSpawn.ToString());
                if(NextSpawn < PlayerSpawn.Count) {
                    GameObject PlayerObj = PlayerSpawn[NextSpawn].Spawn(PlayerSpawn[NextSpawn].transform.position, PlayerSpawn[NextSpawn].transform.rotation);

                    SurvivorPlayer Player = PlayerObj.AddComponent<SurvivorPlayer>();
                    Player.Gamepad = gamepad;
                    Player.FyoApp = this;
                    Player.PlayerId = LocalPlayers.Count;

                    LocalPlayers.Add(Player);
                    ActiveGamepads.Add(gamepad, Player);

                    Framing.TrackedObjects.Add(Player.gameObject);
                }

                //Advance Spawn Point
                if(++NextSpawn >= PlayerSpawn.Count) {
                    NextSpawn = 0;
                }

            } else {
                Debug.LogError("Negative Spawn Index");
            }
        }

        protected override void OnGamepadReconnect(SocketGamepad gamepad) {
            //TODO: Cancel Timing Out
        }

        protected override void OnGamepadTimingOut(SocketGamepad gamepad) {
            //TODO: Show Timing Out
        }

        protected override void OnGamepadUnplugged(SocketGamepad gamepad) {
            SurvivorPlayer Player = (SurvivorPlayer)ActiveGamepads[gamepad];

            LocalPlayers.Remove(Player);

            Framing.TrackedObjects.Remove(Player.gameObject);
            ActiveGamepads.Remove(gamepad);
        }

        protected override void OnHandshake(AppHandshakeMsg handshakeMsg) {
        }

        protected override void OnUpdateGamepad(SocketGamepad gamepad) {
        }
    }
}
