public class PlayerMovingState : PlayerBaseState
{
    public PlayerMovingState(CharacterStateManager ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        _ctx.Animator.SetBool("IsMoving", true);
    }

    public override void UpdateState()
    {
        _ctx.PlayerMovement.HandleMovement();
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        if (!_ctx.IsMovementPressed)
        {
            _ctx.SwitchState(_factory.Idle());
        }
    }
}