using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCharacter : MonoBehaviour
{

    [SerializeField] Transform targetMove;
    [SerializeField] float moveSpeed;
    public bool IsReachTarget => Vector3.Distance(transform.position, targetMove.position) <= 0.5f;
    public Transform TargetMove => this.targetMove;
    public float MoveSpeed => this.moveSpeed;

    public MeleeCharStateMachine MeleeCharStateMachine;
    public MeleeCharWalkState MeleeCharWalkState;
    public MeleeCharAttackState MeleeCharAttackState;


    void Start()
    {
        MeleeCharStateMachine = new MeleeCharStateMachine();
        MeleeCharWalkState = new MeleeCharWalkState(this, MeleeCharStateMachine);
        MeleeCharAttackState = new MeleeCharAttackState();

        MeleeCharStateMachine.Initialize(MeleeCharWalkState);
    }

    void Update()
    {
        transform.position += new Vector3(0, 0, -0.1f);
        MeleeCharStateMachine.currentState?.LogicUpdate();
    }

    void FixedUpdate()
    {
        MeleeCharStateMachine.currentState?.PhysicsUpdate();
    }
}
