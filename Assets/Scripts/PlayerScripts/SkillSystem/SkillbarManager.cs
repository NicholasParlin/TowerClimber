using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SkillbarManager : MonoBehaviour
{
    public static SkillbarManager Instance { get; private set; }

    public event Action<int> OnSkillSlotChanged;
    public event Action<int, float> OnSkillCooldownStarted;
    public event Action<int> OnSkillUsed;

    private Skill[] _skillSlots = new Skill[9];
    private PlayerSkillManager _playerSkillManager;

    #region Save System Integration
    [System.Serializable]
    public class SaveData
    {
        // This will save the skill's asset name for each of the 9 slots.
        // If a slot is empty, it will save an empty string.
        public List<string> assignedSkillNames;

        public SaveData(SkillbarManager skillbarManager)
        {
            assignedSkillNames = skillbarManager._skillSlots.Select(skill => skill != null ? skill.name : string.Empty).ToList();
        }
    }

    public void SaveState()
    {
        // MODIFIED: This line is now active.
        SaveSystem.SavePlayerSkillbar(this);
    }

    public void LoadState(SkillDatabase skillDatabase)
    {
        // MODIFIED: This entire block is now active.
        SaveData data = SaveSystem.LoadPlayerSkillbar();
        if (data == null || skillDatabase == null) return;

        for (int i = 0; i < data.assignedSkillNames.Count && i < _skillSlots.Length; i++)
        {
            if (!string.IsNullOrEmpty(data.assignedSkillNames[i]))
            {
                Skill skill = skillDatabase.GetSkillByName(data.assignedSkillNames[i]);
                AssignSkillToSlot(skill, i);
            }
            else
            {
                UnassignSkillFromSlot(i);
            }
        }
    }
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    public void RegisterPlayerSkillManager(PlayerSkillManager psm)
    {
        _playerSkillManager = psm;
        _playerSkillManager.OnCooldownApplied += HandleCooldownApplied;
    }

    private void OnDestroy()
    {
        if (_playerSkillManager != null)
        {
            _playerSkillManager.OnCooldownApplied -= HandleCooldownApplied;
        }
    }

    public void ActivateSkillInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _skillSlots.Length) return;

        Skill skillToUse = _skillSlots[slotIndex];
        if (skillToUse != null && _playerSkillManager != null)
        {
            OnSkillUsed?.Invoke(slotIndex);
            _playerSkillManager.AttemptToUseSkillByReference(skillToUse);
        }
    }

    private void HandleCooldownApplied(Skill skill)
    {
        for (int i = 0; i < _skillSlots.Length; i++)
        {
            if (_skillSlots[i] == skill)
            {
                OnSkillCooldownStarted?.Invoke(i, skill.cooldown);
                return;
            }
        }
    }

    public void AssignSkillToSlot(Skill skill, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _skillSlots.Length) return;
        _skillSlots[slotIndex] = skill;
        OnSkillSlotChanged?.Invoke(slotIndex);
    }

    public void UnassignSkillFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _skillSlots.Length) return;
        _skillSlots[slotIndex] = null;
        OnSkillSlotChanged?.Invoke(slotIndex);
    }

    public Skill GetSkillInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _skillSlots.Length) return null;
        return _skillSlots[slotIndex];
    }
}