public class EnemyStateFactory
{
    private EnemyStateManager _context;

    public EnemyStateFactory(EnemyStateManager currentContext)
    {
        _context = currentContext;
    }

    public EnemyBaseState Patrolling()
    {
        return new EnemyPatrollingState(_context, this);
    }

    public EnemyBaseState Chasing()
    {
        return new EnemyChasingState(_context, this);
    }

    public EnemyBaseState Attacking()
    {
        return new EnemyAttackingState(_context, this);
    }

    public EnemyBaseState Staggered()
    {
        return new EnemyStaggeredState(_context, this);
    }

    public EnemyBaseState KnockedDown()
    {
        return new EnemyKnockedDownState(_context, this);
    }

    public EnemyBaseState Dead()
    {
        return new EnemyDeadState(_context, this);
    }
}