using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(EnemyController))] // NEW: Require the controller
public class EnemySkillManager : SkillManagerBase
{
    [Header("Enemy Skill Configuration")]
    [SerializeField] private EnemySkillSet skillSet;
    [Tooltip("How often (in seconds) the AI should re-evaluate its best action.")]
    [SerializeField] private float decisionInterval = 1.0f;

    private GameObject _playerTarget;
    private EnemyController _controller; // Reference to the controller for context

    // This class will hold a potential action and its calculated score.
    private class ScoredAction
    {
        public Skill Skill { get; set; }
        public float Score { get; set; }
    }

    protected override void Awake()
    {
        base.Awake();
        _controller = GetComponent<EnemyController>();

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

    private void OnEnable()
    {
        StartCoroutine(DecisionCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // The main AI decision loop
    private IEnumerator DecisionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(decisionInterval);
            if (_controller.CanMakeDecision())
            {
                DecideNextAction();
            }
        }
    }

    // The core of the Utility AI system
    private void DecideNextAction()
    {
        if (learnedSkills.Count == 0 || _playerTarget == null) return;

        List<ScoredAction> scoredActions = new List<ScoredAction>();

        // Loop through every learned skill to score it
        foreach (var skillList in learnedSkills.Values)
        {
            foreach (Skill skill in skillList)
            {
                // First, check if the skill can even be used (cooldown, resources)
                if (!CanUseSkill(skill)) continue;

                float currentScore = skill.baseUtilityScore;
                float scoreModifier = 1f;

                // Multiply the score by the result of each consideration's curve
                foreach (AIAction consideration in skill.aiActions)
                {
                    scoreModifier *= consideration.Score(_controller);
                }

                currentScore *= scoreModifier;
                scoredActions.Add(new ScoredAction { Skill = skill, Score = currentScore });
            }
        }

        // If no actions are viable, do nothing
        if (scoredActions.Count == 0)
        {
            _controller.SetNextAction(null);
            return;
        }

        // Find the action with the highest score
        ScoredAction bestAction = scoredActions.OrderByDescending(a => a.Score).First();

        // Tell the controller what the best action is
        _controller.SetNextAction(bestAction.Skill);
    }

    // A public method for the controller to execute the chosen skill
    public void ExecuteSkill(Skill skill)
    {
        if (skill != null && _playerTarget != null)
        {
            TryToUseSkill(skill, _playerTarget);
        }
    }
}