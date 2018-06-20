using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPlayerState  {
    public string debugString;
    public abstract void Update();
    public abstract void HandleInput();

    public abstract void Enter();
    public abstract void Exit();

}