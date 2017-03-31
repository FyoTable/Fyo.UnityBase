using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class TicTacToeGame : SocketGamepadManager {
    public GameObject GridObject;
    public List<TicTacToeCell> Grid = new List<TicTacToeCell>();

    int PlayerTurn = 1;
    int Winner = -1;

    protected override void OnStart() {
        LocalPlayers = new GamePlayer[2];
        LocalPlayers[0] = new TicTacToePlayer();
        LocalPlayers[1] = new TicTacToePlayer();
    }

    public override void HandleGamepadHandshake(SocketIOEvent e) {
        SGHandshakeMsg handshakeMsg = new SGHandshakeMsg(e.data);
        SocketGamepad gamepad;
        if (LocalPlayers[0].Gamepad == null) {
            gamepad = CreateGamepad(LocalPlayers[0], handshakeMsg.PlayerId);
        } else {
            gamepad = CreateGamepad(LocalPlayers[1], handshakeMsg.PlayerId);
        }

        if (InputIndicatorPrefab != null) {
            SocketGamepadTestIndicator Tester = ((GameObject)Instantiate(InputIndicatorPrefab)).GetComponent<SocketGamepadTestIndicator>();
            Tester.Gamepad = gamepad;
        }

        Debug.Log("[Tic Tac Toe] Gamepad Handshake: " + gamepad.PlayerId.ToString());
    }

    public override void HandleGamepadDisconnected(SocketIOEvent e) {
        int gid = 0;
        e.data.GetField(ref gid, "PlayerId");
        if (gid > -1 && gid < Gamepads.Count) {
            SocketGamepad gamepad = GetGamepad(gid);
            Debug.Log("[Tic Tac Toe] Removing Gamepad " + gid.ToString() + " and player.");

            if (LocalPlayers[0].Gamepad == gamepad) {
                ((TicTacToePlayer)LocalPlayers[0]).PlayerIcon.SetActive(false);
                LocalPlayers[0].Gamepad = null;
            }

            if (LocalPlayers[1].Gamepad == gamepad) {
                ((TicTacToePlayer)LocalPlayers[1]).PlayerIcon.SetActive(false);
                LocalPlayers[1].Gamepad = null;
            }

            Gamepads.Remove(gamepad);
            Destroy(gamepad);

            RenumberConnectedGamepads();
        }
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
        LocalPlayers[0].Ready = LocalPlayers[1].Ready = false;
    }

    public override void Start() {
        base.Start();
        dAddGamepad = OnAddGamepad;
        Reset();
    }

    public void OnAddGamepad(int GamepadId) {
        if (!((TicTacToePlayer)LocalPlayers[0]).PlayerIcon.activeSelf) {
            if (((TicTacToePlayer)LocalPlayers[0]).PlayerIcon != null) {
                ((TicTacToePlayer)LocalPlayers[0]).PlayerIcon.SetActive(true);
            }
            LocalPlayers[0].Gamepad = GetGamepad(GamepadId);
            //TODO: Feedback to controller - Show "Ready" For selection
        } else if (!((TicTacToePlayer)LocalPlayers[1]).PlayerIcon.activeSelf) {
            if (((TicTacToePlayer)LocalPlayers[1]).PlayerIcon != null) {
                ((TicTacToePlayer)LocalPlayers[1]).PlayerIcon.SetActive(true);
            }
            LocalPlayers[1].Gamepad = GetGamepad(GamepadId);
            //TODO: Feedback to controller - Show "Ready" For selection
        } else {
            Debug.Log("[Tic Tac Toe] Only two players can play Tic Tac Toe, fuck off.");
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
        if (mark < LocalPlayers.Length && LocalPlayers[mark] != null) {
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
        if (Winner >= 0) {
            //Playing Game

            if (mark == 0) {
                Debug.Log("[Tic Tac Toe] Marking Cell " + cellid.ToString() + "clear");
            } else if (mark == 1) {
                Debug.Log("[Tic Tac Toe] Marking Cell " + cellid.ToString() + "for " + "X");
            } else if (mark == 2) {
                Debug.Log("[Tic Tac Toe] Marking Cell " + cellid.ToString() + "for " + "Y");
            }

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
        } else {
            //Pre/After Game Menu
            switch (cellid) {
                case 4:
                    //Ready
                    SetReady(mark);

                    if (Gamepads.Count > 1 && LocalPlayers[0].Ready && LocalPlayers[1].Ready) {                       
                        StartGame();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
