using UnityEngine;

public class EnemyStaggeredState : EnemyBaseState
{
    public EnemyStaggeredState(EnemyStateManager ctx, EnemyStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        _ctx.Agent.isStopped = true;
        _ctx.Animator.SetTrigger("StaggerTrigger");

        // Note: An Animation Event on the stagger animation clip will call 
        // a method on the EnemyStateManager to transition out of this state.
    }

    public override void UpdateState()
    {
        // Player has no control in this state.
    }

    public override void ExitState()
    {
        _ctx.Agent.isStopped = false;
    }

    // MODIFIED: Implemented the required abstract method.
    public override void CheckSwitchStates()
    {
        // This state is exited via an animation event, so there's nothing to check here.
        // It can be interrupted by a more severe state like KnockedDown, but that
        // logic is handled in the script that *causes* the state change (e.g., CharacterStatsBase).
    }
}