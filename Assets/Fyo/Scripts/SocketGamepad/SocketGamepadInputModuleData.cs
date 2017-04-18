using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
public class SocketGamepadInputModuleData : JSONObject {
    public float Horizontal = 0.0f;
    public float Vertical = 0.0f;
    public bool Confirm = false;
    public bool Cancel = false;

    private void Setup() {
        AddField("Horizontal", Horizontal);
        AddField("Vertical", Vertical);
        AddField("Confirm", Confirm);
        AddField("Cancel", Cancel);
    }

    public SocketGamepadInputModuleData() : base() {
        Setup();
    }

    public SocketGamepadInputModuleData(JSONObject clone) : base() {
        Setup();
        Clone(clone);
    }

    public void Clone(JSONObject clone) {
        if (clone.HasField("Horizontal"))
            clone.GetField(ref Horizontal, "Horizontal");
        if (clone.HasField("Vertical"))
            clone.GetField(ref Vertical, "Vertical");
        if (clone.HasField("Confirm"))
            clone.GetField(ref Confirm, "Confirm");
        if (clone.HasField("Cancel"))
            clone.GetField(ref Cancel, "Cancel");
        Serialize();
    }

    public void Serialize() {
        SetField("Horizontal", Horizontal);
        SetField("Vertical", Vertical);
        SetField("Confirm", Confirm);
        SetField("Cancel", Cancel);
    }

    public void Deserialize() {
        GetField(ref Horizontal, "Horizontal");
        GetField(ref Vertical, "Vertical");
        GetField(ref Confirm, "Confirm");
        GetField(ref Cancel, "Cancel");
    }
}
