using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adapter for Real (tm) gamepads to translate to SocketGamepad message format
/// Primarily for local testing to remove the need for the Node.js server
/// for basic gameplay tests.
/// </summary>
[RequireComponent(typeof(SocketGamepad))]
public class InputToSocketGamepadConnector : MonoBehaviour {
    SocketGamepadManager GamepadManager;
    public SocketGamepad gamepad;
    SGUpdateMsg msg = new SGUpdateMsg();

    [Range(1, 4)]
    public int Player = 0;

    float[] GamepadInputs = new float[] {
        0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 0.0f, 0.0f, 0.0f
    };

    protected string PlayerStr = "player ";
    protected string ButtonStr = " button ";
    protected string AxisStr = " axis ";
    protected string GyroStr = " gyro ";
    protected string AccelStr = " accel ";

    private void Start() {
        GamepadManager = FindObjectOfType<SocketGamepadManager>();
        if (gamepad == null)
            gamepad = GetComponent<SocketGamepad>();
    }

    void UpdateInput() {
        //Buttons 0-9       
        GamepadInputs[0] = Input.GetAxis(PlayerStr + Player.ToString() + ButtonStr + "0");
        GamepadInputs[1] = Input.GetAxis(PlayerStr + Player.ToString() + ButtonStr + "1");
        GamepadInputs[2] = Input.GetAxis(PlayerStr + Player.ToString() + ButtonStr + "2");
        GamepadInputs[3] = Input.GetAxis(PlayerStr + Player.ToString() + ButtonStr + "3");
        GamepadInputs[4] = Input.GetAxis(PlayerStr + Player.ToString() + ButtonStr + "4");
        GamepadInputs[5] = Input.GetAxis(PlayerStr + Player.ToString() + ButtonStr + "5");
        GamepadInputs[6] = Input.GetAxis(PlayerStr + Player.ToString() + ButtonStr + "6");
        GamepadInputs[7] = Input.GetAxis(PlayerStr + Player.ToString() + ButtonStr + "7");
        GamepadInputs[8] = Input.GetAxis(PlayerStr + Player.ToString() + ButtonStr + "8");
        GamepadInputs[9] = Input.GetAxis(PlayerStr + Player.ToString() + ButtonStr + "9");

        //Axes 10-13
        //Left X/Y
        GamepadInputs[10] = Input.GetAxis(PlayerStr + Player.ToString() + AxisStr + "0");
        GamepadInputs[11] = Input.GetAxis(PlayerStr + Player.ToString() + AxisStr + "1");

        //Right X/Y
        GamepadInputs[12] = Input.GetAxis(PlayerStr + Player.ToString() + AxisStr + "2");
        GamepadInputs[13] = Input.GetAxis(PlayerStr + Player.ToString() + AxisStr + "3");

        //Haptics 14-19
        //Gyroscope
        GamepadInputs[14] = Input.GetAxis(PlayerStr + Player.ToString() + GyroStr + "x");
        GamepadInputs[15] = Input.GetAxis(PlayerStr + Player.ToString() + GyroStr + "y");
        GamepadInputs[16] = Input.GetAxis(PlayerStr + Player.ToString() + GyroStr + "z");

        //Accelerometer
        GamepadInputs[17] = Input.GetAxis(PlayerStr + Player.ToString() + AccelStr + "x");
        GamepadInputs[18] = Input.GetAxis(PlayerStr + Player.ToString() + AccelStr + "y");
        GamepadInputs[19] = Input.GetAxis(PlayerStr + Player.ToString() + AccelStr + "z");
    }

    void Update () {
        if (GamepadManager != null) {
            UpdateInput();
            msg.SocketGamepadID = gamepad.ID;
            msg.inputs = GamepadInputs;
            msg.Serialize();
            GamepadManager.HandleGamepadUpdate(new SocketIO.SocketIOEvent("SocketGamepadMessage", msg));
        }
    }
}
