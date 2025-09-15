using UnityEngine;
using System.Collections.Generic;

// This component is specific to AI-controlled enemies and handles their skill usage.
// It is a self-contained system and does not rely on archetypes for sorting.
public class EnemySkillManager : MonoBehaviour
{
    [Header("Enemy Skill Configuration")]
    [Tooltip("Assign the Skill Set ScriptableObject that defines this enemy's abilities.")]
    [SerializeField] private EnemySkillSet skillSet;

    // A simple list to hold all skills this enemy knows.
    private List<Skill> _learnedSkills = new List<Skill>();

    // Dictionaries to track cooldowns and activation times.
    private Dictionary<Skill, float> _skillCooldowns = new Dictionary<Skill, float>();
    private bool _isActivating;
    private float _currentActivationTime;

    // Reference to this enemy's stats.
    private CharacterStatsBase _stats;

    private void Awake()
    {
        _stats = GetComponent<CharacterStatsBase>();

        // Learn all skills from the assigned skill set and populate the simple list.
        if (skillSet != null)
        {
            foreach (Skill skill in skillSet.skills)
            {
                if (skill != null)
                {
                    _learnedSkills.Add(skill);
                }
            }
        }
        else
        {
            Debug.LogWarning($"No Skill Set assigned to {gameObject.name}. This enemy will have no skills.");
        }
    }

    private void Update()
    {
        HandleCooldowns();
        HandleActivationLock();
        SimpleAIUpdate();
    }

    /// <summary>
    /// Public method that can be called by an external AI controller to use a specific skill.
    /// </summary>
    public void UseSkill(Skill skill)
    {
        if (_learnedSkills.Contains(skill))
        {
            TryToUseSkill(skill);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} tried to use a skill it does not know: {skill.skillName}");
        }
    }

    /// <summary>
    /// A very basic placeholder AI that tries to use a random skill on a timer.
    /// </summary>
    private void SimpleAIUpdate()
    {
        // In a real game, this logic would be in a dedicated AI Controller script.
        // This is just for demonstration.
        if (_learnedSkills.Count == 0) return;

        // Simple timer logic
        if (Time.time > _nextSkillCheckTime)
        {
            // Pick a random learned skill and try to use it.
            int randomIndex = Random.Range(0, _learnedSkills.Count);
            Skill randomSkill = _learnedSkills[randomIndex];
            TryToUseSkill(randomSkill);

            // Set the next time the AI will check to use a skill.
            _nextSkillCheckTime = Time.time + 2.0f;
        }
    }
    private float _nextSkillCheckTime; // Helper variable for the simple AI.


    // --- Core Skill Logic (Simplified from SkillManagerBase) ---

    private void TryToUseSkill(Skill skill)
    {
        if (_isActivating || _skillCooldowns.ContainsKey(skill) || !HasEnoughResources(skill))
        {
            return; // Cannot use skill.
        }

        SpendResources(skill);
        skill.Activate(gameObject);
        _isActivating = true;
        _currentActivationTime = skill.activationTime;

        if (skill.cooldown > 0)
        {
            _skillCooldowns[skill] = skill.cooldown;
        }
    }

    private bool HasEnoughResources(Skill skill)
    {
        return _stats.currentMana >= skill.manaCost &&
               _stats.currentEnergy >= skill.energyCost &&
               _stats.currentHealth > skill.healthCost &&
               _stats.currentAnguish >= skill.anguishCost;
    }

    private void SpendResources(Skill skill)
    {
        _stats.SpendMana(skill.manaCost);
        _stats.SpendEnergy(skill.energyCost);
        _stats.TakeDamage(skill.healthCost);
        _stats.SpendAnguish(skill.anguishCost);
    }

    private void HandleCooldowns()
    {
        if (_skillCooldowns.Count == 0) return;

        List<Skill> skillsToRemove = new List<Skill>();
        foreach (var entry in _skillCooldowns)
        {
            _skillCooldowns[entry.Key] -= Time.deltaTime;
            if (_skillCooldowns[entry.Key] <= 0)
            {
                skillsToRemove.Add(entry.Key);
            }
        }
        foreach (var skill in skillsToRemove)
        {
            _skillCooldowns.Remove(skill);
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