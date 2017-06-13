using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCountdown : MonoBehaviour {
    public float TimeRemaining = 0.0f;
	void Update () {
        TimeRemaining -= Time.deltaTime;
        if (TimeRemaining < 0.0f)
            gameObject.SetActive(false);
	}
}
