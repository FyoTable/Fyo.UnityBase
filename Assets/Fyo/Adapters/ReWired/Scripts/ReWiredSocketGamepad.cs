// Copyright (c) 2017 Kevin W. Gerber, DCCK LLC. All rights reserved.
using UnityEngine;
using System;
using System.Collections.Generic;
using Rewired;

#if SOCKET_GAMEPAD_REWIRED

/* This is a basic example of using the SocketGamepad library with ReWired
 * It uses the builtin axis and button retrieval functions for simplicity
 * If custom SocketGamepad mapping is required simply inherit this class
 * and override GetAxisValueCallback and GetButtonValueCallback
 */


[AddComponentMenu("")]
[RequireComponent(typeof(Fyo.SocketGamepad))]
public class ReWiredSocketGamepad : MonoBehaviour {
    public int playerId;
    public string controllerTag;

    private CustomController controller;
    private Fyo.SocketGamepad gamepad;

    [NonSerialized] // Don't serialize this so the value is lost on an editor script recompile.
    private bool initialized;

    private void Awake() {
        if (SystemInfo.deviceType == DeviceType.Handheld && Screen.orientation != ScreenOrientation.Landscape) { // set screen to landscape mode
            Screen.orientation = ScreenOrientation.Landscape;
        }
        Initialize();
    }

    private void Initialize() {           
        gamepad = GetComponent<Fyo.SocketGamepad>();
            
        // Find the controller we want to manage
        Player player = ReInput.players.GetPlayer(playerId); // get the player
        controller = player.controllers.GetControllerWithTag<CustomController>(controllerTag); // get the controller

        if (controller == null) {
            Debug.LogError("A matching controller was not found for tag \"" + controllerTag + "\"");
        }

        // Callback Update Method:
        // Set callbacks to retrieve current element values.
        // This is a different way of updating the element values in the controller.
        // You set an update function for axes and buttons and these functions will be called
        // to retrieve the current source element values on every update loop in which input is updated.
        if (controller != null) {
            controller.SetAxisUpdateCallback(GetAxisValueCallback);
            controller.SetButtonUpdateCallback(GetButtonValueCallback);
        }

        initialized = true;
    }

    private void Update() {
        if (!ReInput.isReady)
            return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
        if (!initialized)
            Initialize(); // Reinitialize after a recompile in the editor
    }

    // Callbacks

    protected virtual float GetAxisValueCallback(int index) {
        // This will be called by each axis element in the Custom Controller when updating its raw value
        // Get the current value from the source axis at index
        return gamepad.GetAxis("axis " + index.ToString());
    }

    protected virtual bool GetButtonValueCallback(int index) {
        // This will be called by each button element in the Custom Controller when updating its raw value
        // Get the current value from the source button at index
        return gamepad.GetButton("button " + index.ToString());
    }

}

#endif