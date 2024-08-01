using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCharacter : MonoBehaviour
{

    [SerializeField] Transform targetMove;
    [SerializeField] float moveSpeed;

    public Vector3 velocity;
    public bool IsReachTarget => Vector3.Distance(transform.position, targetMove.position) <= 1f;
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
        MeleeCharStateMachine.currentState?.LogicUpdate();
    }

    void FixedUpdate()
    {
        MeleeCharStateMachine.currentState?.PhysicsUpdate();
        velocity += new Vector3(0, -0.1f, 0);
        transform.position = Vector3.MoveTowards(transform.position, velocity, 3.5f * Time.fixedDeltaTime);
    }
}
