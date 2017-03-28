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
        if(gamepad != null)
            GamepadManager.AddExistingGamepad(gamepad);
    }

    void UpdateInput() {
        //Buttons 0-9       
        GamepadInputs[0] = Input.GetAxisRaw(PlayerStr + Player.ToString() + ButtonStr + "0");
        GamepadInputs[1] = Input.GetAxisRaw(PlayerStr + Player.ToString() + ButtonStr + "1");
        GamepadInputs[2] = Input.GetAxisRaw(PlayerStr + Player.ToString() + ButtonStr + "2");
        GamepadInputs[3] = Input.GetAxisRaw(PlayerStr + Player.ToString() + ButtonStr + "3");
        GamepadInputs[4] = Input.GetAxisRaw(PlayerStr + Player.ToString() + ButtonStr + "4");
        GamepadInputs[5] = Input.GetAxisRaw(PlayerStr + Player.ToString() + ButtonStr + "5");
        GamepadInputs[6] = Input.GetAxisRaw(PlayerStr + Player.ToString() + ButtonStr + "6");
        GamepadInputs[7] = Input.GetAxisRaw(PlayerStr + Player.ToString() + ButtonStr + "7");
        GamepadInputs[8] = Input.GetAxisRaw(PlayerStr + Player.ToString() + ButtonStr + "8");
        GamepadInputs[9] = Input.GetAxisRaw(PlayerStr + Player.ToString() + ButtonStr + "9");

        //Axes 10-13
        //Left X/Y
        GamepadInputs[10] = Input.GetAxisRaw(PlayerStr + Player.ToString() + AxisStr + "0");
        GamepadInputs[11] = Input.GetAxisRaw(PlayerStr + Player.ToString() + AxisStr + "1");

        //Right X/Y
        GamepadInputs[12] = Input.GetAxisRaw(PlayerStr + Player.ToString() + AxisStr + "2");
        GamepadInputs[13] = Input.GetAxisRaw(PlayerStr + Player.ToString() + AxisStr + "3");

        //Haptics 14-19
        //Accelerometer
        GamepadInputs[14] = Input.GetAxisRaw(PlayerStr + Player.ToString() + AccelStr + "x");
        GamepadInputs[15] = Input.GetAxisRaw(PlayerStr + Player.ToString() + AccelStr + "y");
        GamepadInputs[16] = Input.GetAxisRaw(PlayerStr + Player.ToString() + AccelStr + "z");

        //Gyroscope
        GamepadInputs[17] = Input.GetAxisRaw(PlayerStr + Player.ToString() + GyroStr + "x");
        GamepadInputs[18] = Input.GetAxisRaw(PlayerStr + Player.ToString() + GyroStr + "y");
        GamepadInputs[19] = Input.GetAxisRaw(PlayerStr + Player.ToString() + GyroStr + "z");
    }

    void Update () {
        if (GamepadManager != null) {
            UpdateInput();
            msg.SocketGamepadID = gamepad.ID;
            msg.inputs = GamepadInputs;
            msg.Serialize();
            GamepadManager.HandleGamepadUpdate(new SocketIO.SocketIOEvent("SGUpdateMsg", msg));
        }
    }
}
