using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Required for .ToList()

// REFACTORED to inherit from SkillManagerBase for consistency and code reuse.
public class EnemySkillManager : SkillManagerBase
{
    [Header("Enemy Skill Configuration")]
    [Tooltip("Assign the Skill Set ScriptableObject that defines this enemy's abilities.")]
    [SerializeField] private EnemySkillSet skillSet;

    // A reference to the player's GameObject to use as a target.
    private GameObject _playerTarget;

    // This overrides the base Awake method but also calls it.
    protected override void Awake()
    {
        base.Awake(); // This calls the Awake() method in SkillManagerBase

        // Find and store a reference to the player.
        _playerTarget = GameObject.FindGameObjectWithTag("Player");

        // Learn all skills from the assigned skill set.
        if (skillSet != null)
        {
            foreach (Skill skill in skillSet.skills)
            {
                if (skill != null)
                {
                    // Using the base class's method to learn skills.
                    LearnNewSkill(skill);
                }
            }
        }
        else
        {
            Debug.LogWarning($"No Skill Set assigned to {gameObject.name}. This enemy will have no skills.");
        }
    }

    // This overrides the base Update method but also calls it.
    protected override void Update()
    {
        base.Update(); // This handles cooldowns and activation locks automatically.
        SimpleAIUpdate();
    }

    /// <summary>
    /// A very basic placeholder AI that tries to use a random skill on a timer.
    /// </summary>
    private void SimpleAIUpdate()
    {
        // Don't do anything if there are no learned skills or no target.
        if (learnedSkills.Count == 0 || _playerTarget == null) return;

        // Simple timer logic
        if (Time.time > _nextSkillCheckTime)
        {
            // --- CORRECTED LOGIC ---
            // 1. Get all the lists of skills from the dictionary.
            List<List<Skill>> allSkillLists = learnedSkills.Values.ToList();
            // 2. Pick a random list.
            List<Skill> randomSkillList = allSkillLists[Random.Range(0, allSkillLists.Count)];
            // 3. Pick a random skill from that list.
            Skill randomSkill = randomSkillList[Random.Range(0, randomSkillList.Count)];

            // We now use the TryToUseSkill method inherited from the base class, passing the player as the target.
            TryToUseSkill(randomSkill, _playerTarget);

            _nextSkillCheckTime = Time.time + 2.0f;
        }
    }
    private float _nextSkillCheckTime;
}