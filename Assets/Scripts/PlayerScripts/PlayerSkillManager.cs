using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This component is specific to the player and handles their input for using skills,
// skill learning progression, and save/load functionality.
public class PlayerSkillManager : SkillManagerBase
{
    [Header("Player-Specific Settings")]
    [Tooltip("The skills the player knows at the very start of a new game.")]
    [SerializeField] private List<Skill> _startingSkills = new List<Skill>();

    [Header("Skill Input Bindings")]
    [Tooltip("Set up which keys correspond to which skills here.")]
    [SerializeField] private List<SkillBinding> skillBindings = new List<SkillBinding>();

    // Events for the movement system to subscribe to.
    public event Action OnSkillActivationStart;
    public event Action OnSkillActivationEnd;

    // Tracking for the 40% skill learning cap.
    private Dictionary<SkillAcquisitionCategory, int> _skillsLearnedPerCategory = new Dictionary<SkillAcquisitionCategory, int>();
    private Dictionary<SkillAcquisitionCategory, int> _skillCapsPerCategory = new Dictionary<SkillAcquisitionCategory, int>();

    #region Save System Integration

    // This is the data structure that will be saved to a file.
    [System.Serializable]
    public class SaveData
    {
        public List<string> learnedSkillNames;
        public Dictionary<SkillAcquisitionCategory, int> skillsLearnedPerCategory;

        public SaveData(PlayerSkillManager skillManager)
        {
            learnedSkillNames = new List<string>();
            foreach (var skillList in skillManager.learnedSkills.Values)
            {
                foreach (var skill in skillList)
                {
                    learnedSkillNames.Add(skill.name); // Use the asset's name as a unique ID
                }
            }
            skillsLearnedPerCategory = new Dictionary<SkillAcquisitionCategory, int>(skillManager._skillsLearnedPerCategory);
        }
    }

    public void SaveState()
    {
        SaveSystem.SavePlayerSkills(this);
        Debug.Log("Player skills saved.");
    }

    public void LoadState(SkillDatabase skillDatabase)
    {
        SaveData data = SaveSystem.LoadPlayerSkills();
        if (data == null || skillDatabase == null)
        {
            InitializeNewCharacter();
            return;
        }

        // Clear any existing skills before loading.
        learnedSkills.Clear();

        // Load the progression caps.
        _skillsLearnedPerCategory = new Dictionary<SkillAcquisitionCategory, int>(data.skillsLearnedPerCategory);

        // Re-learn all skills from the save file using their names.
        foreach (string skillName in data.learnedSkillNames)
        {
            Skill skillToLearn = skillDatabase.GetSkillByName(skillName);
            if (skillToLearn != null)
            {
                // We pass 'fromSaveFile: true' to bypass the cap check during loading.
                LearnNewSkill(skillToLearn, SkillAcquisitionCategory.Taught, fromSaveFile: true);
            }
        }
        Debug.Log("Player skills loaded.");
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        InitializeSkillCaps();
        // NOTE: The actual loading should be called by a central game manager after the skill database is ready.
        // For now, we initialize a new character.
        // LoadState(FindObjectOfType<SkillDatabase>()); 
        InitializeNewCharacter();
    }

    private void InitializeNewCharacter()
    {
        Debug.Log("Initializing new character skills.");
        foreach (Skill skill in _startingSkills)
        {
            // By default, starting skills are considered "Taught"
            LearnNewSkill(skill, SkillAcquisitionCategory.Taught);
        }
    }

    private void InitializeSkillCaps()
    {
        // This is where you would define the total number of skills available per category on this floor.
        // For now, we'll use the hardcoded numbers from our design document for Floor 1.
        _skillCapsPerCategory[SkillAcquisitionCategory.Taught] = 5;
        _skillCapsPerCategory[SkillAcquisitionCategory.MonsterDrop] = 14;
        _skillCapsPerCategory[SkillAcquisitionCategory.Discovery] = 14;
        _skillCapsPerCategory[SkillAcquisitionCategory.QuestReward] = 15;
    }

    protected override void Update()
    {
        // We check if the activation has ended before calling the base Update.
        // This ensures the event fires on the exact frame the lock ends.
        if (_isActivating && _currentActivationTime <= 0)
        {
            OnSkillActivationEnd?.Invoke();
        }

        base.Update(); // This runs the cooldown and activation logic from the base class.
        HandleSkillInput();
    }

    /// <summary>
    /// This is the public-facing method for learning a new skill. It checks the 40% cap.
    /// </summary>
    /// <returns>True if the skill was learned, false if the cap was reached.</returns>
    public bool TryLearnNewSkill(Skill skillToLearn, SkillAcquisitionCategory category)
    {
        return LearnNewSkill(skillToLearn, category, fromSaveFile: false);
    }

    // The internal learning method, now with a flag to bypass checks when loading.
    private bool LearnNewSkill(Skill skillToLearn, SkillAcquisitionCategory category, bool fromSaveFile = false)
    {
        if (!fromSaveFile)
        {
            // Check if we have already learned this skill.
            if (learnedSkills.Values.Any(list => list.Contains(skillToLearn)))
            {
                Debug.Log($"{skillToLearn.skillName} is already known.");
                return false;
            }

            // Initialize the counter for this category if it doesn't exist.
            if (!_skillsLearnedPerCategory.ContainsKey(category))
            {
                _skillsLearnedPerCategory[category] = 0;
            }

            // Check against the 40% cap.
            int cap = Mathf.CeilToInt(_skillCapsPerCategory[category] * 0.4f);
            if (_skillsLearnedPerCategory[category] >= cap)
            {
                Debug.Log($"Skill learning cap for {category} reached. Awarding currency instead.");
                // TODO: Add logic here to give the player gold.
                return false;
            }

            _skillsLearnedPerCategory[category]++;
        }

        // Call the base method to add the skill to our dictionaries.
        base.LearnNewSkill(skillToLearn);
        return true;
    }

    // We override the base method to add the event invocation.
    protected override void TryToUseSkill(Skill skill)
    {
        // First, check if all conditions in the base class are met.
        if (CanUseSkill(skill))
        {
            // Fire the event to stop movement.
            OnSkillActivationStart?.Invoke();

            // Now, actually use the skill by calling the base activation logic.
            ActivateSkill(skill);
        }
    }

    private void HandleSkillInput()
    {
        foreach (SkillBinding binding in skillBindings)
        {
            if (Input.GetKeyDown(binding.key))
            {
                AttemptToUseSkillFromBinding(binding);
            }
        }
    }

    private void AttemptToUseSkillFromBinding(SkillBinding binding)
    {
        if (learnedSkills.ContainsKey(binding.archetype) &&
            binding.skillIndex >= 0 &&
            binding.skillIndex < learnedSkills[binding.archetype].Count)
        {
            Skill skillToUse = learnedSkills[binding.archetype][binding.skillIndex];
            TryToUseSkill(skillToUse);
        }
    }

    // Helper class for the Inspector.
    [System.Serializable]
    private class SkillBinding
    {
        public KeyCode key;
        public Archetype archetype;
        public int skillIndex;
    }
}
