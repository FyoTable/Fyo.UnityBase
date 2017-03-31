using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class ZombieSurvivorGame : FyoApplication {
    public ZombieSurvivorPlayer[] Players = new ZombieSurvivorPlayer[4];

    protected override void OnAddGamepad(int PlayerId) {
        base.OnAddGamepad(PlayerId);
    }

    public override void AddExistingGamepad(SocketGamepad gamepad) {
        base.AddExistingGamepad(gamepad);
    }

    #region Handlers
    protected override void HandleAppHandshake(SocketIOEvent e) {
        base.HandleAppHandshake(e);
    }

    protected override void HandleGamepadHandshake(SocketIOEvent e) {
        base.HandleGamepadHandshake(e);
    }

    protected override void HandleGamepadUpdate(SocketIOEvent e) {
        base.HandleGamepadUpdate(e);
    }

    protected override void HandleGamepadDisconnected(SocketIOEvent e) {
        base.HandleGamepadDisconnected(e);
    }

    protected override void HandleConnectedToSGServer(SocketIOEvent e) {
        base.HandleConnectedToSGServer(e);
    }
    #endregion
}
