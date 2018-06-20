using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine {

    public PlayerStateMachine(Player p, PlayerInput pI)
    {
        player = p;
        playerInput = pI;
    }

    Player player;
    PlayerInput playerInput;

    Dictionary<string, IPlayerState> states = new Dictionary<string, IPlayerState>();

    public IPlayerState previousState,currentState;

    public void AddState(string id,IPlayerState state)
    {
        states.Add(id, state);
    }

    public void Remove(string id)
    {
        states.Remove(id);
    }

    public void Change(string id)
    {
        currentState.Exit();
        currentState = states[id];
        currentState.Enter();
    }

    public void Set(string id)
    {
        currentState = states[id];
    }

    public void UpdateState()
    {
        currentState.Update();
    }

    public void HandleInput()
    {
        currentState.HandleInput();
    }

}
