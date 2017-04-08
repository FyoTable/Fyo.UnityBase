using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.IO;
using System.Linq;
using System;

public class TicTacToeGame : FyoApplication {
    public GameObject GridObject;
    public List<TicTacToeCell> Grid = new List<TicTacToeCell>();
    public GameObject XPlayerTile;
    public GameObject OPlayerTile;

    int PlayerTurn = 1;
    int Winner = -1;

    public TicTacToeGame() : base() {
        MaxPlayers = 2;
    }


    protected override void OnStart() {
        if (XPlayerTile == null)
            Debug.LogError("X Player Tile GameObject is null!");
        if (OPlayerTile == null)
            Debug.LogError("O Player Tile GameObject is null!");
        Reset();
    }

    protected override void AssignExtraHandlers() {
    }

    /*
    protected override void HandleConnectedToSGServer(SocketIOEvent e) {
        string ControllerPath = Fyo.Paths.Controllers + "TicTacToe.zip";
        string strData = string.Empty;
        if (File.Exists(ControllerPath)) {
            byte[] data = File.ReadAllBytes(ControllerPath);
            strData = System.Convert.ToBase64String(data);
        } else {
            Debug.LogWarning(ControllerPath + " does not exist!");
        }

        AppHandshakeMsg GameInfoMsg = new AppHandshakeMsg(AppIdString, strData);

        //Identify App to Node Server
        Debug.Log("Handshake Accepted, sending Game Info: " + GameInfoMsg.ToString());
        socket.Emit("AppHandshakeMsg", GameInfoMsg);
    }
    */

    /*
    protected override void HandleGamepadHandshake(SocketIOEvent e) {
        SGHandshakeMsg handshakeMsg = new SGHandshakeMsg(e.data);
        SocketGamepad gamepad = null;

        if (XPlayerTile.GetComponent<TicTacToePlayer>() == null) {
            TicTacToePlayer XPlayer = XPlayerTile.AddComponent<TicTacToePlayer>();
            //TODO: Send back Update to load Xs controller?
            gamepad = CreateGamepad(handshakeMsg.PlayerId);
            XPlayer.Xs = true;
            XPlayer.Gamepad = gamepad;
            Debug.Log("X Connected");
        } else if (OPlayerTile.GetComponent<TicTacToePlayer>() == null) {
            TicTacToePlayer OPlayer = OPlayerTile.AddComponent<TicTacToePlayer>();
            //TODO: Send back Update to load Os controller?
            gamepad = CreateGamepad(handshakeMsg.PlayerId);
            OPlayer.Xs = false;
            OPlayer.Gamepad = gamepad;
            Debug.Log("O Connected");
        } else {
            //Extra Player
        }

        Debug.Log("[Tic Tac Toe] Gamepad Handshake: " + gamepad.PlayerId.ToString());
    }
    */
    /*
    protected override void HandleGamepadDisconnected(SocketIOEvent e) {
        int PlayerId = 0;
        e.data.GetField(ref PlayerId, "PlayerId");
        if (PlayerId > -1 && PlayerId < ActiveGamepads.Count) {
            SocketGamepad gamepad = GetGamepad(PlayerId);
            Debug.Log("[Tic Tac Toe] Removing Gamepad " + PlayerId + " and player.");

            if (LocalPlayers[0].Gamepad == gamepad) {
                ((TicTacToePlayer)LocalPlayers[0]).PlayerIcon.SetActive(false);
                LocalPlayers[0].Gamepad = null;
            }

            if (LocalPlayers[1].Gamepad == gamepad) {
                ((TicTacToePlayer)LocalPlayers[1]).PlayerIcon.SetActive(false);
                LocalPlayers[1].Gamepad = null;
            }

            ActiveGamepads.Remove(gamepad);
            OnRemoveGamepad(gamepad);
            Destroy(gamepad);

            RenumberActiveGamepads();
        }
    }
    */

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
            if (!Grid[c].X.activeSelf || !Grid[c].O.activeSelf) {
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
    }

    protected override void OnConnected() {
    }

    protected override void OnHandshake(AppHandshakeMsg handshakeMsg) {
    }

    protected override void OnDisconnected() {
    }

    protected override void OnGamepadPluggedIn(SocketGamepad gamepad) {
        if (XPlayerTile.GetComponent<TicTacToePlayer>() == null) {
            TicTacToePlayer XPlayer = XPlayerTile.AddComponent<TicTacToePlayer>();
            //TODO: Send back Update to load Xs controller?
            XPlayer.Xs = true;
            XPlayer.Gamepad = gamepad;
            Debug.Log("X Connected");
        } else if (OPlayerTile.GetComponent<TicTacToePlayer>() == null) {
            TicTacToePlayer OPlayer = OPlayerTile.AddComponent<TicTacToePlayer>();
            //TODO: Send back Update to load Os controller?
            OPlayer.Xs = false;
            OPlayer.Gamepad = gamepad;
            Debug.Log("O Connected");
        } else {
            //Extra Player
        }
    }

