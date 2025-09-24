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
        public List<string> assignedSkillNames;
        public SaveData(SkillbarManager skillbarManager)
        {
            assignedSkillNames = skillbarManager._skillSlots.Select(skill => skill != null ? skill.name : string.Empty).ToList();
        }
    }
    public void SaveState() { /* Placeholder */ }
    public void LoadState(SkillDatabase skillDatabase) { /* Placeholder */ }
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    // NEW: A public method for the PlayerSkillManager to register itself.
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