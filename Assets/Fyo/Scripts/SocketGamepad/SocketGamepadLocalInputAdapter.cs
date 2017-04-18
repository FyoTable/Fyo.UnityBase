using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fyo;

/// <summary>
/// Adapter for real gamepads (PS4, XBox 360, XBox One, etc) to translate to SocketGamepad 
/// Uses the default (SGUpdateMsg) message format as used in 'default_controller'
/// 
/// Primarily for local testing to remove the need for the Node.js server
/// for basic gameplay tests with the default controllers.
/// 
/// If you need to specialize this it is recommended to copy and build your
/// own version of this class as it relates to your own use of SocketGamepad
/// </summary>
public class SocketGamepadLocalInputAdapter : MonoBehaviour {
    FyoApplication GamepadManager;
    JSONObject InputData = new JSONObject();

    protected static string PlayerStr = "player ";
    protected static string ButtonStr = " button ";
    protected static string AxisStr = " axis ";

    public int LocalInputIndex = 0;

    //This is set manually to represent the PlayerId sent from the server
    public int SocketPlayerId = -1;

    private void Start() {
        GamepadManager = FindObjectOfType<FyoApplication>();

        if (LocalInputIndex >= 0 && LocalInputIndex < 8) {
            //Allocate Buttons
            for (int b = 0; b < 10; b++)
                InputData.AddField("button " + b.ToString(), false);

            //Allocate Axes
            for (int a = 0; a < 8; a++)
                InputData.AddField("axis " + a.ToString(), 0.0f);

            SGHandshakeMsg HandshakeMsg = new SGHandshakeMsg();
            HandshakeMsg.PlayerId = SocketPlayerId;
            HandshakeMsg.Serialize();
            GamepadManager.InjectGamepadHandshake(HandshakeMsg);
        } else {
            Debug.LogWarning("Local Input is not configured for a valid Local Gamepad");
        }
    }

    SGUpdateMsg UpdateMsg = new SGUpdateMsg();
    int a = 0, b = 0;
    bool button = false;
    float axis = 0.0f;
    void Update () {
        if (GamepadManager != null) {
            //Buttons 0-9       
            for (b = 0; b < 10; b++) {
                button = Input.GetAxisRaw(PlayerStr + LocalInputIndex.ToString() + ButtonStr + b.ToString()) > 0;
                InputData.SetField("button " + b.ToString(), button);
            }

            //Axes (LeftX, LeftY, RightX, RightY, POVX, POVY, LeftTrigger, RightTrigger) 0-8 (indices 10-17)
            for (a = 0; a < 8; a++) {
                axis = Input.GetAxisRaw(PlayerStr + LocalInputIndex.ToString() + AxisStr + a.ToString());
                InputData.SetField("axis " + a.ToString(), axis);
            }

            UpdateMsg.PlayerId = SocketPlayerId;
            UpdateMsg.Data = InputData;
            UpdateMsg.Serialize();

            GamepadManager.InjectGamepadUpdate(UpdateMsg);
        }
    }
}
