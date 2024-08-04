using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCharWalkState : IState
{
    MeleeCharacter meleeChar;
    MeleeCharStateMachine stateMachine;
    bool isReached;

    Vector3 direction;

    public MeleeCharWalkState(MeleeCharacter meleeChar, MeleeCharStateMachine stateMachine)
    {
        this.meleeChar = meleeChar;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Enter Walk State");
    }
    public void Exit() 
    { 

    }

    public void DoChecks()
    {
        isReached = meleeChar.IsReachTarget;
        //direction = meleeChar.TargetMove.position - meleeChar.transform.position;
    }
    public void LogicUpdate()
    {
        if (isReached)
            stateMachine.ChangeState(meleeChar.MeleeCharAttackState);
        else 
            meleeChar.Agent.SetDestination(meleeChar.TargetMove.position);
    }
    public void PhysicsUpdate()
    {
        DoChecks();
    }
}
