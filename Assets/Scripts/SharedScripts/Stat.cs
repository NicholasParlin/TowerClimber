using System.Collections.Generic;
using System.Linq;

// An enum to define how a modifier is applied.
public enum ModifierType
{
    Flat,       // Adds a flat value (e.g., +10 Strength)
    Percentage  // Adds a percentage of the base value (e.g., +0.15 for 15% Armor)
}

// A class to represent a change to a stat.
[System.Serializable]
public class StatModifier
{
    public readonly float Value;
    public readonly float Duration;
    public readonly object Source; // The skill, item, or title that applied this
    public readonly ModifierType Type;

    public StatModifier(float value, float duration, object source, ModifierType type = ModifierType.Flat)
    {
        Value = value;
        Duration = duration;
        Source = source;
        Type = type;
    }
}

// This class defines a single, modifiable stat.
[System.Serializable]
public class Stat
{
    public float BaseValue;
    private readonly List<StatModifier> _statModifiers = new List<StatModifier>();

    public float Value
    {
        get
        {
            float finalValue = BaseValue;
            float percentageBonus = 0;

            // Apply flat modifiers first
            foreach (var modifier in _statModifiers.Where(m => m.Type == ModifierType.Flat))
            {
                finalValue += modifier.Value;
            }

            // Sum up all percentage modifiers
            foreach (var modifier in _statModifiers.Where(m => m.Type == ModifierType.Percentage))
            {
                percentageBonus += modifier.Value;
            }

            // Apply the total percentage bonus to the value after flat modifications.
            finalValue *= (1 + percentageBonus);

            return finalValue;
        }
    }

    public void AddModifier(StatModifier modifier)
    {
        _statModifiers.Add(modifier);
    }

    public void RemoveModifier(StatModifier modifier)
    {
        _statModifiers.Remove(modifier);
    }

    public bool RemoveAllModifiersFromSource(object source)
    {
        int numRemoved = _statModifiers.RemoveAll(mod => mod.Source == source);
        return numRemoved > 0;
    }
}