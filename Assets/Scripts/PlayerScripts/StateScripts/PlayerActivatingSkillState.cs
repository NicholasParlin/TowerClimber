using UnityEngine;

public class PlayerActivatingSkillState : PlayerBaseState
{
    private float _activationTimer;

    public PlayerActivatingSkillState(CharacterStateManager ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        // This time would be set by the PlayerSkillManager before switching to this state
        _activationTimer = _ctx.CurrentActionTime;
    }

    public override void UpdateState()
    {
        _activationTimer -= Time.deltaTime;
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        // Once the activation time is up, return to idle.
        if (_activationTimer <= 0)
        {
            _ctx.SwitchState(_factory.Idle());
        }
    }
}