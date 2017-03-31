using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

//Socket.IO Gamepad Connection
public class SocketGamepad : MonoBehaviour {
    public int PlayerId = -1;

    public const int GamepadInputCount = 20;
    public int LocalId = 0;

    public static Dictionary<string, int> Interface = new Dictionary<string, int>() {
        {"button 0", 0},
        {"button 1", 1},
        {"button 2", 2},
        {"button 3", 3},
        {"button 4", 4},
        {"button 5", 5},
        {"button 6", 6},
        {"button 7", 7},
        {"button 8", 8},
        {"button 9", 9},
        {"axis 0", 10},
        {"axis 1", 11},
        {"axis 2", 12},
        {"axis 3", 13},
        {"accel x", 14},
        {"accel y", 15},
        {"accel z", 16},
        {"gyro x", 17},
        {"gyro y", 18},
        {"gyro z", 19}
    };

    public float[] inputs = new float[GamepadInputCount];

    public float Get(string inputStr) {
        return Interface.ContainsKey(inputStr) ? inputs[Interface[inputStr]] : 0.0f;
    }

    public bool GetButton(int button) {
        return (button < 10) ? (inputs[button] > 0.0f) : false;
    }

    public float GetAxis(int axis) {
        return ((axis > 9) && (axis < 14)) ? inputs[axis + 10] : 0.0f;
    }

    public float GetAccel(int axis) {
        return ((axis >= 0) && (axis < 3)) ? inputs[axis + 14] : 0.0f;
    }

    public float GetGyro(int axis) {
        return ((axis >= 0) && (axis < 3)) ? inputs[axis + 17] : 0.0f;
    }

    void Start() {
        SocketGamepadManager manager = GameObject.FindObjectOfType<SocketGamepadManager>();
        if (!manager.HasGamepad(this)) {
            manager.AddExistingGamepad(this);
        } 
    }
}
