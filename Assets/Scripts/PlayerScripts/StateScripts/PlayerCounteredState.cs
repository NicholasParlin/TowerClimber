public class PlayerCounteredState : PlayerBaseState
{
    public PlayerCounteredState(CharacterStateManager ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        _ctx.Animator.SetTrigger("CounteredTrigger");
        // This animation should also have an event calling "ReturnToIdleState".
    }

    public override void UpdateState() { } // Player has no control
    public override void ExitState() { }
    public override void CheckSwitchStates() { }
}