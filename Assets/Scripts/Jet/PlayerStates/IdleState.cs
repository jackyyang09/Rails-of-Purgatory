using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IPlayerState

{

    public IdleState(Player p, PlayerStateMachine psm, PlayerInput input)
    {
        this.player = p;
        isp = player.charAttrib.idleState;
        this.psm = psm;
        this.input = input;
    }

    Player player;
    IdleStateProperties isp;
    PlayerStateMachine psm;
    PlayerInput input;

    public override void Update()
    {
        if (!player.isGrounded)
        {
            psm.Change("fall");
            return;
        }




        Vector3 localInput = player.cameraRig.transform.rotation * input.lstick;
        if (localInput.magnitude < 0.1)
        {
            player.velocity *= isp.deaccel;

        }
        localInput.y = 0;

        Quaternion targetRot = Quaternion.LookRotation(localInput);
        RaycastHit hit;
        Ray ray = new Ray(player.transform.position, -player.transform.up);
        Physics.Raycast(ray, out hit);
        player.velocity = Vector3.ProjectOnPlane(player.velocity,hit.normal);
        player.Ccontroller.Move(player.velocity*Time.deltaTime);
        if(targetRot.eulerAngles != Vector3.zero)
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRot, isp.rotsPerFrame);
    }

    public override void HandleInput()
    {
        //jump
        if (input.aButton)
        {
            psm.Change("jump");
            return;
        }

        //run
        if (input.lstick.sqrMagnitude >= isp.runThreshold)
        {
            psm.Change("run");
            return;
        } else //walk
        if (input.lstick.sqrMagnitude >= isp.walkThreshold)
        {
            psm.Change("walk");
            return;
        }
        if (input.bButton)
        {
            psm.Change("BoostRun");
        }
    }

    public override void Enter()
    {
        //  Debug.Log("Entered IdleState");
    }

    public override void Exit()
    {
        //  Debug.Log("Exited IdleState");
    }

    public override string ToString()
    {
        return "Idle State";
    }
}
