using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketGamepadTestIndicator : MonoBehaviour {
    public SocketGamepad Gamepad;
    public List<GameObject> Indicators = new List<GameObject>(20);

    int i = 0;
    private void LateUpdate() {
        if (Gamepad != null) {
            if (Gamepad.inputs.Length == 20) {
                i = 0;
                for (; i < 10; i++)
                    Indicators[i].SetActive(Gamepad.inputs[i] > 0);
                for (; i < 20; i++)
                    Indicators[i].transform.localPosition = (Indicators[i].transform.right * Gamepad.inputs[i]);
            }
        }
    }
}
