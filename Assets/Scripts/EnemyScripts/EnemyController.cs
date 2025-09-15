using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

// This is the base AI controller for an enemy.
[RequireComponent(typeof(EnemyHealth), typeof(EnemySkillManager), typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private enum AIState { Patrolling, Chasing, Attacking }
    private AIState _currentState;

    [Header("AI Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackRange = 2f;

    // Component references
    private NavMeshAgent _agent;
    private EnemySkillManager _skillManager;
    private Transform _playerTransform;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _skillManager = GetComponent<EnemySkillManager>();
        // In a real game, you might have a more robust way to find the player.
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        // Simple state machine logic
        switch (_currentState)
        {
            case AIState.Patrolling:
                // TODO: Add logic for wandering around.
                // If player is close, switch to chasing.
                if (Vector3.Distance(transform.position, _playerTransform.position) < detectionRadius)
                {
                    _currentState = AIState.Chasing;
                }
                break;

            case AIState.Chasing:
                _agent.SetDestination(_playerTransform.position);
                // If we get in attack range, switch to attacking.
                if (Vector3.Distance(transform.position, _playerTransform.position) <= attackRange)
                {
                    _currentState = AIState.Attacking;
                }
                // If player gets too far away, go back to patrolling.
                if (Vector3.Distance(transform.position, _playerTransform.position) > detectionRadius)
                {
                    _currentState = AIState.Patrolling;
                }
                break;

            case AIState.Attacking:
                // Stop moving and face the player.
                _agent.SetDestination(transform.position);
                transform.LookAt(_playerTransform);

                // Here, the AI would decide whether to do a basic attack or use a skill.
                // This is where it communicates with the EnemySkillManager.
                // For example: _skillManager.UseSkill(someSkill);

                // If player moves out of attack range, go back to chasing.
                if (Vector3.Distance(transform.position, _playerTransform.position) > attackRange)
                {
                    _currentState = AIState.Chasing;
                }
                break;
        }
    }
}