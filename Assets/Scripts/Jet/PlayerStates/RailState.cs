using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailState : IPlayerState
{

    public RailState(Player p, PlayerStateMachine psm, PlayerInput pI)
    {
        player = p;
        rsp = player.charAttrib.runState;
        this.psm = psm;
        input = pI;
    }

    Player player;
    PlayerStateMachine psm;
    PlayerInput input;
    Vector3 lastInput;
    RunStateProperties rsp;
    public override void Enter()
    {
        lastInput *= 0;
    }

    public override void Exit()
    {
        //Debug.Log("Exited RunState");
    }

    public override void HandleInput()
    {

        Vector3 localInput = player.cameraRig.transform.rotation * input.lstick;

        if (input.aButton)
        {
            psm.Change("jump");
            player.GetCurrentRail().LeaveRail();
            return;
        }

        //if (input.lstick.sqrMagnitude < rsp.walkThreshold)
        //{
        //    psm.Change("idle");
        //    return;
        //}
        //else if (input.lstick.sqrMagnitude < rsp.runThreshold)
        //{
        //    psm.Change("walk");
        //    return;
        //}

        lastInput = localInput;
    }

    public override void Update()
    {
        if (!player.isGrounded && !player.isOnRail())
        {
            psm.Change("fall");
            return;
        }

        //Vector3 localInput = player.cameraRig.transform.rotation * input.lstick;
        //localInput.y = 0;
        //Quaternion currentRot = player.transform.rotation;
        //Quaternion desiredRotation = player.transform.rotation;
        //if (localInput != Vector3.zero)
        //    desiredRotation = Quaternion.LookRotation(localInput);
        //
        //player.transform.rotation = Quaternion.RotateTowards(currentRot, desiredRotation, rsp.turnRate);
        //
        //Vector3 accelDir = player.transform.forward * rsp.accel * localInput.magnitude;
        //player.velocity += accelDir;
        //player.velocity = Vector3.ClampMagnitude(player.velocity, rsp.maxRunSpeed);
        //
        //
        //
        //player.currentSpeed = player.velocity.magnitude;
        //RaycastHit hit;
        //Ray ray = new Ray(player.transform.position, -player.transform.up);
        //Physics.Raycast(ray, out hit);
        //player.velocity = Vector3.ProjectOnPlane(player.velocity, hit.normal);
        //player.Ccontroller.Move(player.velocity * Time.deltaTime);
    }

    public override string ToString()
    {
        return "Rail State";
    }
}
