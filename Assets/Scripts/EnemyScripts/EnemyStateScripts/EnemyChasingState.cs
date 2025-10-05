using UnityEngine;

public class EnemyChasingState : EnemyBaseState
{
    public EnemyChasingState(EnemyStateManager ctx, EnemyStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        // Optional: Play an alert sound or animation when the enemy starts chasing.
    }

    public override void UpdateState()
    {
        // The core logic of this state is to always move towards the player.
        _ctx.Agent.SetDestination(_ctx.PlayerTransform.position);

        // Then, check if we should switch to a different state.
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        // Stop the agent when exiting this state to prevent sliding.
        _ctx.Agent.ResetPath();
    }

    // MODIFIED: Implemented the required abstract method.
    public override void CheckSwitchStates()
    {
        float distanceToPlayer = Vector3.Distance(_ctx.transform.position, _ctx.PlayerTransform.position);

        // Transition to Patrolling if the player gets too far away.
        if (distanceToPlayer > _ctx.DetectionRadius)
        {
            _ctx.SwitchState(_factory.Patrolling());
        }
        // Transition to Attacking if the player is in range.
        else if (distanceToPlayer <= _ctx.AttackRange)
        {
            _ctx.SwitchState(_factory.Attacking());
        }
    }
}