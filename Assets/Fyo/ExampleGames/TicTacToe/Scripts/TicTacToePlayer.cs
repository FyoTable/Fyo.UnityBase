using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToePlayer : FyoPlayer {
    TicTacToeGame GameManager;
    public bool Xs = false;
    public bool isMyTurn = false;
    public GameObject PlayerIcon;
    public uint Wins = 0;
    public uint Losses = 0;
    public uint Draws = 0;

    public void Start() {
        GameManager = GameObject.FindObjectOfType<TicTacToeGame>();
        if (!GameManager)
            Debug.LogError("GameManager Missing!");
    }

    public void Update() {
        if (Gamepad != null && isMyTurn) {
            for (int b = 0; b < 9; b++) {
                if (Gamepad.GetButton(b.ToString()) && GameManager.Grid[b].CurrentMark == 0) {
                    GameManager.SetCell(b, Xs ? 1 : 2);
                }
            }
        }
    }

    public void Mark(int tile) {
        if(isMyTurn)
            GameManager.SetCell(tile, Xs ? 1 : 2);
    }
}
