using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(BuffManager))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerTargeting))]
public class PlayerSkillManager : SkillManagerBase
{
    [Header("Player-Specific Settings")]
    [SerializeField] private List<Skill> _startingSkills = new List<Skill>();
    [SerializeField] private int currencyRewardForDuplicateSkill = 100;

    public event Action OnSkillActivationStart;
    public event Action OnSkillActivationEnd;

    private List<Skill> _activePassives = new List<Skill>();
    private Dictionary<SkillAcquisitionCategory, int> _skillsLearnedPerCategory = new Dictionary<SkillAcquisitionCategory, int>();
    private Dictionary<SkillAcquisitionCategory, int> _skillCapsPerCategory = new Dictionary<SkillAcquisitionCategory, int>();

    private InventoryManager _inventoryManager;
    private BuffManager _buffManager;
    private PlayerStats _playerStats;
    private PlayerTargeting _playerTargeting;

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
        InitializeSkillCaps();
    }

    private void Start()
    {
        // Register this manager with the SkillbarManager to create a direct link.
        if (SkillbarManager.Instance != null)
        {
            SkillbarManager.Instance.RegisterPlayerSkillManager(this);
        }
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

    protected override void ActivateSkill(Skill skill, GameObject target)
    {
        SpendResources(skill);
        StartCoroutine(skill.Activate(this, this.gameObject, target));

        float reductionPercent = Mathf.Sqrt(_stats.Dexterity.Value / 2500f);
        float finalActivationTime = skill.baseActivationTime * (1 - reductionPercent);

        _currentActivationTime = Mathf.Max(0.05f, finalActivationTime);
        _isActivating = true;
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
            OnSkillActivationStart?.Invoke();
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

    // NEW: A more direct method for the SkillbarManager to call.
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