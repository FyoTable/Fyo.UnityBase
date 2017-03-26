using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinUI : MonoBehaviour {
    public UnityEngine.UI.Text CountText;
    
	// Use this for initialization
	void Start () {
        CoinTracker.Instance.AddMessageReceiver(gameObject);
        SetText();
	}

    void SetText() {
        if (CountText != null)
            CountText.text = CoinTracker.Instance.Credits().ToString();
    }

    public void CreditsSpent(int coins) {
        SetText();
    }

    public void InsufficientCredits(int coins) {
        SetText();
    }

    public void CoinsAdded(int coins) {
        SetText();
    }
}
