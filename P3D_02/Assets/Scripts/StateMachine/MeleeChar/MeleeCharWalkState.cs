using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCharWalkState : State
{
    MeleeCharacter meleeChar;
    MeleeCharStateMachine stateMachine;
    bool isReached;


    public MeleeCharWalkState(MeleeCharacter meleeChar, MeleeCharStateMachine stateMachine)
    {
        this.meleeChar = meleeChar;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit() 
    { 
        base.Exit();
    }
    public override void DoChecks()
    {
        base.DoChecks();
        isReached = meleeChar.IsReachTarget;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        meleeChar.transform.position = Vector3.MoveTowards(meleeChar.transform.position, meleeChar.TargetMove.position, meleeChar.MoveSpeed * 3.5f * Time.deltaTime);
     
        if (isReached) 
        {
            stateMachine.ChangeState(meleeChar.MeleeCharAttackState);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
