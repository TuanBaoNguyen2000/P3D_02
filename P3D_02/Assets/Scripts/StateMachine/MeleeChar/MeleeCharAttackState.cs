using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.GraphicsBuffer;

public class MeleeCharAttackState : IState
{
    MeleeCharacter meleeChar;
    MeleeCharStateMachine StateMachine => this.meleeChar.MeleeCharStateMachine;

    Collider hitboxCollider;
    List<Collider> collidersDamaged = new List<Collider>();

    //IDamageable target;

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

    protected void Attack()
    {
        collidersDamaged.Clear();
        Collider[] collidersToDamage = new Collider[10];
        LayerMask enemyLayerMask = 0;

        Vector3 hitboxSize = hitboxCollider.bounds.size;
        Vector3 hitboxCenter = hitboxCollider.bounds.center;

        int colliderCount = Physics.OverlapBoxNonAlloc(hitboxCenter, hitboxSize / 2, collidersToDamage, hitboxCollider.transform.rotation, enemyLayerMask);

        for (int i = 0; i < colliderCount; i++)
        {
            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                if (collidersToDamage[i].TryGetComponent<IDamageable>(out IDamageable damageable))
                {

                }
                collidersDamaged.Add(collidersToDamage[i]);
            }
        }
    }

}
