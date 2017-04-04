using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelectPanel : MonoBehaviour {
    AppHandshakeMsg GameData = null;
    
    public int Cost = 2;
    public string Name = "Game";
    public int MinPlayers = 1;
    public int MaxPlayers = 4;

    public virtual IEnumerator StartGame() {
        yield return null;
    }

    public void SelectGame() {
        if (GameData != null && GameData.HasField("Cost")) {
            if (CoinTracker.Instance.SpendCredits(Mathf.FloorToInt(GameData.GetField("Cost").f))) {
                //Start Game
                StartCoroutine("StartGame");
            }
        }
    }
}
