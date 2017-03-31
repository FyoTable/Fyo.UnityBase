using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class ZombieSurvivorGame : SocketGamepadManager {
    public ZombieSurvivorPlayer[] Players = new ZombieSurvivorPlayer[4];

    public override void AddExistingGamepad(SocketGamepad gamepad) {
        base.AddExistingGamepad(gamepad);
    }

    public override void HandleAppHandshake(SocketIOEvent e) {
        base.HandleAppHandshake(e);
    }

    public override void HandleGamepadHandshake(SocketIOEvent e) {
        base.HandleGamepadHandshake(e);
    }

    public override void HandleGamepadUpdate(SocketIOEvent e) {
        base.HandleGamepadUpdate(e);
    }

    public override void HandleGamepadDisconnected(SocketIOEvent e) {
        base.HandleGamepadDisconnected(e);
    }

    public override void HandleConnectedToSGServer(SocketIOEvent e) {
        base.HandleConnectedToSGServer(e);
    }
    
}
