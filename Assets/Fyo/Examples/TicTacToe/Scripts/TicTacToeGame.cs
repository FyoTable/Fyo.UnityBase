using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.IO;
using System.Linq;
using System;
using Fyo;

namespace ExampleTicTacToe {
    public class TicTacToeGame : FyoApplication {
        public GameObject GridObject;
        public List<TicTacToeCell> Grid = new List<TicTacToeCell>();
        public GameObject XPlayerTile;
        TicTacToePlayer XPlayer;
        public GameObject OPlayerTile;
        TicTacToePlayer OPlayer;
        public int PlayerIconOffset = 200;
        public long InputWaitMs = 1000;
        public GameObject XWinsPlaque;
        public GameObject OWinsPlaque;
        public GameObject DrawPlaque;

        public enum CurrentModeType {
            WaitForPlayers,
            WaitForReady,
            Playing
        }
        public CurrentModeType CurrentMode = CurrentModeType.WaitForPlayers;

        int PlayerTurn = 1;
        int Winner = -1;

        public TicTacToeGame() : base() {
            MaxPlayers = 2;
        }

        protected override void OnStart() {
            if (XPlayerTile == null)
                Debug.LogError("X Player Tile GameObject is null!");
            else
                XPlayer = XPlayerTile.GetComponent<TicTacToePlayer>();

            if (OPlayerTile == null)
                Debug.LogError("O Player Tile GameObject is null!");
            else
                OPlayer = OPlayerTile.GetComponent<TicTacToePlayer>();

            LocalPlayers.Add(XPlayer);
            LocalPlayers.Add(OPlayer);

            Reset();
        }

        protected override void AssignExtraHandlers() {
        }

        public int CheckWinner() {
            if (//Horizontal
                (Grid[0].X.activeSelf && Grid[1].X.activeSelf && Grid[2].X.activeSelf) ||
                (Grid[3].X.activeSelf && Grid[4].X.activeSelf && Grid[5].X.activeSelf) ||
                (Grid[6].X.activeSelf && Grid[7].X.activeSelf && Grid[8].X.activeSelf) ||
                //Vertical
                (Grid[0].X.activeSelf && Grid[3].X.activeSelf && Grid[6].X.activeSelf) ||
                (Grid[1].X.activeSelf && Grid[4].X.activeSelf && Grid[7].X.activeSelf) ||
                (Grid[2].X.activeSelf && Grid[5].X.activeSelf && Grid[8].X.activeSelf) ||
                //Diagonal
                (Grid[0].X.activeSelf && Grid[4].X.activeSelf && Grid[8].X.activeSelf) ||
                (Grid[2].X.activeSelf && Grid[4].X.activeSelf && Grid[6].X.activeSelf)
                ) {
                //X Win
                return 1;
            }

            if (//Horizontal
                (Grid[0].O.activeSelf && Grid[1].O.activeSelf && Grid[2].O.activeSelf) ||
                (Grid[3].O.activeSelf && Grid[4].O.activeSelf && Grid[5].O.activeSelf) ||
                (Grid[6].O.activeSelf && Grid[7].O.activeSelf && Grid[8].O.activeSelf) ||
                //Vertical
                (Grid[0].O.activeSelf && Grid[3].O.activeSelf && Grid[6].O.activeSelf) ||
                (Grid[1].O.activeSelf && Grid[4].O.activeSelf && Grid[7].O.activeSelf) ||
                (Grid[2].O.activeSelf && Grid[5].O.activeSelf && Grid[8].O.activeSelf) ||
                //Diagonal
                (Grid[0].O.activeSelf && Grid[4].O.activeSelf && Grid[8].O.activeSelf) ||
                (Grid[2].O.activeSelf && Grid[4].O.activeSelf && Grid[6].O.activeSelf)
                ) {
                //O Win
                return 2;
            }

            //NO WINNER
            for (int c = 0; c < Grid.Count; c++) {
                //Check if any cells are empty
                if (!Grid[c].X.activeSelf && !Grid[c].O.activeSelf) {
                    return 0;
                }
            }
        
            //DRAW
            return 3;
        }

        public void Reset() {
            Winner = 0;
            Grid.Clear();
            TicTacToeCell cell;
            for (int c = 0; c < GridObject.transform.childCount; c++) {
                Grid.Add(cell = GridObject.transform.GetChild(c).GetComponent<TicTacToeCell>());
                cell.X.SetActive(false);
                cell.O.SetActive(false);
            }
            foreach (TicTacToePlayer player in LocalPlayers) {
                if(player != null)
                    player.Ready = false;
            }
            if (XWinsPlaque != null)
                XWinsPlaque.SetActive(false);
            if (OWinsPlaque != null)
                OWinsPlaque.SetActive(false);
            if (DrawPlaque != null)
                DrawPlaque.SetActive(false);
        }

        protected override void OnConnected() {
        }

        protected override void OnHandshake(AppHandshakeMsg handshakeMsg) {
        }

        protected override void OnDisconnected() {
            Reset();
        }

