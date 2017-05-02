using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fyo;
using System;

namespace PaddleBricks {
    public class PaddleBricksGame : FyoApplication {
        public GameObject PlayerPrefab;
        public List<Transform> PlayerStart = new List<Transform>();
        public List<Brick> Bricks = new List<Brick>();

        public GameObject BrickPrefab;

        void GenerateBrickField(int Density, int ColumnWidth) {
            if (ColumnWidth < 1) {
                ColumnWidth = 1;
                Debug.LogError("Brick Field Column Width was < 1");
            }

            foreach (Brick b in Bricks) {
                DestroyImmediate(b.gameObject);
            }

            if (BrickPrefab == null) {
                Debug.Log("Brick Prefab not set");
            }

            Vector3 BrickPosition = Vector3.zero;

            int BrickWidth = 64, BrickHeight = 32;
            Vector2 CenterOffset = new Vector2(-((BrickWidth * (ColumnWidth * 0.5f))), -(BrickHeight * (Density / )));

            int BrickRow, BrickColumn;
            GameObject BrickObj;
            Brick brick;
            RectTransform rt;
            for (int b = 0; b < Density; b++) {
                //Calculate Row
                BrickRow = b % ColumnWidth;
                //Calculate Column
                BrickColumn = b / ColumnWidth;

                BrickPosition = CenterOffset + new Vector2(BrickWidth * BrickColumn, BrickHeight * BrickRow);
                BrickObj = Instantiate(BrickPrefab, transform);
                brick = BrickObj.GetComponent<Brick>();
                if (brick != null) {
                    rt = BrickObj.GetComponent<RectTransform>();
                    rt.localPosition = BrickPosition;
                    Bricks.Add(brick);
                } else {
                    Debug.LogError("Brick Prefab does not contain a Brick Component!");
                }
            }
        }

        protected override void OnStart() {
            GenerateBrickField(40, 8);
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
                Paddle Player = PlayerObj.GetComponent<Paddle>();
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
                Destroy(Player);
            }
        }

        protected override void OnHandshake(AppHandshakeMsg handshakeMsg) {
        }

        protected override void OnUpdateGamepad(SocketGamepad gamepad) {
        }
        
    }
}

