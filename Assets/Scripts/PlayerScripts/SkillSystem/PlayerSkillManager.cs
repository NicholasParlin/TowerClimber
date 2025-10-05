using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(BuffManager))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerTargeting))]
[RequireComponent(typeof(CharacterStateManager))] // Make sure it has a state manager
public class PlayerSkillManager : SkillManagerBase
{
    [Header("Player-Specific Settings")]
    [SerializeField] private List<Skill> _startingSkills = new List<Skill>();
    [SerializeField] private int currencyRewardForDuplicateSkill = 100;

    // These events are no longer needed as the state machine handles these transitions.
    // public event Action OnSkillActivationStart;
    // public event Action OnSkillActivationEnd;

    private List<Skill> _activePassives = new List<Skill>();
    private Dictionary<SkillAcquisitionCategory, int> _skillsLearnedPerCategory = new Dictionary<SkillAcquisitionCategory, int>();
    private Dictionary<SkillAcquisitionCategory, int> _skillCapsPerCategory = new Dictionary<SkillAcquisitionCategory, int>();

    private InventoryManager _inventoryManager;
    private BuffManager _buffManager;
    private PlayerStats _playerStats;
    private PlayerTargeting _playerTargeting;
    private CharacterStateManager _stateManager; // NEW: Reference to the state manager
    private PlayerStateFactory _stateFactory; // NEW: Reference to the state factory

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
            if (passiveSkill != null && passiveSkill.passiveEffectToApply != null)
            {
                _activePassives.Add(passiveSkill);
                _buffManager.ApplyStatusEffect(passiveSkill.passiveEffectToApply, this.gameObject, passiveSkill);
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
        _playerTargeting = GetComponent<PlayerTargeting>();
        _stateManager = GetComponent<CharacterStateManager>(); // NEW: Get the component
        _stateFactory = new PlayerStateFactory(_stateManager); // NEW: Initialize the factory
        InitializeSkillCaps();
    }

    // The Update and redundant Start methods can be removed.

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

    protected override void ActivateSkill(Skill skill, GameObject target)
    {
        SpendResources(skill);
        StartCoroutine(skill.Activate(this, this.gameObject, target));

        // MODIFIED: Calculate activation time and switch state
        float reductionPercent = Mathf.Sqrt(_stats.Dexterity.Value / 2500f);
        float finalActivationTime = skill.baseActivationTime * (1 - reductionPercent);
        _stateManager.CurrentActionTime = Mathf.Max(0.05f, finalActivationTime);

        // Pass the skill itself to the state manager so it knows about super armor
        _stateManager.CurrentSkillInUse = skill;
        _stateManager.SwitchState(_stateFactory.ActivatingSkill());
    }

    public bool TryLearnNewSkill(Skill skillToLearn, SkillAcquisitionCategory category)
    {
        if (learnedSkills.Values.Any(list => list.Contains(skillToLearn)))
        {
            return false;
        }

        if (!_skillsLearnedPerCategory.ContainsKey(category))
        {
            _skillsLearnedPerCategory[category] = 0;
        }

        int cap = Mathf.CeilToInt(_skillsLearnedPerCategory[category] * 0.4f);
        if (_skillsLearnedPerCategory[category] >= cap)
        {
            if (_inventoryManager != null) { _inventoryManager.AddGold(currencyRewardForDuplicateSkill); }
            return false;
        }

        _skillsLearnedPerCategory[category]++;
        base.LearnNewSkill(skillToLearn);
        return true;
    }

    protected override void TryToUseSkill(Skill skill, GameObject target)
    {
        if (CanUseSkill(skill))
        {
            // The OnSkillActivationStart event is no longer needed here
            ActivateSkill(skill, target);
        }
    }

    public void AttemptToUseSkill(Archetype archetype, int index)
    {
        if (learnedSkills.ContainsKey(archetype) && index >= 0 && index < learnedSkills[archetype].Count)
        {
            Skill skillToUse = learnedSkills[archetype][index];
            AttemptToUseSkillByReference(skillToUse);
        }
    }

    public void AttemptToUseSkillByReference(Skill skillToUse)
    {
        if (skillToUse.passiveEffectToApply != null) return;

        GameObject target = null;
        if (skillToUse.isSelfCast)
        {
            target = this.gameObject;
        }
        else
        {
            target = _playerTargeting.GetCurrentTarget();
        }

        TryToUseSkill(skillToUse, target);
    }

    public void TogglePassive(Skill passiveSkill)
    {
        if (passiveSkill == null || passiveSkill.passiveEffectToApply == null) return;

        if (_activePassives.Contains(passiveSkill))
        {
            _activePassives.Remove(passiveSkill);
            _buffManager.RemoveAllModifiersFromSource(passiveSkill.passiveEffectToApply);
        }
        else
        {
            _activePassives.Add(passiveSkill);
            _buffManager.ApplyStatusEffect(passiveSkill.passiveEffectToApply, this.gameObject, passiveSkill);
        }
    }

    public bool IsPassiveActive(Skill passiveSkill)
    {
        return _activePassives.Contains(passiveSkill);
    }
}