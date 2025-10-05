using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemySkillManager))]
[RequireComponent(typeof(CharacterStatsBase))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyStateManager : MonoBehaviour
{
    // --- State Machine ---
    private EnemyBaseState _currentState;
    private EnemyStateFactory _states;

    // --- Properties for States to Access (Context) ---
    public NavMeshAgent Agent { get; private set; }
    public Animator Animator { get; private set; }
    public EnemySkillManager SkillManager { get; private set; }
    public EnemyHealth EnemyHealth { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public Skill NextSkillToUse { get; private set; }
    public Vector3 StartPosition { get; private set; }

    [Header("AI Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float patrolRadius = 15f;
    [SerializeField] private float patrolTimerDuration = 5f;

    // --- Public Getters for State Logic ---
    public float DetectionRadius => detectionRadius;
    public float AttackRange => attackRange;
    public float PatrolRadius => patrolRadius;
    public float PatrolTimerDuration => patrolTimerDuration;


    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        SkillManager = GetComponent<EnemySkillManager>();
        EnemyHealth = GetComponent<EnemyHealth>();

        if (GameManager.Instance != null && GameManager.Instance.PlayerStats != null)
        {
            PlayerTransform = GameManager.Instance.PlayerStats.transform;
        }
        else
        {
            Debug.LogError("GameManager or Player not found! Enemy AI will not function.", this);
            this.enabled = false;
        }

        StartPosition = transform.position;

        _states = new EnemyStateFactory(this);
        _currentState = _states.Patrolling();
        _currentState.EnterState();
    }

    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.UpdateState();
        }

        // NEW: Add this block to sync the NavMeshAgent's speed with the Animator.
        // We check the magnitude of the agent's velocity. A value greater than a small
        // threshold (like 0.1) means the agent is moving.
        bool isMoving = Agent.velocity.magnitude > 0.1f;
        Animator.SetBool("IsMoving", isMoving);
    }

    public void SwitchState(EnemyBaseState newState)
    {
        if (_currentState != null)
        {
            _currentState.ExitState();
        }
        _currentState = newState;
        _currentState.EnterState();
    }

    public void SetNextSkill(Skill skill)
    {
        NextSkillToUse = skill;
    }

    public void ReturnToChasingState()
    {
        SwitchState(_states.Chasing());
    }
}