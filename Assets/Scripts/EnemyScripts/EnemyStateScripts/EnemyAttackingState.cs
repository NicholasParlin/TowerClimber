using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
    private float _actionTimer;
    private bool _hasChosenSkill;

    public EnemyAttackingState(EnemyStateManager ctx, EnemyStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        // Stop the agent and make it face the player
        _ctx.Agent.ResetPath();
        _ctx.transform.LookAt(_ctx.PlayerTransform.position);

        // Reset state variables
        _actionTimer = 0f;
        _hasChosenSkill = false;

        // Ask the Skill Manager (Utility AI) to decide on the best action
        _ctx.SkillManager.DecideNextAction();
    }

    public override void UpdateState()
    {
        // If an action is in progress, count down the timer
        if (_hasChosenSkill)
        {
            _actionTimer -= Time.deltaTime;
        }
        else if (_ctx.NextSkillToUse != null)
        {
            // The Skill Manager has chosen a skill, let's execute it.
            ExecuteSkill(_ctx.NextSkillToUse);
        }

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        // Clear the chosen skill when leaving the state
        _ctx.SetNextSkill(null);
    }

    public override void CheckSwitchStates()
    {
        // If the action is finished, go back to chasing
        if (_hasChosenSkill && _actionTimer <= 0)
        {
            _ctx.SwitchState(_factory.Chasing());
            return;
        }

        // If the player moves out of attack range BEFORE a skill has been executed, go back to chasing.
        if (!_hasChosenSkill && Vector3.Distance(_ctx.transform.position, _ctx.PlayerTransform.position) > _ctx.AttackRange)
        {
            _ctx.SwitchState(_factory.Chasing());
        }
    }

    private void ExecuteSkill(Skill skill)
    {
        _hasChosenSkill = true;
        _actionTimer = skill.baseActivationTime; // Set timer for action lock
        _ctx.SkillManager.ExecuteSkill(skill); // Tell the skill manager to perform the skill
    }
}