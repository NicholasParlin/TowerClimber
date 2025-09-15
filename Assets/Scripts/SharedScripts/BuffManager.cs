using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// This component manages all temporary stat modifiers (buffs and debuffs) on a character.
public class BuffManager : MonoBehaviour
{
    // A private class to keep track of active modifiers and their timers.
    private class ActiveModifier
    {
        public Stat TargetStat;
        public StatModifier Modifier;
        public float RemainingTime;
    }

    private List<ActiveModifier> _activeModifiers = new List<ActiveModifier>();
    private CharacterStatsBase _stats;
    private PlayerStats _playerStats; // A specific reference to PlayerStats for event handling

    private void Awake()
    {
        _stats = GetComponent<CharacterStatsBase>();
        // Try to get the PlayerStats component. If this is an enemy, it will be null, which is intended.
        _playerStats = _stats as PlayerStats;
    }

    private void Update()
    {
        // Loop backwards to safely remove items from the list while iterating.
        for (int i = _activeModifiers.Count - 1; i >= 0; i--)
        {
            ActiveModifier activeMod = _activeModifiers[i];
            activeMod.RemainingTime -= Time.deltaTime;

            if (activeMod.RemainingTime <= 0)
            {
                // Timer has expired, so remove the modifier.
                RemoveModifier(activeMod.TargetStat, activeMod.Modifier);
                _activeModifiers.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Adds a new stat modifier to a character. Handles stacking by refreshing duration.
    /// </summary>
    public void AddModifier(Stat stat, StatModifier modifier)
    {
        // --- Stacking Logic ---
        // Check if a modifier from the same source already exists.
        var existingModifier = _activeModifiers.FirstOrDefault(m => m.Modifier.Source == modifier.Source);
        if (existingModifier != null)
        {
            // If it exists, remove the old one before adding the new one to refresh the timer.
            _activeModifiers.Remove(existingModifier);
            existingModifier.TargetStat.RemoveModifier(existingModifier.Modifier);
        }

        // Add the modifier to the stat's internal list.
        stat.AddModifier(modifier);

        // If the modifier has a duration, add it to our active list to be tracked.
        if (modifier.Duration > 0)
        {
            _activeModifiers.Add(new ActiveModifier { TargetStat = stat, Modifier = modifier, RemainingTime = modifier.Duration });
        }

        // --- Event Firing Logic ---
        // Check if this is the player and if the modified stat was Sense.
        if (_playerStats != null && stat == _playerStats.Sense)
        {
            // Call the public method on PlayerStats to safely fire the event.
            _playerStats.NotifyCoreStatChanged();
        }
    }

    /// <summary>
    /// Manually removes a stat modifier from a character (e.g., when a toggleable passive is turned off).
    /// </summary>
    public void RemoveModifier(Stat stat, StatModifier modifier)
    {
        stat.RemoveModifier(modifier);

        // --- Event Firing Logic ---
        // Check if this is the player and if the modified stat was Sense.
        if (_playerStats != null && stat == _playerStats.Sense)
        {
            // Call the public method to ensure the GameManager is updated.
            _playerStats.NotifyCoreStatChanged();
        }
    }
}