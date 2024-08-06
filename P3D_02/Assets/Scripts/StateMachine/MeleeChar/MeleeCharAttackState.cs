using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCharAttackState : IState
{
    MeleeCharacter meleeChar;
    MeleeCharStateMachine StateMachine => this.meleeChar.MeleeCharStateMachine;
    public MeleeCharAttackState(MeleeCharacter meleeChar)
    {
        this.meleeChar = meleeChar;
    }

    public void Enter()
    {
        Debug.Log("Enter Attack State");
    }

    public void Exit()
    {
        Debug.Log("Exit Attack State");

    }

    public void LogicUpdate()
    {
        Debug.Log("LogicUpdate Attack State");

    }

    public void PhysicsUpdate()
    {
        Debug.Log("PhysicsUpdate Attack State");
    }
}
