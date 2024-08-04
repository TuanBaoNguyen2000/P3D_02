using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeCharacter : MonoBehaviour
{

    [SerializeField] Transform targetMove;
    [SerializeField] float moveSpeed;

    NavMeshAgent agent;

    public NavMeshAgent Agent => this.agent;
    public bool IsReachTarget => Vector3.Distance(transform.position, targetMove.position) <= 1f;
    public Transform TargetMove => this.targetMove;
    public float MoveSpeed => this.moveSpeed;

    public MeleeCharStateMachine MeleeCharStateMachine;
    public MeleeCharWalkState MeleeCharWalkState;
    public MeleeCharAttackState MeleeCharAttackState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

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
    }
}
