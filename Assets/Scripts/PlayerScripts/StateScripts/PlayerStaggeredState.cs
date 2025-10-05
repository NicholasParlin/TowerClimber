public class PlayerStaggeredState : PlayerBaseState
{
    public PlayerStaggeredState(CharacterStateManager ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        _ctx.Animator.SetTrigger("StaggerTrigger");
        // The animation clip itself should have an Animation Event that calls
        // the "ReturnToIdleState" method on the CharacterStateManager.
    }

    public override void UpdateState()
    {
        // In this state, the player has no control. We wait for the animation to finish.
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        // This state can be interrupted by a more severe state, like being knocked down.
        // Logic for this would be added here if needed.
    }
}