using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
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

}

