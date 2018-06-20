using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : IPlayerState
{
    Player player;
    FallStateProperties fsp;
    PlayerStateMachine psm;
    PlayerInput input;

    public FallState(Player p, PlayerStateMachine psm,PlayerInput pI)
    {
        player = p;
        fsp = player.charAttrib.fallState;
        this.psm = psm;
        input = pI;
    }

    public override void Enter()
    {
        //Debug.Log("Entered FallState");
    }

    public override void Exit()
    {
        //Debug.Log("Exited FallState");
    }

    public override void HandleInput()
    {
    }

    public override void Update()
    {
        if (player.isGrounded)
        {
            psm.Change("idle");
            return;
        }

        if (player.isOnRail())
        {
            psm.Change("rail");
            return;
        }

        Vector3 downwardVelocity = Vector3.down * fsp.fallSpeed ;
        player.Ccontroller.Move((player.velocity+ downwardVelocity)*Time.deltaTime);
    }
}
