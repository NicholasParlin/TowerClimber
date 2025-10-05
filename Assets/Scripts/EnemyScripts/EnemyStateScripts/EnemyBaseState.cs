using UnityEngine;

public abstract class EnemyBaseState
{
    protected EnemyStateManager _ctx;
    protected EnemyStateFactory _factory;

    public EnemyBaseState(EnemyStateManager currentContext, EnemyStateFactory enemyStateFactory)
    {
        _ctx = currentContext;
        _factory = enemyStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    // NEW: Add this required abstract method.
    public abstract void CheckSwitchStates();
}