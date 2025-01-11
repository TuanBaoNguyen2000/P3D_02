using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCharWalkState : IState
{
    MeleeCharacter meleeChar;
    MeleeCharStateMachine StateMachine => this.meleeChar.MeleeCharStateMachine;
    bool IsReached => this.meleeChar.IsReachTarget;

    public MeleeCharWalkState(MeleeCharacter meleeChar)
    {
        this.meleeChar = meleeChar;
    }

    public void Enter()
    {
        meleeChar.Animator.SetBool("IsWalk", true);
    }

    public void Exit() 
    {
        meleeChar.Animator.SetBool("IsWalk", false);
    }

    public void LogicUpdate()
    {
        if (IsReached)
            StateMachine.ChangeState(meleeChar.MeleeCharAttackState);
        else 
            meleeChar.Agent.SetDestination(meleeChar.TargetMove.position);
    }
    public void PhysicsUpdate()
    {
    }
}
