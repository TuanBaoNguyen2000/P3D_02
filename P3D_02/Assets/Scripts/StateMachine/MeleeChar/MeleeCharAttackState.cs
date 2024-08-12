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

    int attackComboLimit = 0;
    float timer = 0;
    float attackSpeed;

    public MeleeCharAttackState(MeleeCharacter meleeChar)
    {
        this.meleeChar = meleeChar;
    }

    public void Enter()
    {
        Debug.Log("Enter Attack State");
        attackComboLimit = 1;
        attackSpeed = 2f;
        meleeChar.Animator.SetInteger("AttackComboLimit", attackComboLimit);

        hitboxCollider = meleeChar.hitboxCol;
        Attack();
        meleeChar.AnimationEvents.OnAnimationFinish += OnAttackAnimFinsh;
        meleeChar.AnimationEvents.OnCustomEvent += OnCauseDamageEvent;
    }

    public void Exit()
    {
        Debug.Log("Exit Attack State");
        meleeChar.Animator.SetBool("IsAttack", false);
        meleeChar.AnimationEvents.OnAnimationFinish -= OnAttackAnimFinsh;
        meleeChar.AnimationEvents.OnCustomEvent -= OnCauseDamageEvent;
    }

    void OnAttackAnimFinsh()
    {
        meleeChar.Animator.SetBool("IsAttack", false);
    }

    void OnCauseDamageEvent(string eventName)
    {
        if (eventName != "CauseDamge") return;
        CauseDamage();
    }

    public void LogicUpdate()
    {
        if (timer < attackSpeed)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Attack();
            timer = 0;
        }
    }

    public void PhysicsUpdate()
    {

    }

    private void Attack()
    {
        meleeChar.Animator.SetBool("IsAttack", true);
    }

    private void CauseDamage()
    {
        Debug.Log("CauseDamage");
        collidersDamaged.Clear();
        Collider[] collidersToDamage = new Collider[10];
        LayerMask enemyLayerMask = 6;

        Vector3 hitboxSize = hitboxCollider.bounds.size;
        Vector3 hitboxCenter = hitboxCollider.bounds.center;

        int colliderCount = Physics.OverlapBoxNonAlloc(hitboxCenter, hitboxSize / 2, collidersToDamage, hitboxCollider.transform.rotation, enemyLayerMask);

        Debug.Log(colliderCount);
        for (int i = 0; i < colliderCount; i++)
        {
            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                if (collidersToDamage[i].TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    damageable.TakeDamage(meleeChar.curATK, 0);
                }
                collidersDamaged.Add(collidersToDamage[i]);
            }
        }
    }

}
