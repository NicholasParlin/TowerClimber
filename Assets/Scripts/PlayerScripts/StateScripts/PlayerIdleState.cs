public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(CharacterStateManager ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        _ctx.Animator.SetBool("IsMoving", false);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsMovementPressed)
        {
            _ctx.SwitchState(_factory.Moving());
        }
    }
}