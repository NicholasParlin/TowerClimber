using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    public EnemyDeadState(EnemyStateManager ctx, EnemyStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        // Disable the agent and trigger the death animation
        _ctx.Agent.enabled = false;
        _ctx.Animator.SetTrigger("DeathTrigger");

        // Call the Die method on the EnemyHealth component to handle loot, quest events, and object pooling.
        _ctx.EnemyHealth.Die();
    }

    // The enemy is dead, so it does nothing in Update.
    public override void UpdateState() { }

    // The enemy cannot exit the dead state.
    public override void ExitState() { }

    // The enemy cannot transition from the dead state.
    public override void CheckSwitchStates() { }
}