        protected override void OnGamepadPluggedIn(SocketGamepad gamepad) {

            if (XPlayer.Gamepad == null) {
                XPlayer.Xs = true;
                XPlayer.FyoApp = this;
                XPlayer.Gamepad = gamepad;
                XPlayer.PlayerIcon.SetActive(true);
                ActiveGamepads.Add(gamepad, XPlayer);
                XPlayer.InputWait = DateTime.Now.Ticks + (InputWaitMs * TimeSpan.TicksPerMillisecond);
                Debug.Log("X Connected");
            } else if (OPlayer.Gamepad == null) {
                OPlayer.Xs = false;
                OPlayer.FyoApp = this;
                OPlayer.Gamepad = gamepad;
                OPlayer.PlayerIcon.SetActive(true);
                Debug.Log("O Connected");
                ActiveGamepads.Add(gamepad, OPlayer);
                OPlayer.InputWait = DateTime.Now.Ticks + (InputWaitMs * TimeSpan.TicksPerMillisecond);
            } else {
                //Reconnected?
                if (CurrentMode == CurrentModeType.Playing && (XPlayer.Gamepad == gamepad || OPlayer.Gamepad == gamepad)) {
                    SendGameState(gamepad);
                }       
            }

            if (XPlayer.Gamepad != null && OPlayer.Gamepad != null && CurrentMode == CurrentModeType.WaitForPlayers) {
                CurrentMode = CurrentModeType.WaitForReady;
                Debug.Log("Waiting for Players to Ready");
            }
        }

