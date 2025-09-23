using System;
using System.Collections.Generic;
using System.Linq;

public enum ModifierType
{
    Flat,
    Percentage
}

[System.Serializable]
public class StatModifier
{
    public readonly float Value;
    public readonly object Source;
    public readonly ModifierType Type;

    public StatModifier(float value, ModifierType type, object source)
    {
        Value = value;
        Type = type;
        Source = source;
    }
}

[System.Serializable]
public class Stat
{
    // This event will fire whenever the final calculated value of the stat changes.
    public event Action OnValueChanged;

    private float _baseValue;
    public float BaseValue
    {
        get { return _baseValue; }
        set
        {
            _baseValue = value;
            OnValueChanged?.Invoke();
        }
    }

    private readonly List<StatModifier> _statModifiers = new List<StatModifier>();

    public float Value
    {
        get
        {
            float finalValue = BaseValue;
            float percentageBonus = 0;

            foreach (var modifier in _statModifiers.Where(m => m.Type == ModifierType.Flat))
            {
                finalValue += modifier.Value;
            }

            foreach (var modifier in _statModifiers.Where(m => m.Type == ModifierType.Percentage))
            {
                percentageBonus += modifier.Value;
            }

            finalValue *= (1 + percentageBonus);

            return finalValue;
        }
    }

    public void AddModifier(StatModifier modifier)
    {
        _statModifiers.Add(modifier);
        OnValueChanged?.Invoke();
    }

    public void RemoveModifier(StatModifier modifier)
    {
        _statModifiers.Remove(modifier);
        OnValueChanged?.Invoke();
    }

    public bool RemoveAllModifiersFromSource(object source)
    {
        int numRemoved = _statModifiers.RemoveAll(mod => mod.Source == source);
        if (numRemoved > 0)
        {
            OnValueChanged?.Invoke();
            return true;
        }
        return false;
    }
}