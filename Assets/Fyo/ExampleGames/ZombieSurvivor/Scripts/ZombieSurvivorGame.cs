﻿using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class ZombieSurvivorGame : FyoApplication {
    public ZombieSurvivorPlayer[] Players = new ZombieSurvivorPlayer[4];

    #region Handlers
    protected override void AssignExtraHandlers() {
    }

    protected override void OnConnected() {
    }

    protected override void OnHandshake(AppHandshakeMsg handshakeMsg) {
    }

    protected override void OnDisconnected() {
    }

    protected override void OnGamepadPluggedIn(SocketGamepad gamepad) {
    }

    protected override void OnUpdateGamepad(SocketGamepad gamepad) {
    }

    protected override void OnGamepadUnplugged(SocketGamepad gamepad) {
    }
    #endregion
}
