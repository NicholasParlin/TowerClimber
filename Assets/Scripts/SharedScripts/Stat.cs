using System.Collections.Generic;
using System.Linq;

// An enum to define how a modifier is applied.
public enum ModifierType
{
    Flat,       // Adds a flat value (e.g., +10 Strength)
    Percentage  // Adds a percentage (e.g., +15% Armor)
}

// A class to represent a change to a stat. It's serializable so it can be viewed
// in the Inspector if needed, though it's primarily used in code.
[System.Serializable]
public class StatModifier
{
    public readonly float Value;
    public readonly float Duration;
    public readonly object Source; // The skill, item, or title that applied this
    public readonly ModifierType Type;

    // Constructor for creating a new modifier.
    public StatModifier(float value, float duration, object source, ModifierType type = ModifierType.Flat)
    {
        Value = value;
        Duration = duration;
        Source = source;
        Type = type;
    }
}

// This class defines a single, modifiable stat like Strength or Dexterity.
// It is not a MonoBehaviour; it's a data container used by CharacterStatsBase.
[System.Serializable]
public class Stat
{
    public float BaseValue;
    private readonly List<StatModifier> _statModifiers = new List<StatModifier>();

    // This is the public-facing value that should be used for all calculations.
    // It automatically calculates the final value including all modifiers.
    public float Value
    {
        get
        {
            float finalValue = BaseValue;
            float percentageBonusTotal = 0;

            // Apply all flat modifiers first.
            foreach (var modifier in _statModifiers.Where(m => m.Type == ModifierType.Flat))
            {
                finalValue += modifier.Value;
            }

            // Sum up all percentage modifiers.
            foreach (var modifier in _statModifiers.Where(m => m.Type == ModifierType.Percentage))
            {
                percentageBonusTotal += modifier.Value;
            }

            // Apply the total percentage bonus to the value after flat modifications.
            // e.g., if total percentage is 0.15 (for 15%), it multiplies by 1.15.
            finalValue *= (1 + percentageBonusTotal);

            return finalValue;
        }
    }

    /// <summary>
    /// Adds a new modifier to this stat's list.
    /// </summary>
    public void AddModifier(StatModifier modifier)
    {
        _statModifiers.Add(modifier);
    }

    /// <summary>
    /// Removes a specific modifier from this stat's list.
    /// </summary>
    public void RemoveModifier(StatModifier modifier)
    {
        _statModifiers.Remove(modifier);
    }

    /// <summary>
    /// Removes all modifiers that came from a specific source (e.g., an unequipped Title).
    /// </summary>
    /// <returns>True if any modifiers were removed.</returns>
    public bool RemoveAllModifiersFromSource(object source)
    {
        // Use RemoveAll for an efficient way to remove multiple items that match a condition.
        int numRemoved = _statModifiers.RemoveAll(mod => mod.Source == source);

        // Return true if we actually removed something.
        return numRemoved > 0;
    }
}