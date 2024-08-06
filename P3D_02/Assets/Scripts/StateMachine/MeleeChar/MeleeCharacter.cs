using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeCharacter : MonoBehaviour
{
    NavMeshAgent agent;
    public NavMeshAgent Agent => this.agent;

    #region Animation Manager
    [SerializeField] Animator animator;
    public Animator Animator => this.animator;
    #endregion

    #region State Manager
    public MeleeCharStateMachine MeleeCharStateMachine;
    public MeleeCharWalkState MeleeCharWalkState;
    public MeleeCharAttackState MeleeCharAttackState;
    [SerializeField] float moveSpeed;
    [SerializeField] Transform targetMove;
    public float MoveSpeed => this.moveSpeed;
    public Transform TargetMove => this.targetMove;
    public bool IsReachTarget => Vector3.Distance(transform.position, targetMove.position) <= 1f;
    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        MeleeCharStateMachine = new MeleeCharStateMachine();
        MeleeCharWalkState = new MeleeCharWalkState(this);
        MeleeCharAttackState = new MeleeCharAttackState(this);

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
