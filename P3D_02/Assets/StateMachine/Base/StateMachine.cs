using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine
{
    public IState currentState { get; private set; }
    public void Initialize(IState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(IState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
