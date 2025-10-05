using UnityEngine;

public abstract class PlayerBaseState
{
    protected CharacterStateManager _ctx;
    protected PlayerStateFactory _factory;

    public PlayerBaseState(CharacterStateManager currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();
}