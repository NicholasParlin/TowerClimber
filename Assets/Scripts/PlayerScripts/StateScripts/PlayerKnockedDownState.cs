public class PlayerKnockedDownState : PlayerBaseState
{
    public PlayerKnockedDownState(CharacterStateManager ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        _ctx.Animator.SetTrigger("KnockdownTrigger");
        // This animation should also have an event calling "ReturnToIdleState" at its end.
    }

    public override void UpdateState() { } // Player has no control
    public override void ExitState() { }
    public override void CheckSwitchStates() { } // Cannot be interrupted
}