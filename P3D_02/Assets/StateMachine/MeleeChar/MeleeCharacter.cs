using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeCharacter : MonoBehaviour
{
    NavMeshAgent agent;
    public NavMeshAgent Agent => this.agent;

    #region Entity Data
    [SerializeField] EntityDataSO entityDataSO;
    public EntityDataSO DataSO => this.entityDataSO;

    public int level = 0;
    public float curMaxHP, curATK, curATKSpeed;
    public float curCritChance, curCritDMG;
    public float curDEF, curDodgeRate;

    public float curHP;
    #endregion

    #region Animation Manager
    [SerializeField] Animator animator;
    [SerializeField] AnimationEventMN animationEvents;
    public Animator Animator => this.animator;
    public AnimationEventMN AnimationEvents => this.animationEvents;
    #endregion

    public Collider hitboxCol;
    #region State Manager
    public MeleeCharStateMachine MeleeCharStateMachine;
    public MeleeCharWalkState MeleeCharWalkState;
    public MeleeCharAttackState MeleeCharAttackState;
    [SerializeField] float moveSpeed;
    [SerializeField] Transform targetMove;
    public float MoveSpeed => this.moveSpeed;
    public Transform TargetMove => this.targetMove;
    public bool IsReachTarget => Vector3.Distance(transform.position, targetMove.position) <= 2f;
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

        level = 1;
        InitData();
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

    private void InitData()
    {
        curMaxHP = DataSO.baseMaxHP * level;
        curATK = DataSO.baseATK * level;
        curATKSpeed = DataSO.baseATKSpeed * level;
        curCritChance = DataSO.baseCritChance * level;
        curCritDMG = DataSO.baseCritDMG * level;
        curDEF = DataSO.baseDEF * level;
        curDodgeRate = DataSO.baseDodgeRate * level;

        curHP = curMaxHP;
    }
}
