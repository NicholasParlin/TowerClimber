using System.Collections.Generic;
using UnityEngine;

public abstract class SkillManagerBase : MonoBehaviour
{
    public Dictionary<Archetype, List<Skill>> learnedSkills { get; protected set; } = new Dictionary<Archetype, List<Skill>>();

    protected Dictionary<Skill, float> skillCooldowns = new Dictionary<Skill, float>();

    protected bool _isActivating;
    protected float _currentActivationTime;

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

    protected virtual void TryToUseSkill(Skill skill, GameObject target)
    {
        if (CanUseSkill(skill))
        {
            ActivateSkill(skill, target);
        }
    }

    protected bool CanUseSkill(Skill skill)
    {
        if (skill == null) return false;
        if (_isActivating) return false;
        if (skillCooldowns.ContainsKey(skill)) return false;
        if (!HasEnoughResources(skill)) return false;
        return true;
    }

    protected virtual void ActivateSkill(Skill skill, GameObject target)
    {
        SpendResources(skill);

        // We now pass 'this' (the SkillManager) to the coroutine.
        StartCoroutine(skill.Activate(this, this.gameObject, target));

        _isActivating = true;
        _currentActivationTime = skill.baseActivationTime;
    }

    // NEW PUBLIC METHOD: The Skill coroutine will call this when its sequence is finished.
    public void ApplyCooldown(Skill skill)
    {
        if (skill.cooldown > 0)
        {
            skillCooldowns[skill] = skill.cooldown;
        }
    }

    protected bool HasEnoughResources(Skill skill)
    {
        if (_stats.currentMana < skill.manaCost) return false;
        if (_stats.currentEnergy < skill.energyCost) return false;
        if (_stats.currentHealth <= skill.healthCost) return false;
        if (_stats.currentAnguish < skill.anguishCost) return false;
        return true;
    }

    protected void SpendResources(Skill skill)
    {
        _stats.SpendMana(skill.manaCost);
        _stats.SpendEnergy(skill.energyCost);
        _stats.TakeDamage(skill.healthCost);
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