using UnityEngine;

public class EnemyKnockedDownState : EnemyBaseState
{
    public EnemyKnockedDownState(EnemyStateManager ctx, EnemyStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        _ctx.Agent.isStopped = true;
        _ctx.Animator.SetTrigger("KnockdownTrigger");

        // Note: Your knockdown animation should include the "getting up" part.
        // Place an Animation Event at the very end of this animation to call 
        // a method like "ReturnToChasingState" on the EnemyStateManager.
    }

    // The enemy has no control while knocked down.
    public override void UpdateState() { }

    public override void ExitState()
    {
        _ctx.Agent.isStopped = false;
    }

    // This is a "hard" CC state and cannot be interrupted.
    public override void CheckSwitchStates() { }
}