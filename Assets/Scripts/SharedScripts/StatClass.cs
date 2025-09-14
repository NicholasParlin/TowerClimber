using System.Collections.Generic;
using System.Collections.ObjectModel;

// Defines a single character stat and the modifiers that can affect it.
[System.Serializable]
public class Stat
{
    public float BaseValue;

    // A read-only list for other scripts to safely view modifiers
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;
    private readonly List<StatModifier> _statModifiers;

    // Calculates the final value by adding all modifiers to the base value
    public virtual float Value
    {
        get
        {
            float finalValue = BaseValue;
            _statModifiers.ForEach(x => finalValue += x.Value);
            return finalValue;
        }
    }

    public Stat(float baseValue)
    {
        BaseValue = baseValue;
        _statModifiers = new List<StatModifier>();
        StatModifiers = _statModifiers.AsReadOnly();
    }

    public virtual void AddModifier(StatModifier mod)
    {
        _statModifiers.Add(mod);
    }

    public virtual bool RemoveModifier(StatModifier mod)
    {
        return _statModifiers.Remove(mod);
    }

    // Helper method to remove all modifiers from a specific source (e.g., a skill wears off)
    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;
        for (int i = _statModifiers.Count - 1; i >= 0; i--)
        {
            if (_statModifiers[i].Source == source)
            {
                didRemove = true;
                _statModifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }
}

// Defines a buff or debuff that can be applied to a Stat
[System.Serializable]
public class StatModifier
{
    public readonly float Value;
    public readonly float Duration;
    public readonly object Source; // The skill, item, or effect that applied this modifier

    /// <summary>
    /// Creates a StatModifier.
    /// </summary>
    /// <param name="value">The value to add to the stat.</param>
    /// <param name="duration">The duration in seconds. Use 0 for a permanent modifier.</param>
    /// <param name="source">The object that created this modifier (e.g., the Skill asset).</param>
    public StatModifier(float value, float duration, object source)
    {
        Value = value;
        Duration = duration;
        Source = source;
    }
}