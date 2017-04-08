using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FyoMarquee : FyoApplication {
    protected List<FyoPlayer> Users = new List<FyoPlayer>();
    protected List<int> LocalIds = new List<int>();

    protected override void AssignExtraHandlers() {
    }

    protected override void OnConnected() {
    }

    protected override void OnHandshake(AppHandshakeMsg handshakeMsg) {
    }

    protected override void OnDisconnected() {
    }

    private int GetNextAvailableLocalId() {
        for (int i = 0; i < MaxPlayers; i++) {
            if (!LocalIds.Contains(i)) {
                return i;
            }
        }
        return -1;
    }

    private void RenumberConnectedGamepads() {
        for (int i = 0; i < LocalIds.Count; i++) {
            
        }
    }

    protected override void OnGamepadPluggedIn(SocketGamepad gamepad) {
        if (Users.Find(p => p.Gamepad == gamepad) == null) {
            //Add Gamepad to Ids
            gamepad.LocalId = GetNextAvailableLocalId();

            GameObject PlayerObj = new GameObject("Player " + gamepad.LocalId);
            FyoPlayer player = PlayerObj.AddComponent<FyoPlayer>();
            player.Gamepad = gamepad;

            Users.Add(player);
            ActiveGamepads.Add(gamepad, player);
        }
    }

    protected override void OnUpdateGamepad(SocketGamepad gamepad) {
        if (gamepad.LocalId == 0) {
            //Only allow master controller
            SocketGamepadInputModuleData InputModuleData = new SocketGamepadInputModuleData(gamepad.InputData);
            
        }
    }

    protected override void OnGamepadUnplugged(SocketGamepad gamepad) {
        if (ActiveGamepads.ContainsKey(gamepad)) {
            ActiveGamepads.Remove(gamepad);
            LocalIds.Remove(gamepad.LocalId);
            //Reassign Local Ids
            if (Gamepads.Count > 0) {
                
            }
        }
    }
}
