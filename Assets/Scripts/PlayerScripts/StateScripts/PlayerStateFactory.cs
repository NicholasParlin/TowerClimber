public class PlayerStateFactory
{
    private CharacterStateManager _context;

    public PlayerStateFactory(CharacterStateManager currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Idle() => new PlayerIdleState(_context, this);
    public PlayerBaseState Moving() => new PlayerMovingState(_context, this);
    public PlayerBaseState ActivatingSkill() => new PlayerActivatingSkillState(_context, this);
    public PlayerBaseState Staggered() => new PlayerStaggeredState(_context, this);
    public PlayerBaseState KnockedDown() => new PlayerKnockedDownState(_context, this);
    public PlayerBaseState Countered() => new PlayerCounteredState(_context, this);
    public PlayerBaseState Dead() => new PlayerDeadState(_context, this);

    // NOTE: ActionInProgress and ChannelingSkill are often managed *within* the ActivatingSkill state 
    // or as more complex states that might take parameters (like which skill is being channeled).
    // For now, they are omitted as their logic depends heavily on specific skill implementations.
}