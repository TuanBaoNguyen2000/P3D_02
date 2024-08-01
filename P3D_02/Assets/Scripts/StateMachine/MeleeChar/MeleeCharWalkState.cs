using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCharWalkState : State
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
        direction = meleeChar.TargetMove.position - meleeChar.transform.position;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
     
        //if (isReached) 
        //{
        //    stateMachine.ChangeState(meleeChar.MeleeCharAttackState);
        //}
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        //meleeChar.velocity = direction.normalized;
    }
}
