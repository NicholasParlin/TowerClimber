using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for both Player and Enemy skill managers.
/// Handles the core logic for learning, activating, and managing cooldowns for skills.
/// </summary>
public abstract class SkillManagerBase : MonoBehaviour
{
    // A dictionary to hold learned skills, sorted by their archetype for easy access.
    public Dictionary<Archetype, List<Skill>> learnedSkills { get; protected set; } = new Dictionary<Archetype, List<Skill>>();

    // A dictionary to track the current cooldown time for each skill.
    protected Dictionary<Skill, float> skillCooldowns = new Dictionary<Skill, float>();

    // Variables to handle activation time, preventing skill spamming.
    protected bool _isActivating;
    protected float _currentActivationTime;

    // References to other essential components on this character.
    protected CharacterStatsBase _stats;

    protected virtual void Awake()
    {
        _stats = GetComponent<CharacterStatsBase>();
    }

    protected virtual void Update()
    {
        HandleCooldowns();
        HandleActivationLock();
    }

    /// <summary>
    /// Adds a new skill to the character's learned skills during gameplay.
    /// </summary>
    public bool LearnNewSkill(Skill skillToLearn)
    {
        if (skillToLearn == null) return false;

        if (!learnedSkills.ContainsKey(skillToLearn.archetype))
        {
            learnedSkills[skillToLearn.archetype] = new List<Skill>();
        }

        if (!learnedSkills[skillToLearn.archetype].Contains(skillToLearn))
        {
            learnedSkills[skillToLearn.archetype].Add(skillToLearn);
            Debug.Log($"{gameObject.name} learned a new skill: {skillToLearn.skillName}");
            return true;
        }
        return false;
    }

    /// <summary>
    /// This is the main method that child classes will call. 
    /// It's marked 'virtual' so it can be overridden.
    /// </summary>
    protected virtual void TryToUseSkill(Skill skill)
    {
        if (CanUseSkill(skill))
        {
            ActivateSkill(skill);
        }
    }

    /// <summary>
    /// Checks all conditions to see if a skill can be used.
    /// </summary>
    /// <returns>True if the skill can be activated.</returns>
    protected bool CanUseSkill(Skill skill)
    {
        if (skill == null) return false;
        if (_isActivating) return false;
        if (skillCooldowns.ContainsKey(skill)) return false;
        if (!HasEnoughResources(skill)) return false;
        return true;
    }

    /// <summary>
    /// Spends resources and triggers the skill's activation logic and cooldowns.
    /// </summary>
    protected void ActivateSkill(Skill skill)
    {
        SpendResources(skill);
        skill.Activate(gameObject);

        _isActivating = true;
        _currentActivationTime = skill.activationTime;

        if (skill.cooldown > 0)
        {
            skillCooldowns[skill] = skill.cooldown;
        }
    }

    private bool HasEnoughResources(Skill skill)
    {
        if (_stats.currentMana < skill.manaCost) return false;
        if (_stats.currentEnergy < skill.energyCost) return false;
        if (_stats.currentHealth <= skill.healthCost) return false; // Can't kill yourself
        if (_stats.currentAnguish < skill.anguishCost) return false;
        return true;
    }

    private void SpendResources(Skill skill)
    {
        _stats.SpendMana(skill.manaCost);
        _stats.SpendEnergy(skill.energyCost);
        _stats.TakeDamage(skill.healthCost); // Health cost is treated as damage
        _stats.SpendAnguish(skill.anguishCost);
    }

    private void HandleCooldowns()
    {
        if (skillCooldowns.Count == 0) return;
        List<Skill> skillsToRemove = new List<Skill>(skillCooldowns.Keys);
        foreach (Skill skill in skillsToRemove)
        {
            skillCooldowns[skill] -= Time.deltaTime;
            if (skillCooldowns[skill] <= 0)
            {
                skillCooldowns.Remove(skill);
            }
        }
    }

    private void HandleActivationLock()
    {
        if (_isActivating)
        {
            _currentActivationTime -= Time.deltaTime;
            if (_currentActivationTime <= 0)
            {
                _isActivating = false;
            }
        }
    }
}