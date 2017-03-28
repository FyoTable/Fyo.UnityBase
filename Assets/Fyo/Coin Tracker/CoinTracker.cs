using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTracker : MonoBehaviour {
    protected static CoinTracker _instance = null;
    public static CoinTracker Instance {
        get {
            if (_instance == null)
                new GameObject().AddComponent<CoinTracker>();
            return _instance;
        }
        protected set {
            _instance = value;
        }
    }

    public bool AddCoin = false;

    CoinTracker() {
        Instance = this;
    }

    private int CoinsPerCredit = 1;
    public int CreditCost {
        get {
            return CoinsPerCredit;
        }
    }

    private int Coins = 0;
    public  float Credits() {
        return (float)Coins / (float)CoinsPerCredit;
    }

    CoinMessage msg = new CoinMessage();

    public  bool SpendCredits(int cost) {
        if (Credits() < (float)cost) {
            Send("InsufficentCredits", cost * CoinsPerCredit);
            return false;
        }

        Coins -= cost * CoinsPerCredit;
        Send("CoinsSpent", cost * CoinsPerCredit);
        msg.SaveToFile("Coin.txt");
        return true;
    }

    private void Start() {
        if (!msg.LoadFromFile("Coin.txt")) {
            Debug.Log("Saving Coin.txt");
            msg.SaveToFile("Coin.txt");
        }

        Coins = Mathf.CeilToInt(msg.Credits) * CoinsPerCredit;
        Send("CoinsAdded", Coins);
    }

    void Update() {
        if (AddCoin) {
            AddCoin = false;
            Coins++;
            msg.SetField("Credits", Credits());
            msg.SaveToFile("Coin.txt");
            Send("CoinsAdded", 1);
        }

        if (Input.GetKeyDown(KeyCode.C) && Input.GetKeyDown(KeyCode.R)) {
            float c = Credits();
            Coins = 0;
            msg.SetField("Credits", Credits());
            msg.SaveToFile("Coin.txt");
            Send("CreditsSpent", Mathf.RoundToInt(c));
        }
    }

    #region Messaging
    static List<GameObject> MessageReceivers = new List<GameObject>();
    public void AddMessageReceiver(GameObject receiver) {
        if (!MessageReceivers.Contains(receiver)) {
            MessageReceivers.Add(receiver);
        }
    }

    public void RemoveMessageReceiver(GameObject receiver) {
        if (MessageReceivers.Contains(receiver)) {
            MessageReceivers.Remove(receiver);
        }
    }

    protected void Send(string msgFunc, int coins) {
        if (MessageReceivers.Count > 0) {
            for (int i = 0; i < MessageReceivers.Count; i++) {
                MessageReceivers[i].SendMessage(msgFunc, coins, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    #endregion
}
