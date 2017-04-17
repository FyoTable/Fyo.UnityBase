using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeInputData : JSONObject {
	void Start () {
        AddField("PlayerSelect", -1);
        AddField("Mark", -1);
	}
}
