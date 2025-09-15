using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BuffManager), typeof(PlayerStats))]
public class TitleManager : MonoBehaviour
{
    // --- TODO RESOLVED: Event for UI updates ---
    public event Action OnEquippedTitleChanged;

    private List<Title> _unlockedTitles = new List<Title>();
    private Title _equippedTitle;

    private BuffManager _buffManager;
    private PlayerStats _playerStats;

    #region Save System Integration
    [System.Serializable]
    public class SaveData
    {
        public List<string> unlockedTitleNames;
        public string equippedTitleName;

        public SaveData(TitleManager titleManager)
        {
            unlockedTitleNames = titleManager._unlockedTitles.Select(t => t.name).ToList();
            equippedTitleName = titleManager._equippedTitle != null ? titleManager._equippedTitle.name : "";
        }
    }

    public void SaveState() { SaveSystem.SavePlayerTitles(this); }

    public void LoadState(TitleDatabase titleDatabase)
    {
        SaveData data = SaveSystem.LoadPlayerTitles();
        if (data == null || titleDatabase == null) return;

        _unlockedTitles.Clear();
        foreach (string titleName in data.unlockedTitleNames)
        {
            Title title = titleDatabase.GetTitleByName(titleName);
            if (title != null) { _unlockedTitles.Add(title); }
        }

        if (!string.IsNullOrEmpty(data.equippedTitleName))
        {
            Title titleToEquip = titleDatabase.GetTitleByName(data.equippedTitleName);
            if (titleToEquip != null) { EquipTitle(titleToEquip); }
        }
    }
    #endregion

    private void Awake()
    {
        _buffManager = GetComponent<BuffManager>();
        _playerStats = GetComponent<PlayerStats>();
    }

    public List<Title> GetUnlockedTitles() => _unlockedTitles;

    // --- TODO RESOLVED: Method for UI to check equipped status ---
    public bool IsTitleEquipped(Title title) => title != null && _equippedTitle == title;

    public void UnlockTitle(Title titleToUnlock)
    {
        if (titleToUnlock != null && !_unlockedTitles.Contains(titleToUnlock))
        {
            _unlockedTitles.Add(titleToUnlock);
        }
    }

    public void EquipTitle(Title newTitle)
    {
        if (newTitle == null || !_unlockedTitles.Contains(newTitle)) return;
        if (_equippedTitle == newTitle) return; // Don't re-equip the same title

        UnequipCurrentTitle();

        _equippedTitle = newTitle;
        foreach (StatBonus bonus in _equippedTitle.statBonuses)
        {
            Stat targetStat = _playerStats.GetStat(bonus.statToBuff);
            if (targetStat != null)
            {
                var modifier = new StatModifier(bonus.value, -1f, _equippedTitle);
                _buffManager.AddModifier(targetStat, modifier);
            }
        }
        OnEquippedTitleChanged?.Invoke(); // Fire event to update UI
    }

    public void UnequipCurrentTitle()
    {
        if (_equippedTitle == null) return;
        _buffManager.RemoveAllModifiersFromSource(_equippedTitle);
        _equippedTitle = null;
        OnEquippedTitleChanged?.Invoke(); // Fire event to update UI
    }
}