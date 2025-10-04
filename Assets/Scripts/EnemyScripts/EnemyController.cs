using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyHealth), typeof(EnemySkillManager), typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    // A state machine to define the enemy's current behavior.
    private enum AIState { Patrolling, Chasing, Attacking, ExecutingAction }
    private AIState _currentState;

    [Header("AI Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float patrolRadius = 15f;
    [SerializeField] private float patrolTimer = 5f;

    // Component references
    private NavMeshAgent _agent;
    private EnemySkillManager _skillManager;
    private BuffManager _buffManager;
    private EnemyHealth _enemyHealth;
    private CharacterStatsBase _stats;
    private Transform _playerTransform;

    // Variables for AI logic
    private Vector3 _startPosition;
    private float _timeToNextPatrol;
    private Skill _nextSkillToUse; // The skill chosen by the Skill Manager

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _skillManager = GetComponent<EnemySkillManager>();
        _buffManager = GetComponent<BuffManager>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _stats = GetComponent<CharacterStatsBase>();

        if (GameManager.Instance != null && GameManager.Instance.PlayerStats != null)
        {
            _playerTransform = GameManager.Instance.PlayerStats.transform;
        }
        else
        {
            Debug.LogError("GameManager or Player not found! Enemy AI will not function.", this);
            this.enabled = false;
        }
        _startPosition = transform.position;
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        // --- AI State Machine ---
        switch (_currentState)
        {
            case AIState.Patrolling:
                HandlePatrolling();
                break;
            case AIState.Chasing:
                HandleChasing();
                break;
            case AIState.Attacking:
                HandleAttacking();
                break;
            case AIState.ExecutingAction:
                // While executing an action (like a skill animation), the AI does nothing else.
                // A more advanced system would use animation events to return to another state.
                // For now, we'll use a simple timer based on the skill's activation time.
                break;
        }
    }

    private void HandlePatrolling()
    {
        _timeToNextPatrol -= Time.deltaTime;
        if (_timeToNextPatrol <= 0)
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += _startPosition;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, -1);
            _agent.SetDestination(navHit.position);
            _timeToNextPatrol = patrolTimer;
        }

        if (Vector3.Distance(transform.position, _playerTransform.position) < detectionRadius)
        {
            _currentState = AIState.Chasing;
        }
    }

    private void HandleChasing()
    {
        _agent.SetDestination(_playerTransform.position);

        if (Vector3.Distance(transform.position, _playerTransform.position) <= attackRange)
        {
            _currentState = AIState.Attacking;
        }
        if (Vector3.Distance(transform.position, _playerTransform.position) > detectionRadius)
        {
            _currentState = AIState.Patrolling;
        }
    }

    private void HandleAttacking()
    {
        _agent.SetDestination(transform.position);
        transform.LookAt(_playerTransform.position);

        if (_nextSkillToUse != null)
        {
            _skillManager.ExecuteSkill(_nextSkillToUse);
            _currentState = AIState.ExecutingAction;
            // A simple way to "lock" the AI during the action
            Invoke(nameof(ActionComplete), _nextSkillToUse.baseActivationTime);
            _nextSkillToUse = null; // Clear the chosen action
        }

        if (Vector3.Distance(transform.position, _playerTransform.position) > attackRange)
        {
            _currentState = AIState.Chasing;
        }
    }

    private void ActionComplete()
    {
        if (_currentState == AIState.ExecutingAction)
        {
            _currentState = AIState.Chasing; // Or Attacking, depending on desired behavior
        }
    }

    // --- Public Methods for Skill Manager ---

    public void SetNextAction(Skill skill)
    {
        _nextSkillToUse = skill;
    }

    public bool CanMakeDecision()
    {
        // The AI can only make a new decision if it's not already busy.
        return _currentState == AIState.Chasing || _currentState == AIState.Attacking;
    }

    // --- Public Methods for Contextual Scoring ---

    public float GetHealthPercentage() => _stats.currentHealth / _stats.maxHealth;
    public float GetDistanceToTarget() => Vector3.Distance(transform.position, _playerTransform.position);
    public float GetDetectionRadius() => detectionRadius;

    /// <summary>
    /// This is the public method called by the Object Pooler to reset the enemy's state
    /// each time it is reused.
    /// </summary>
    public void ResetEnemy()
    {
        if (_stats != null)
        {
            _stats.RestoreAllResources();
        }

        if (_buffManager != null)
        {
            _buffManager.ClearAllModifiers();
        }

        if (_enemyHealth != null)
        {
            _enemyHealth.ResetHealth();
        }

        _currentState = AIState.Patrolling;
        _timeToNextPatrol = 0f;

        if (_agent != null)
        {
            _agent.enabled = true;
        }
    }
}