        protected override void OnUpdateGamepad(SocketGamepad gamepad) {
            switch (CurrentMode) {
                case CurrentModeType.WaitForPlayers:
                    break;
                case CurrentModeType.WaitForReady:
                    //Check if player is present
                    if (ActiveGamepads.ContainsKey(gamepad)) {
                        if (DateTime.Now.Ticks > ActiveGamepads[gamepad].InputWait) {
                            if (!ActiveGamepads[gamepad].Ready) {
                                ActiveGamepads[gamepad].Ready = true;
                            }
                        }
                    }

                    //Check if all players have readied
                    if (ActiveGamepads.Values.Count(p => p.Ready) >= 2) {
                        StartGame();
                        ActiveGamepads[gamepad].InputWait = DateTime.Now.Ticks + (InputWaitMs * TimeSpan.TicksPerMillisecond);
                    }
                    break;

                case CurrentModeType.Playing:
                    if (ActiveGamepads.ContainsKey(gamepad)) {
                        TicTacToePlayer player = (TicTacToePlayer)ActiveGamepads[gamepad];
                        if (player.isMyTurn) {
                            for (int b = 0; b < 9; b++) {
                                if (gamepad.GetButton("button " + b.ToString()) && Grid[b].CurrentMark == 0) {
                                    ActiveGamepads[gamepad].InputWait = DateTime.Now.Ticks + (InputWaitMs * TimeSpan.TicksPerMillisecond);
                                    SetCell(b, player.Xs ? 1 : 2);
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }
        }

        protected override void OnGamepadTimingOut(SocketGamepad gamepad) {
        }

        protected override void OnGamepadUnplugged(SocketGamepad gamepad) {
            if (ActiveGamepads.ContainsKey(gamepad)) {
                //Remove from players
                TicTacToePlayer player = (TicTacToePlayer)ActiveGamepads[gamepad];
                player.PlayerIcon.SetActive(false);
                ActiveGamepads.Remove(gamepad);
                LocalPlayers.Remove(player);
                player.Gamepad = null;

                if (XPlayer.Gamepad == null && OPlayer.Gamepad == null) {
                    CurrentMode = CurrentModeType.WaitForPlayers;
                }
            }
        }

        protected override void OnGamepadReconnect(SocketGamepad gamepad) {
            
        }

        protected void StartGame() {
            Debug.Log("[Tic Tac Toe] Starting Game");
            Reset();
            if (UnityEngine.Random.Range(0, 2) > 0) {
                XPlayer.isMyTurn = true;
                OPlayer.isMyTurn = false;
                PlayerTurn = 1;
            } else {
                OPlayer.isMyTurn = true;
                XPlayer.isMyTurn = false;
                PlayerTurn = 2;
            }

            CurrentMode = CurrentModeType.Playing;
            SendGameStart();
            SendMyTurn();
        }

        protected void TriggerEndgame() {
            XPlayer.Ready = false;
            OPlayer.Ready = false;

            switch (Winner) {
                case 1:
                    //TODO: Display Text
                    Debug.Log("[Tic Tac Toe] X Won");
                    XPlayer.Wins++;

                    if (XWinsPlaque != null)
                        XWinsPlaque.SetActive(true);
                    break;
                case 2:
                    //TODO: Display Text
                    Debug.Log("[Tic Tac Toe] O Won");
                    OPlayer.Wins++;
                    if (OWinsPlaque != null)
                        OWinsPlaque.SetActive(true);
                    break;
                case 3:
                    //TODO: Display Text
                    Debug.Log("[Tic Tac Toe] Draw");
                    XPlayer.Draws++;
                    OPlayer.Draws++;
                    if (DrawPlaque != null)
                        DrawPlaque.SetActive(true);
                    break;
            }

            CurrentMode = CurrentModeType.WaitForReady;
            Debug.Log("Waiting for Players to Ready");
            SendGameEnd();
        }

        protected void SendCellUpdate(int cellid, int mark) {
            SGUpdateMsg SetCellOnController = new SGUpdateMsg();

            JSONObject CellState = new JSONObject();
            CellState.AddField("cell", cellid);
            CellState.AddField("state", mark);

            SetCellOnController.SGID = -2;
            SetCellOnController.MessageType = "cell";
            SetCellOnController.Data = CellState;
            SetCellOnController.Serialize();

            socket.Emit("SGUpdateMsg", SetCellOnController);
        }

        protected void SendGameStart() {
            SGUpdateMsg StartGameMsg = new SGUpdateMsg();

            StartGameMsg.SGID = XPlayer.Gamepad.SGID;
            StartGameMsg.MessageType = "start";
            StartGameMsg.Data = new JSONObject();

            StartGameMsg.Data.AddField("mark", 1);
            StartGameMsg.Serialize();

            Debug.Log("Sending 'start': " + StartGameMsg.ToString());
            socket.Emit("SGUpdateMsg", StartGameMsg);

            StartGameMsg.SGID = OPlayer.Gamepad.SGID;
            StartGameMsg.Data.SetField("mark", 2);
            StartGameMsg.Serialize();
            Debug.Log("Sending 'start': " + StartGameMsg.ToString());
            socket.Emit("SGUpdateMsg", StartGameMsg);
        }

        protected void SendMyTurn() {
            SGUpdateMsg MyTurnMsg = new SGUpdateMsg();

            MyTurnMsg.SGID = PlayerTurn == 1 ? XPlayer.Gamepad.SGID : OPlayer.Gamepad.SGID;
            MyTurnMsg.MessageType = "turn";
            MyTurnMsg.Data = new JSONObject();
            MyTurnMsg.Serialize();

            Debug.Log("Sending 'turn': " + MyTurnMsg.ToString());
            socket.Emit("SGUpdateMsg", MyTurnMsg);
        }

        protected void SendGameState(SocketGamepad gamepad) {
            SGUpdateMsg GameStateMsg = new SGUpdateMsg();
            TicTacToePlayer tttPlayer = (TicTacToePlayer)ActiveGamepads[gamepad];
            GameStateMsg.SGID = tttPlayer.PlayerId;
            GameStateMsg.MessageType = "gamestate";
            GameStateMsg.Data = new JSONObject();
            JSONObject CellJSON = new JSONObject(JSONObject.Type.ARRAY);

            GameStateMsg.Data.AddField("mark", tttPlayer.Xs ? 1 : 2);
            for (int m = 0; m < 9; m++) {
                CellJSON.list.Add(new JSONObject(Grid[m].CurrentMark));
            }
            GameStateMsg.Data.AddField("cells", CellJSON);
            GameStateMsg.Serialize();

            Debug.Log("Sending Game State: " + GameStateMsg.ToString());

            socket.Emit("SGUpdateMsg", GameStateMsg);
        }

        protected void SendGameEnd() {
            SGUpdateMsg ResetGameMsg = new SGUpdateMsg();

            ResetGameMsg.SGID = -1;
            ResetGameMsg.MessageType = "finish";
            ResetGameMsg.Data = new JSONObject();
            ResetGameMsg.Data.AddField("winner", Winner > 2 ? 0 : Winner);
            ResetGameMsg.Serialize();

            Debug.Log("Sending 'finish': " + ResetGameMsg.ToString());

            socket.Emit("SGUpdateMsg", ResetGameMsg);
        }

        public void SetCell(int cellid, int mark) {
    #if UNITY_EDITOR
            if (mark == 0) {
                Debug.Log("[Tic Tac Toe] Marking Cell " + cellid.ToString() + " clear");
            } else if (mark == 1) {
                Debug.Log("[Tic Tac Toe] Marking Cell " + cellid.ToString() + " for X");
            } else if (mark == 2) {
                Debug.Log("[Tic Tac Toe] Marking Cell " + cellid.ToString() + " for O");
            }
    #endif

            if (mark == PlayerTurn) {
                if (mark == 0) {
                    Grid[cellid].X.SetActive(false);
                    Grid[cellid].O.SetActive(false);
                } else if (mark == 1) { //X
                    Grid[cellid].X.SetActive(true);
                    Grid[cellid].O.SetActive(false);
                } else if (mark == 2) { // O
                    Grid[cellid].X.SetActive(false);
                    Grid[cellid].O.SetActive(true);
                }
            
                if (PlayerTurn == 1) {
                    XPlayer.isMyTurn = false;
                    PlayerTurn = 2;
                    OPlayer.isMyTurn = true;
                } else if (PlayerTurn == 2) {
                    OPlayer.isMyTurn = false;
                    PlayerTurn = 1;
                    XPlayer.isMyTurn = true;
                }

                SendCellUpdate(cellid, mark);

                if ((Winner = CheckWinner()) > 0) {
                    //Winner winner, chicken dinner!
                    TriggerEndgame();
                } else {
                    SendMyTurn();
                }
            }
        }

    }
}
