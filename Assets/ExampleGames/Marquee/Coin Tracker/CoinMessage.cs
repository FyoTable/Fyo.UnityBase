using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class Coin {
    public float Credits = 0;
    public Coin() {
    }
    public Coin(float credits) {
        Credits = credits;
    }
}

[System.Serializable]
public class CoinMessage : JSONObject {
    float _credits = 0;
    public float Credits {
        get {
            return _credits;
        }
    }

    protected void Initialize() {
        AddField("Credits", _credits);
    }

    public CoinMessage() {
        Initialize();
    }

    public CoinMessage(int StartCredits) {
        _credits = StartCredits;
        Initialize();
    }

    public string Serialize() {
        SetField("Credits", _credits);
        return ToString();
    }

    public void Deserialize() {
        GetField(ref _credits, "Credits");
    }
    
    public bool SaveToFile(string Filename) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        string FilePath = Application.persistentDataPath + "/" + Filename;
        if (!File.Exists(FilePath))
            file = File.Open(FilePath, FileMode.Create);
        else
            file = File.Open(FilePath, FileMode.Truncate);

        if (!file.CanWrite)
            return false;

        Deserialize();
        bf.Serialize(file, new Coin(_credits));
        file.Close();
        return true;
    }

    public bool LoadFromFile(string Filename) {
        string FilePath = Application.persistentDataPath + "/" + Filename;
        if (!File.Exists(FilePath)) 
             return false;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(FilePath, FileMode.Open);
        Coin coin = (Coin)bf.Deserialize(file);
        SetField("Credits", coin.Credits);
        _credits = coin.Credits;
        file.Close();
        return true;
    }
}
