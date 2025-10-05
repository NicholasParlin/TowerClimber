public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(CharacterStateManager ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        _ctx.Animator.SetTrigger("DeathTrigger");
    }

    public override void UpdateState() { } // No actions can be taken
    public override void ExitState() { }
    public override void CheckSwitchStates() { } // Cannot exit this state
}