    public enum CurrentModeType {
        WaitForPlayers,
        WaitForReady,
        Playing
    }
    public CurrentModeType CurrentMode = CurrentModeType.WaitForPlayers;
    protected override void OnUpdateGamepad(SocketGamepad gamepad) {
        switch (CurrentMode) {
            case CurrentModeType.WaitForPlayers:
                break;
            case CurrentModeType.WaitForReady:
                //Check if player is present
                if (!ActiveGamepads.ContainsKey(gamepad))
                    if (!ActiveGamepads[gamepad].Ready)
                        ActiveGamepads[gamepad].Ready = true;

                //Check if all players have readied
                if (ActiveGamepads.Values.Count(p => p.Ready) >= 2) {
                    StartGame();
                    OnUpdateGamepad(gamepad);
                }
                break;

            case CurrentModeType.Playing:
                //Gamepad disappeared?
                if (ActiveGamepads.Count < 2) {
                    Debug.LogError("Gamepad disappeared");
                    CurrentMode = CurrentModeType.WaitForPlayers;
                    OnUpdateGamepad(gamepad);
                    break;
                }

                if (ActiveGamepads.ContainsKey(gamepad)) {
                    TicTacToePlayer player = (TicTacToePlayer)ActiveGamepads[gamepad];
                    if (player.isMyTurn) {
                        for (int b = 0; b < 9; b++) {
                            if (gamepad.GetButton(b.ToString()) && Grid[b].CurrentMark == 0) {
                                SetCell(b, player.Xs ? 1 : 2);

                                //TODO: Fill Update Data with FX Channel Data, or use builtin functions of FyoApplication
                                SGUpdateMsg UpdateMsg = new SGUpdateMsg(gamepad);
                                socket.Emit("SGUpdateMsg", UpdateMsg);
                                break;
                            }
                        }
                    }
                }
                break;
        }
    }

    protected override void OnGamepadUnplugged(SocketGamepad gamepad) {
        if (ActiveGamepads.ContainsKey(gamepad)) {
            //Remove from players
            TicTacToePlayer player = (TicTacToePlayer)ActiveGamepads[gamepad];
            ActiveGamepads.Remove(gamepad);
            Destroy(player);
        }
    }

    protected void StartGame() {
        Debug.Log("[Tic Tac Toe] Starting Game");
        Reset();
        PlayerTurn = 1;
        ((TicTacToePlayer)LocalPlayers[0]).isMyTurn = true;
    }

    protected void TriggerEndgame() {
        if (LocalPlayers[0] != null)
            LocalPlayers[0].Ready = false;

        if (LocalPlayers[1] != null)
            LocalPlayers[1].Ready = false;

        switch (Winner) {
            case 1:
                //TODO: Display Text
                Debug.Log("[Tic Tac Toe] X Won");
                ((TicTacToePlayer)LocalPlayers[0]).Wins++;
                break;
            case 2:
                //TODO: Display Text
                Debug.Log("[Tic Tac Toe] O Won");
                ((TicTacToePlayer)LocalPlayers[1]).Wins++;
                break;
            case 3:
                //TODO: Display Text
                Debug.Log("[Tic Tac Toe] Draw");
                ((TicTacToePlayer)LocalPlayers[0]).Draws++;
                ((TicTacToePlayer)LocalPlayers[1]).Draws++;
                break;
        }

        //TODO: Feedback to controller - Show "Ready" For selection
    }
    
    protected void SetReady(int mark) {
        if (mark < LocalPlayers.Count && LocalPlayers[mark] != null) {
            if(LocalPlayers[mark] != null)
                LocalPlayers[mark].Ready = true;
        }

        if (LocalPlayers[0] != null && LocalPlayers[1] != null) {
            if (LocalPlayers[0].Ready && LocalPlayers[1].Ready) {
                StartGame();
            }
        }
    }

    public void SetCell(int cellid, int mark) {
#if UNITY_EDITOR
        if (mark == 0) {
            Debug.Log("[Tic Tac Toe] Marking Cell " + cellid.ToString() + "clear");
        } else if (mark == 1) {
            Debug.Log("[Tic Tac Toe] Marking Cell " + cellid.ToString() + "for " + "X");
        } else if (mark == 2) {
            Debug.Log("[Tic Tac Toe] Marking Cell " + cellid.ToString() + "for " + "Y");
        }
#endif

        if (mark == PlayerTurn) {
            if (mark == 0) {
                Grid[cellid].X.SetActive(false);
                Grid[cellid].O.SetActive(false);
            } else if (mark == 1) { //X
                Grid[cellid].X.SetActive(true);
                Grid[cellid].O.SetActive(false);
            } else if (mark == 2) { // Y
                Grid[cellid].X.SetActive(false);
                Grid[cellid].O.SetActive(true);
            }

            if (PlayerTurn == 1) {
                ((TicTacToePlayer)LocalPlayers[0]).isMyTurn = false;
                PlayerTurn = 2;
                ((TicTacToePlayer)LocalPlayers[1]).isMyTurn = true;
            } else if (PlayerTurn == 2) {
                ((TicTacToePlayer)LocalPlayers[1]).isMyTurn = false;
                PlayerTurn = 1;
                ((TicTacToePlayer)LocalPlayers[0]).isMyTurn = true;
            }

            if ((Winner = CheckWinner()) > 0) {
                //Winner winner, chicken dinner!
                TriggerEndgame();
            }
        }
    }
}
