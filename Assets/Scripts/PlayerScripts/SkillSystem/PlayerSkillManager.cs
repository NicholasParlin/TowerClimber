using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This component is specific to the player and handles their skill learning progression,
// save/load functionality, and the logic for using skills. It is controlled by the PlayerInputManager.
public class PlayerSkillManager : SkillManagerBase
{
    [Header("Player-Specific Settings")]
    [Tooltip("The skills the player knows at the very start of a new game.")]
    [SerializeField] private List<Skill> _startingSkills = new List<Skill>();

    // Events for the movement system to listen to.
    public event Action OnSkillActivationStart;
    public event Action OnSkillActivationEnd;

    // Tracking for the 40% skill learning cap.
    private Dictionary<SkillAcquisitionCategory, int> _skillsLearnedPerCategory = new Dictionary<SkillAcquisitionCategory, int>();
    private Dictionary<SkillAcquisitionCategory, int> _skillCapsPerCategory = new Dictionary<SkillAcquisitionCategory, int>();

    #region Save System Integration
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
                    learnedSkillNames.Add(skill.name);
                }
            }
            skillsLearnedPerCategory = new Dictionary<SkillAcquisitionCategory, int>(skillManager._skillsLearnedPerCategory);
        }
    }

    public void SaveState()
    {
        SaveSystem.SavePlayerSkills(this);
    }

    public void LoadState(SkillDatabase skillDatabase)
    {
        SaveData data = SaveSystem.LoadPlayerSkills();
        if (data == null || skillDatabase == null)
        {
            InitializeNewCharacter();
            return;
        }

        learnedSkills.Clear();
        _skillsLearnedPerCategory = new Dictionary<SkillAcquisitionCategory, int>(data.skillsLearnedPerCategory);

        foreach (string skillName in data.learnedSkillNames)
        {
            Skill skillToLearn = skillDatabase.GetSkillByName(skillName);
            if (skillToLearn != null)
            {
                // Learn the skill, bypassing the 40% cap check since we're loading from a save.
                LearnNewSkill(skillToLearn, fromSaveFile: true);
            }
        }
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        InitializeSkillCaps();
        // The SaveLoadManager is responsible for calling LoadState, 
        // which in turn will call InitializeNewCharacter if no save is found.
    }

    private void InitializeNewCharacter()
    {
        foreach (Skill skill in _startingSkills)
        {
            LearnNewSkill(skill, SkillAcquisitionCategory.Taught);
        }
    }

    private void InitializeSkillCaps()
    {
        // These values represent the total number of skills available in each category on Floor 1.
        _skillCapsPerCategory[SkillAcquisitionCategory.Taught] = 5;
        _skillCapsPerCategory[SkillAcquisitionCategory.MonsterDrop] = 14;
        _skillCapsPerCategory[SkillAcquisitionCategory.Discovery] = 14;
        _skillCapsPerCategory[SkillAcquisitionCategory.QuestReward] = 15;
    }

    protected override void Update()
    {
        // The base update handles cooldowns and the activation lock.
        base.Update();

        // When the activation lock ends, fire the event for the movement system.
        if (_isActivating && _currentActivationTime <= 0)
        {
            OnSkillActivationEnd?.Invoke();
        }
    }

    /// <summary>
    /// The public-facing method for learning a new skill. It checks the 40% cap.
    /// </summary>
    /// <returns>True if the skill was learned, false if the cap was reached.</returns>
    public bool TryLearnNewSkill(Skill skillToLearn, SkillAcquisitionCategory category)
    {
        return LearnNewSkill(skillToLearn, category, fromSaveFile: false);
    }

    // The internal learning method, with a flag to bypass checks when loading from a save file.
    private bool LearnNewSkill(Skill skillToLearn, SkillAcquisitionCategory category = SkillAcquisitionCategory.Taught, bool fromSaveFile = false)
    {
        if (!fromSaveFile)
        {
            if (learnedSkills.Values.Any(list => list.Contains(skillToLearn))) return false;
            if (!_skillsLearnedPerCategory.ContainsKey(category)) _skillsLearnedPerCategory[category] = 0;

            int cap = Mathf.CeilToInt(_skillCapsPerCategory[category] * 0.4f);
            if (_skillsLearnedPerCategory[category] >= cap)
            {
                Debug.Log($"Skill cap for {category} reached. Awarding currency instead.");
                // TODO: Add currency reward logic here via the InventoryManager.
                return false;
            }
            _skillsLearnedPerCategory[category]++;
        }

        // Call the base method to add the skill to our dictionaries.
        base.LearnNewSkill(skillToLearn);
        return true;
    }

    // --- THIS IS THE FIX ---
    // The access modifier is changed from 'public' to 'protected' to match the base class.
    protected override void TryToUseSkill(Skill skill)
    {
        if (CanUseSkill(skill))
        {
            OnSkillActivationStart?.Invoke();
            ActivateSkill(skill);
        }
    }

    /// <summary>
    /// A public method for the PlayerInputManager to call when a skill key is pressed.
    /// </summary>
    public void AttemptToUseSkill(Archetype archetype, int index)
    {
        if (learnedSkills.ContainsKey(archetype) && index >= 0 && index < learnedSkills[archetype].Count)
        {
            Skill skillToUse = learnedSkills[archetype][index];
            TryToUseSkill(skillToUse);
        }
        else
        {
            Debug.LogWarning($"Attempted to use an invalid skill binding: Archetype {archetype}, Index {index}.");
        }
    }
}