using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToePlayer : FyoPlayer {
    public bool Xs = false;
    public bool isMyTurn = false;
    public GameObject PlayerIcon;
    public uint Wins = 0;
    public uint Losses = 0;
    public uint Draws = 0;
}
