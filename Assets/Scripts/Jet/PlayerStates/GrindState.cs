using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindState : IPlayerState {

    public GrindState(Player p, PlayerStateMachine psm, PlayerInput pI)
    {
        player = p;
        gsp = player.charAttrib.grindStateProperties;
        this.psm = psm;
        input = pI;
    }
    Player player;
    PlayerStateMachine psm;
    PlayerInput input;
    GrindStateProperties gsp;
    Vector3 lastInput;
    Rail rail;
    public override void Enter()
    {
        rail = player.GetRail();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void HandleInput()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}
