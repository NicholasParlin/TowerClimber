using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This component manages the player's unlocked titles and their currently equipped title.
[RequireComponent(typeof(BuffManager), typeof(PlayerStats))]
public class TitleManager : MonoBehaviour
{
    private List<Title> _unlockedTitles = new List<Title>();
    private Title _equippedTitle;

    // References to other components for applying buffs.
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

    public void SaveState()
    {
        SaveSystem.SavePlayerTitles(this);
        Debug.Log("Player titles saved.");
    }

    public void LoadState(TitleDatabase titleDatabase)
    {
        SaveData data = SaveSystem.LoadPlayerTitles();
        if (data == null || titleDatabase == null)
        {
            Debug.Log("No title save data found, starting fresh.");
            return;
        }

        _unlockedTitles.Clear();
        foreach (string titleName in data.unlockedTitleNames)
        {
            Title title = titleDatabase.GetTitleByName(titleName);
            if (title != null)
            {
                _unlockedTitles.Add(title);
            }
        }

        if (!string.IsNullOrEmpty(data.equippedTitleName))
        {
            Title titleToEquip = titleDatabase.GetTitleByName(data.equippedTitleName);
            if (titleToEquip != null)
            {
                EquipTitle(titleToEquip);
            }
        }
        Debug.Log("Player titles loaded.");
    }
    #endregion

    private void Awake()
    {
        _buffManager = GetComponent<BuffManager>();
        _playerStats = GetComponent<PlayerStats>();
    }

    /// <summary>
    /// Public method to safely get the list of unlocked titles for UI display.
    /// This is the new method that the CharacterPanelUI needs.
    /// </summary>
    public List<Title> GetUnlockedTitles()
    {
        return _unlockedTitles;
    }

    /// <summary>
    /// Unlocks a new title for the player, adding it to their list of available titles.
    /// </summary>
    public void UnlockTitle(Title titleToUnlock)
    {
        if (titleToUnlock != null && !_unlockedTitles.Contains(titleToUnlock))
        {
            _unlockedTitles.Add(titleToUnlock);
            Debug.Log($"New Title Unlocked: {titleToUnlock.titleName}");
            // Here you would fire an event to show a UI notification.
        }
    }

    /// <summary>
    /// Equips a new title, unequipping any previous one and applying the new stat bonuses.
    /// </summary>
    public void EquipTitle(Title newTitle)
    {
        if (newTitle == null || !_unlockedTitles.Contains(newTitle)) return;

        // First, remove the buffs from the currently equipped title.
        UnequipCurrentTitle();

        // Equip the new title and apply its buffs.
        _equippedTitle = newTitle;
        foreach (StatBonus bonus in _equippedTitle.statBonuses)
        {
            Stat targetStat = _playerStats.GetStat(bonus.statToBuff);
            if (targetStat != null)
            {
                // The modifier has no duration (-1f) and its source is the Title asset itself.
                var modifier = new StatModifier(bonus.value, -1f, _equippedTitle);
                _buffManager.AddModifier(targetStat, modifier);
            }
        }
        Debug.Log($"Title Equipped: {_equippedTitle.titleName}");
    }

    /// <summary>
    /// Unequips the currently active title and removes its associated stat bonuses.
    /// </summary>
    public void UnequipCurrentTitle()
    {
        if (_equippedTitle == null) return;

        // Use the BuffManager's ability to remove all modifiers from a specific source.
        _buffManager.RemoveAllModifiersFromSource(_equippedTitle);
        Debug.Log($"Title Unequipped: {_equippedTitle.titleName}");
        _equippedTitle = null;
    }
}