using UnityEngine;
using UnityEngine.AI;

// This is the base AI controller for an enemy. It manages states, movement, and actions.
[RequireComponent(typeof(EnemyHealth), typeof(EnemySkillManager), typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    // A simple state machine to define the enemy's current behavior.
    private enum AIState { Patrolling, Chasing, Attacking }
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

    // Variables for patrolling
    private Vector3 _startPosition;
    private float _timeToNextPatrol;

    private void Awake()
    {
        // Get references to all necessary components on this GameObject.
        _agent = GetComponent<NavMeshAgent>();
        _skillManager = GetComponent<EnemySkillManager>();
        _buffManager = GetComponent<BuffManager>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _stats = GetComponent<CharacterStatsBase>();

        // Find the player's transform. In a large game, a manager might provide this reference.
        _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Store the initial position for patrolling.
        _startPosition = transform.position;
    }

    private void Update()
    {
        if (_playerTransform == null) return; // Do nothing if there's no player.

        // --- Simple AI State Machine ---
        switch (_currentState)
        {
            case AIState.Patrolling:
                _timeToNextPatrol -= Time.deltaTime;
                if (_timeToNextPatrol <= 0)
                {
                    // Find a new random point within the patrol radius to wander to.
                    Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
                    randomDirection += _startPosition;
                    NavMeshHit navHit;
                    NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, -1);
                    _agent.SetDestination(navHit.position);
                    _timeToNextPatrol = patrolTimer;
                }

                // If the player enters our detection radius, start chasing them.
                if (Vector3.Distance(transform.position, _playerTransform.position) < detectionRadius)
                {
                    _currentState = AIState.Chasing;
                }
                break;

            case AIState.Chasing:
                // Set the NavMeshAgent's destination to the player's current position.
                _agent.SetDestination(_playerTransform.position);

                // If we get in attack range, switch to the attacking state.
                if (Vector3.Distance(transform.position, _playerTransform.position) <= attackRange)
                {
                    _currentState = AIState.Attacking;
                }
                // If the player gets too far away, give up and go back to patrolling.
                if (Vector3.Distance(transform.position, _playerTransform.position) > detectionRadius)
                {
                    _currentState = AIState.Patrolling;
                }
                break;

            case AIState.Attacking:
                // Stop moving and face the player to attack.
                _agent.SetDestination(transform.position);
                transform.LookAt(_playerTransform.position);

                // Here, a more complex AI would decide whether to do a basic attack or use a skill.
                // This is where it communicates with the EnemySkillManager.
                // For example: if (someCondition) { _skillManager.UseSkill(someSkill); }

                // If the player moves out of attack range, go back to chasing them.
                if (Vector3.Distance(transform.position, _playerTransform.position) > attackRange)
                {
                    _currentState = AIState.Chasing;
                }
                break;
        }
    }

    /// <summary>
    /// This is the public method called by the Object Pooler to reset the enemy's state
    /// each time it is reused.
    /// </summary>
    public void ResetEnemy()
    {
        // 1. Restore all resources to their maximum values.
        _stats.RestoreAllResources();

        // 2. Clear any lingering buffs or debuffs from the last life.
        if (_buffManager != null)
        {
            _buffManager.ClearAllModifiers();
        }

        // 3. Reset the health component's death flag.
        if (_enemyHealth != null)
        {
            _enemyHealth.ResetHealth();
        }

        // 4. Reset the AI to its default state.
        _currentState = AIState.Patrolling;
        _timeToNextPatrol = 0f; // Ensure it finds a new patrol point immediately.

        // 5. Ensure the NavMeshAgent is active and ready to move.
        if (_agent != null)
        {
            _agent.enabled = true;
        }
    }
}