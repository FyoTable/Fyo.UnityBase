using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fyo;
using System;

namespace PaddleBricks {
    public class PaddleBricksGame : FyoApplication {
        public GameObject PlayerPrefab;
        public List<RectTransform> PlayerStart = new List<RectTransform>();
        public List<Brick> Bricks = new List<Brick>();

        public GameObject BrickPrefab;

        public bool PongMode = false;

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

            int Rows = Density / ColumnWidth;

            int BrickWidth = 64, BrickHeight = 32;
            Vector2 CenterOffset = new Vector2(
                -(BrickWidth * ColumnWidth) * 0.5f,
                -(BrickHeight * Rows) * 0.5f
            );

            int BrickRow, BrickColumn;
            GameObject BrickObj;
            Brick brick;
            RectTransform rt;
            for (int b = 0; b < Density; b++) {
                //Calculate Row
                BrickRow = b % ColumnWidth;
                //Calculate Column
                BrickColumn = b / ColumnWidth;

                BrickPosition = CenterOffset + new Vector2(BrickWidth * BrickRow + (0.5f * BrickWidth), BrickHeight * BrickColumn + (0.5f * BrickHeight));
                BrickObj = Instantiate(BrickPrefab, transform);
                brick = BrickObj.GetComponent<Brick>();
                if (brick != null) {
                    rt = brick.GetComponent<RectTransform>();
                    //Base + Percentage + Shift
                    brick.color = new Color(
                        0.25f + (Mathf.Abs(BrickColumn / (float)ColumnWidth)) * 2.0f, 
                        0,
                        (Mathf.Abs(BrickRow / (float)Rows)),
                        1);
                    rt.transform.localPosition = BrickPosition;
                    Bricks.Add(brick);
                } else {
                    Debug.LogError("Brick Prefab does not contain a Brick Component!");
                }
            }
        }

        protected override void OnStart() {
            if(!PongMode)
                GenerateBrickField(200, 20);
        }

        protected override void AssignExtraHandlers() {
        }

        protected override void OnConnected() {
        }

        protected override void OnDisconnected() {
        }

        protected override void OnGamepadPluggedIn(SocketGamepad gamepad) {
            if (!ActiveGamepads.ContainsKey(gamepad)) {
                GameObject PlayerObj = Instantiate(PlayerPrefab, transform);
                Paddle Player = PlayerObj.GetComponent<Paddle>();
                Player.FyoApp = this;
                Player.Gamepad = gamepad;
                Player.PlayerId = ActiveGamepads.Count;
                ActiveGamepads.Add(gamepad, Player);

                if (Player.PlayerId < PlayerStart.Count) {
                    RectTransform t = PlayerStart[Player.PlayerId];
                    RectTransform pt = Player.GetComponent<RectTransform>();

                    //Set Player Color
                    UnityEngine.UI.Image PlayerStartImg = PlayerStart[Player.PlayerId].GetComponent<UnityEngine.UI.Image>();
                    UnityEngine.UI.Image PlayerImg = pt.GetComponent<UnityEngine.UI.Image>();
                    PlayerImg.color = PlayerStartImg.color;

                    pt.position = t.position;
                    pt.rotation = t.rotation;
                    pt.localScale = t.localScale;
                    pt.SetParent(transform);

                    SGUpdateMsg ControllerUpdate = new SGUpdateMsg(gamepad);
                    ControllerUpdate.Data = new JSONObject();
                    ControllerUpdate.Data.AddField("MessageType", JSONObject.CreateStringObject("PlayerColor"));
                    ControllerUpdate.Data.AddField("Color", JSONObject.CreateStringObject(PlayerImg.color.ToString()));

                    PlayerStart[Player.PlayerId].gameObject.SetActive(false);
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
        }

        protected override void OnHandshake(AppHandshakeMsg handshakeMsg) {
        }

        protected override void OnUpdateGamepad(SocketGamepad gamepad) {
        }
        
        protected override void OnGamepadTimingOut(SocketGamepad gamepad) {
            gamepad.InputData.Clear();
        }
    }
}

