using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IPlayerState
{
    public JumpState(Player p, PlayerStateMachine psm, PlayerInput input)
    {
        this.player = p;
        jsp = player.charAttrib.jumpState;
        this.psm = psm;
        this.input = input;
    }

    Player player;
    JumpStateProperties jsp;
    PlayerStateMachine psm;
    PlayerInput input;
    public override void Enter()
    {
        player.velocity.y = jsp.initialJumpForce;
        player.Ccontroller.Move(player.velocity * Time.deltaTime);

    }

    public override void Exit()
    {
        jsp.jumpTimer = 0;
    }

    public override void HandleInput()
    {
        if (input.aButton && jsp.jumpTimer < jsp.jumpDuration)
        {
            player.velocity.y = jsp.jumpForce;
            jsp.jumpTimer += Time.deltaTime;
        }
        else if(jsp.jumpTimer>= jsp.jumpDuration || !input.aButton)
        {
            player.velocity.y = 0;
            jsp.jumpTimer = jsp.jumpDuration;
            psm.Change("fall");
            return;
        }
        if (input.bButton)
        {
            psm.Change("BoostFall");
        }
    }

    public override void Update()
    {
        player.Ccontroller.Move(player.velocity * Time.deltaTime);

        if (player.isOnRail() && player.canGrind())
        {
            Debug.Log("ON RAILS!");
            psm.Change("rail");
            return;
        }
    }
}
