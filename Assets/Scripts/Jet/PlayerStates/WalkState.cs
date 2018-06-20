using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : IPlayerState
{
    public WalkState(Player p, PlayerStateMachine psm, PlayerInput pI)
    {
        player = p;
        wsp = player.charAttrib.walkState;
        this.psm = psm;
        input = pI;
    }
    Player player;
    PlayerStateMachine psm;
    PlayerInput input;
    Quaternion targetRotation;
    Vector3 localInput;
    WalkStateProperties wsp;
    public override void Update()
    {
        if (!player.isGrounded)
        {
            psm.Change("fall");
            return;
        }

        localInput = player.cameraRig.transform.rotation * input.lstick;
        localInput.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(localInput);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRot, player.cornering);

        debugString = "" + Vector3.Angle(localInput, player.velocity.normalized);

        if (player.currentSpeed < wsp.maxWalkSpeed)
        {
            player.velocity = player.transform.forward * (wsp.walkAccel + player.currentSpeed);
        }
        else
        {
            player.velocity = player.transform.forward * (player.currentSpeed) ;
        }

        player.Ccontroller.Move(player.velocity*Time.deltaTime);

    }

    public override void HandleInput()
    {
        if (input.lstick.sqrMagnitude <= wsp.walkThreshold)
        {
            psm.Change("idle");
            return;
        }
        else
        if (input.lstick.sqrMagnitude >= wsp.runThreshold)
           {
               psm.Change("run");
               return;
           }
   
        if (input.aButton)
        {
            psm.Change("jump");
            return;
        }
        if (input.bButton)
        {
            psm.Change("BoostRun");
        }
    }

    public override void Enter()
    {
        //  Debug.Log("Entered WalkState");
    }

    public override void Exit()
    {
        //  Debug.Log("Exited WalkState");
    }

    public override string ToString()
    {
        return "Walk State";
    }

}

