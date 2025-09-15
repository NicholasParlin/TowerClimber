using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This component manages all temporary stat modifiers (buffs and debuffs) for a character.
// It can be attached to both the player and enemies.
[RequireComponent(typeof(CharacterStatsBase))]
public class BuffManager : MonoBehaviour
{
    // A private class to track active modifiers and their remaining time.
    private class ActiveModifier
    {
        public Stat TargetStat;
        public StatModifier Modifier;
        public float TimeRemaining;

        public ActiveModifier(Stat stat, StatModifier modifier)
        {
            TargetStat = stat;
            Modifier = modifier;
            TimeRemaining = modifier.Duration;
        }
    }

    private List<ActiveModifier> _activeModifiers = new List<ActiveModifier>();
    private CharacterStatsBase _stats;
    private PlayerStats _playerStats; // This will be null if the component is on an enemy.

    private void Awake()
    {
        // Get a reference to the base stats component.
        _stats = GetComponent<CharacterStatsBase>();
        // Also try to get a reference to the player-specific stats component.
        _playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        // We iterate backwards to safely remove items from the list while iterating.
        for (int i = _activeModifiers.Count - 1; i >= 0; i--)
        {
            ActiveModifier activeMod = _activeModifiers[i];

            // A negative duration indicates a permanent modifier (e.g., from a title or gear), so we ignore it.
            if (activeMod.TimeRemaining < 0) continue;

            // Tick down the timer for temporary buffs/debuffs.
            activeMod.TimeRemaining -= Time.deltaTime;

            if (activeMod.TimeRemaining <= 0)
            {
                // Timer has expired, so remove the modifier from the stat and our tracking list.
                RemoveModifier(activeMod.TargetStat, activeMod.Modifier);
                _activeModifiers.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Adds a new stat modifier to a character, handling stacking logic.
    /// </summary>
    public void AddModifier(Stat stat, StatModifier modifier)
    {
        // Stacking logic: If a modifier from the same source already exists on the same stat,
        // remove the old one first. This effectively refreshes the duration.
        var existingMod = _activeModifiers.FirstOrDefault(m => m.Modifier.Source == modifier.Source && m.TargetStat == stat);
        if (existingMod != null)
        {
            RemoveModifier(existingMod.TargetStat, existingMod.Modifier);
            _activeModifiers.Remove(existingMod);
        }

        // Add the new modifier to the stat's internal list and to our tracking list.
        stat.AddModifier(modifier);
        _activeModifiers.Add(new ActiveModifier(stat, modifier));

        // Check if this change needs to be reported to the GameManager.
        CheckForSenseChangeEvent(stat);
    }

    /// <summary>
    /// Removes a specific stat modifier from a character.
    /// </summary>
    public void RemoveModifier(Stat stat, StatModifier modifier)
    {
        stat.RemoveModifier(modifier);

        // Check if this change needs to be reported to the GameManager.
        CheckForSenseChangeEvent(stat);
    }

    /// <summary>
    /// Removes all modifiers that came from a specific source (e.g., an unequipped Title).
    /// </summary>
    public void RemoveAllModifiersFromSource(object source)
    {
        // Find all active modifiers in our list that match the source.
        var modsToRemove = _activeModifiers.Where(m => m.Modifier.Source == source).ToList();

        foreach (var activeMod in modsToRemove)
        {
            // Remove the modifier from the stat and from our tracking list.
            RemoveModifier(activeMod.TargetStat, activeMod.Modifier);
            _activeModifiers.Remove(activeMod);
        }
    }

    /// <summary>
    /// A helper method to safely notify the GameManager of a change to the player's Sense stat.
    /// </summary>
    private void CheckForSenseChangeEvent(Stat stat)
    {
        // Only proceed if this component is on the player (_playerStats is not null)
        // and if the stat that changed was the Sense stat.
        if (_playerStats != null && stat == _playerStats.Sense)
        {
            // Call the public notifier method on the PlayerStats script.
            _playerStats.NotifyCoreStatChanged();
        }
    }
}
