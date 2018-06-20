using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : IPlayerState
{

    public RunState(Player p, PlayerStateMachine psm, PlayerInput pI)
    {
        player = p;
        rsp = player.charAttrib.runState;
        this.psm = psm;
        input = pI;
    }
    Player player;
    PlayerStateMachine psm;
    PlayerInput input;
    Queue<Vector3> inputs;
    RunStateProperties rsp;
    public override void Enter()
    {
        inputs = new Queue<Vector3>();
        inputs.Enqueue(player.cameraRig.transform.rotation* input.lstick);
    }

    public override void Exit()
    {
        //Debug.Log("Exited RunState");
    }

    public override void HandleInput()
    {

        if (input.aButton)
        {
            psm.Change("jump");
            return;
        }

        if (input.lstick.sqrMagnitude < rsp.walkThreshold)
        {
            psm.Change("idle");
            return;
        }
        else if (input.lstick.sqrMagnitude < rsp.runThreshold)
        {
            psm.Change("walk");
            return;
        }


    }

    public override void Update()
    {
        if (!player.isGrounded)
        {
            psm.Change("fall");
            return;
        }

        Vector3 localInput = player.cameraRig.transform.rotation * input.lstick;
        localInput.y = 0;
        Quaternion currentRot = player.transform.rotation;
        Quaternion desiredRotation = player.transform.rotation;
        if (localInput != Vector3.zero)
            desiredRotation = Quaternion.LookRotation(localInput);

        player.transform.rotation = Quaternion.RotateTowards(currentRot, desiredRotation, rsp.turnRate);

        Vector3 accelDir = player.transform.forward * rsp.accel * localInput.magnitude;
        player.velocity += accelDir;
        player.velocity = Vector3.ClampMagnitude(player.velocity, rsp.maxRunSpeed) ;



        player.currentSpeed = player.velocity.magnitude;
        RaycastHit hit;
        Ray ray = new Ray(player.transform.position, -player.transform.up);
        Physics.Raycast(ray, out hit);
        player.velocity = Vector3.ProjectOnPlane(player.velocity, hit.normal);
        player.Ccontroller.Move(player.velocity * Time.deltaTime);
    }

    public override string ToString()
    {
        return "Run State";
    }
}
