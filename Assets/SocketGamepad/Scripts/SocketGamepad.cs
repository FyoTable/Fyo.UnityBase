using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

//Uses Sockets.IO to communicate with the CoffeePlayer server and relay inputs
public class SocketGamepad : MonoBehaviour {
    public const int GamepadInputCount = 20;
    public int ID = -1;
    public int LocalId = 0;
    public bool Local = false;

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
        {"gyro x", 14},
        {"gyro y", 15},
        {"gyro z", 16},
        {"accel x", 17},
        {"accel y", 18},
        {"accel z", 19}
    };
    
    public float[] inputs = new float[GamepadInputCount];

    void Start() {
        SocketGamepadManager manager = GameObject.FindObjectOfType<SocketGamepadManager>();
        if (!manager.HasGamepad(this)) {
            manager.AddExistingGamepad(this);
        } 
    }
}
