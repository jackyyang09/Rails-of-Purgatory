using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSkidState : IPlayerState
{
    public RunSkidState(Player p, PlayerStateMachine psm, PlayerInput pI)
    {
        player = p;
        rksp = player.charAttrib.runSkidState;
        this.psm = psm;
        input = pI;
    }
    Player player;
    RunSkidStateProperties rksp;
    PlayerStateMachine psm;
    PlayerInput input;
    Vector3 lastInput;
    public override void Enter()
    {
        player.velocity = player.velocity.normalized * rksp.runSkidSpeed;
    }

    public override void Exit()
    {
        //Debug.Log("Exited RunState");
    }

    public override void HandleInput()
    {
        if (input.aButton)
        {
            Vector3 localInput = (player.cameraRig.transform.rotation * input.lstick);
            localInput.y = 0;
            player.transform.rotation = Quaternion.LookRotation(localInput);
            psm.Change("jump");
            return;
        }

        if (Vector3.Dot(input.lstick, lastInput) < 0)
        {

            psm.Change("run");
            return;
        }
        if (input.bButton)
        {
            psm.Change("BoostRun");
        }

    }

    public override void Update()
    {
        if (!player.isGrounded)
        {
            psm.Change("fall");
            return;
        }
        /*if (player.touchingRail && player.canGrind)
        {
            psm.Change("grind");
        }*/


        if (player.velocity.magnitude > 0.01)
            player.velocity -= player.velocity.normalized * rksp.runSkidDecay;
        else
        {
            Debug.Log("");
            player.velocity = Vector3.zero;
            psm.Change("idle");
        }

        

        player.Ccontroller.Move(player.velocity);



    }

    public override string ToString()
    {
        return "Run Skid State";
    }
}
