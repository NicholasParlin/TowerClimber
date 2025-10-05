using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(EnemyStateManager))] // MODIFIED: Changed from EnemyController
public class EnemySkillManager : SkillManagerBase
{
    [Header("Enemy Skill Configuration")]
    [SerializeField] private EnemySkillSet skillSet;

    // REMOVED: The decision interval is no longer needed here.
    // [SerializeField] private float decisionInterval = 1.0f;

    private GameObject _playerTarget;
    private EnemyStateManager _controller; // MODIFIED: Changed from EnemyController

    private class ScoredAction
    {
        public Skill Skill { get; set; }
        public float Score { get; set; }
    }

    protected override void Awake()
    {
        base.Awake();
        _controller = GetComponent<EnemyStateManager>(); // MODIFIED: Get the new state manager

        if (GameManager.Instance != null && GameManager.Instance.PlayerStats != null)
        {
            _playerTarget = GameManager.Instance.PlayerStats.gameObject;
        }
        else
        {
            Debug.LogError("GameManager or Player not found! Enemy Skill Manager will not function.", this);
        }

        if (skillSet != null)
        {
            foreach (Skill skill in skillSet.skills)
            {
                if (skill != null) LearnNewSkill(skill);
            }
        }
    }

    // REMOVED: The OnEnable/OnDisable and DecisionCoroutine are no longer needed. The state machine drives the logic now.

    /// <summary>
    /// The core of the Utility AI system. This is now called by the EnemyAttackingState.
    /// </summary>
    public void DecideNextAction()
    {
        if (learnedSkills.Count == 0 || _playerTarget == null) return;

        List<ScoredAction> scoredActions = new List<ScoredAction>();

        foreach (var skillList in learnedSkills.Values)
        {
            foreach (Skill skill in skillList)
            {
                if (!CanUseSkill(skill)) continue;

                float currentScore = skill.baseUtilityScore;
                float scoreModifier = 1f;

                foreach (AIAction consideration in skill.aiActions)
                {
                    // The consideration now gets context from the EnemyStateManager
                    // You will need to update your AIAction script to take EnemyStateManager
                    // For now, we assume it works with the old EnemyController reference
                    // scoreModifier *= consideration.Score(_controller); 
                }

                currentScore *= scoreModifier;
                scoredActions.Add(new ScoredAction { Skill = skill, Score = currentScore });
            }
        }

        if (scoredActions.Count == 0)
        {
            _controller.SetNextSkill(null);
            return;
        }

        ScoredAction bestAction = scoredActions.OrderByDescending(a => a.Score).First();

        // Inform the state manager of the best action. The attacking state will handle execution.
        _controller.SetNextSkill(bestAction.Skill);
    }

    /// <summary>
    /// A simple method for the Attacking state to call to execute the chosen skill.
    /// </summary>
    public void ExecuteSkill(Skill skill)
    {
        if (skill != null && _playerTarget != null)
        {
            TryToUseSkill(skill, _playerTarget);
        }
    }
}