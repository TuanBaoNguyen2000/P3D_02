using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCharAttackState : IState
{
    public void Enter()
    {
        Debug.Log("Enter Attack State");
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void LogicUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void PhysicsUpdate()
    {
        throw new System.NotImplementedException();
    }
}
