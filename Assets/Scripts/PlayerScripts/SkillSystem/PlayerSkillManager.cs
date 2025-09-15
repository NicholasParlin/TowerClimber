using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This component is specific to the player and handles their skill learning, progression,
// and usage. It now also manages the toggling of passive skills.
[RequireComponent(typeof(InventoryManager), typeof(BuffManager), typeof(PlayerStats))]
public class PlayerSkillManager : SkillManagerBase
{
    [Header("Player-Specific Settings")]
    [SerializeField] private List<Skill> _startingSkills = new List<Skill>();
    [SerializeField] private int currencyRewardForDuplicateSkill = 100;

    public event Action OnSkillActivationStart;
    public event Action OnSkillActivationEnd;

    private List<Skill> _activePassives = new List<Skill>();

    // Dictionaries for the 40% skill learning cap.
    private Dictionary<SkillAcquisitionCategory, int> _skillsLearnedPerCategory = new Dictionary<SkillAcquisitionCategory, int>();
    private Dictionary<SkillAcquisitionCategory, int> _skillCapsPerCategory = new Dictionary<SkillAcquisitionCategory, int>();

    // Component references.
    private InventoryManager _inventoryManager;
    private BuffManager _buffManager;
    private PlayerStats _playerStats;

    #region Save System Integration
    [System.Serializable]
    public class SaveData
    {
        public List<string> learnedSkillNames;
        public Dictionary<SkillAcquisitionCategory, int> skillsLearnedPerCategory;
        public List<string> activePassiveSkillNames;

        public SaveData(PlayerSkillManager skillManager)
        {
            learnedSkillNames = new List<string>();
            foreach (var skillList in skillManager.learnedSkills.Values)
            {
                foreach (var skill in skillList) { learnedSkillNames.Add(skill.name); }
            }
            skillsLearnedPerCategory = new Dictionary<SkillAcquisitionCategory, int>(skillManager._skillsLearnedPerCategory);
            activePassiveSkillNames = skillManager._activePassives.Select(s => s.name).ToList();
        }
    }

    public void SaveState() { SaveSystem.SavePlayerSkills(this); }

    public void LoadState(SkillDatabase skillDatabase)
    {
        SaveData data = SaveSystem.LoadPlayerSkills();
        if (data == null || skillDatabase == null) { InitializeNewCharacter(); return; }

        learnedSkills.Clear();
        _skillsLearnedPerCategory = new Dictionary<SkillAcquisitionCategory, int>(data.skillsLearnedPerCategory);

        foreach (string skillName in data.learnedSkillNames)
        {
            Skill skillToLearn = skillDatabase.GetSkillByName(skillName);
            if (skillToLearn != null) { base.LearnNewSkill(skillToLearn); }
        }

        _activePassives.Clear();
        foreach (string skillName in data.activePassiveSkillNames)
        {
            Skill passiveSkill = skillDatabase.GetSkillByName(skillName);
            if (passiveSkill != null && passiveSkill.isPassive)
            {
                _activePassives.Add(passiveSkill);
                ApplyPassiveEffect(passiveSkill);
            }
        }
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        _inventoryManager = GetComponent<InventoryManager>();
        _buffManager = GetComponent<BuffManager>();
        _playerStats = GetComponent<PlayerStats>();
        InitializeSkillCaps();
    }

    private void InitializeNewCharacter()
    {
        Debug.Log("Initializing new character with starting skills.");
        foreach (Skill skill in _startingSkills)
        {
            base.LearnNewSkill(skill);
        }
    }

    private void InitializeSkillCaps()
    {
        _skillCapsPerCategory[SkillAcquisitionCategory.Taught] = 5;
        _skillCapsPerCategory[SkillAcquisitionCategory.MonsterDrop] = 14;
        _skillCapsPerCategory[SkillAcquisitionCategory.Discovery] = 14;
        _skillCapsPerCategory[SkillAcquisitionCategory.QuestReward] = 15;
    }

    protected override void Update()
    {
        base.Update();
        if (_isActivating && _currentActivationTime <= 0) { OnSkillActivationEnd?.Invoke(); }
    }

    public bool TryLearnNewSkill(Skill skillToLearn, SkillAcquisitionCategory category)
    {
        if (learnedSkills.Values.Any(list => list.Contains(skillToLearn)))
        {
            Debug.Log($"{skillToLearn.skillName} is already known.");
            return false;
        }

        if (!_skillsLearnedPerCategory.ContainsKey(category))
        {
            _skillsLearnedPerCategory[category] = 0;
        }

        int cap = Mathf.CeilToInt(_skillCapsPerCategory[category] * 0.4f);
        if (_skillsLearnedPerCategory[category] >= cap)
        {
            if (_inventoryManager != null)
            {
                _inventoryManager.AddGold(currencyRewardForDuplicateSkill);
            }
            Debug.Log($"Skill cap for {category} reached. Awarded {currencyRewardForDuplicateSkill} gold instead.");
            return false;
        }

        _skillsLearnedPerCategory[category]++;
        base.LearnNewSkill(skillToLearn);
        return true;
    }

    protected override void TryToUseSkill(Skill skill)
    {
        if (CanUseSkill(skill))
        {
            OnSkillActivationStart?.Invoke();
            ActivateSkill(skill);
        }
    }

    public void AttemptToUseSkill(Archetype archetype, int index)
    {
        if (learnedSkills.ContainsKey(archetype) && index >= 0 && index < learnedSkills[archetype].Count)
        {
            Skill skillToUse = learnedSkills[archetype][index];
            if (!skillToUse.isPassive)
            {
                TryToUseSkill(skillToUse);
            }
        }
    }

    public void TogglePassive(Skill passiveSkill)
    {
        if (passiveSkill == null || !passiveSkill.isPassive) return;

        if (_activePassives.Contains(passiveSkill))
        {
            _activePassives.Remove(passiveSkill);
            RemovePassiveEffect(passiveSkill);
            Debug.Log($"Deactivated passive: {passiveSkill.skillName}");
        }
        else
        {
            _activePassives.Add(passiveSkill);
            ApplyPassiveEffect(passiveSkill);
            Debug.Log($"Activated passive: {passiveSkill.skillName}");
        }
    }

    public bool IsPassiveActive(Skill passiveSkill)
    {
        return _activePassives.Contains(passiveSkill);
    }

    private void ApplyPassiveEffect(Skill passiveSkill)
    {
        // This is where you would define the specific stat modifiers for each passive skill.
        // Using the skill's name (which is unique as it's an asset filename) is a simple way to do this.
        switch (passiveSkill.name)
        {
            case "IronWill": // Juggernaut
                // Example: A 15% bonus to Armor.
                var defenseMod = new StatModifier(0.15f, -1f, passiveSkill, ModifierType.Percentage);
                _buffManager.AddModifier(_playerStats.Armor, defenseMod);
                break;
                // Add other passive skill cases here...
        }
    }

    private void RemovePassiveEffect(Skill passiveSkill)
    {
        _buffManager.RemoveAllModifiersFromSource(passiveSkill);
    }
}