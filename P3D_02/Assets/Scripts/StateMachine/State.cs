using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{

    protected bool isAnimationFinished;
    protected bool isExitingState;

    string animBoolName;
    public virtual void Enter()
    {
        DoChecks();
    }

    public virtual void Exit()
    {
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks() { }

    public virtual void AnimationTrigger() { }

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;



}

