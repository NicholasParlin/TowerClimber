using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrollingState : EnemyBaseState
{
    private float _patrolTimer;

    public EnemyPatrollingState(EnemyStateManager ctx, EnemyStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        _patrolTimer = 0f;
    }

    public override void UpdateState()
    {
        // Handle patrol movement
        _patrolTimer -= Time.deltaTime;
        if (_patrolTimer <= 0f)
        {
            _patrolTimer = _ctx.PatrolTimerDuration;
            Vector3 randomDirection = Random.insideUnitSphere * _ctx.PatrolRadius;
            randomDirection += _ctx.StartPosition;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, _ctx.PatrolRadius, -1);
            _ctx.Agent.SetDestination(navHit.position);
        }

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _ctx.Agent.ResetPath();
    }

    // MODIFIED: Implemented the required abstract method.
    public override void CheckSwitchStates()
    {
        // If player is within detection radius, switch to chasing.
        if (Vector3.Distance(_ctx.transform.position, _ctx.PlayerTransform.position) < _ctx.DetectionRadius)
        {
            _ctx.SwitchState(_factory.Chasing());
        }
    }
}