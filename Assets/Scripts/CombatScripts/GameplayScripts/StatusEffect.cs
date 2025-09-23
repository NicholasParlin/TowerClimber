using UnityEngine;
using System.Collections.Generic;

public enum StackingRule
{
    RefreshDuration,
}

// NEW: An enum to categorize the type of a status effect.
public enum EffectCategory
{
    StatBuff,
    StatDebuff,
    DamageOverTime,
    HealOverTime,
    HardCC // (e.g., Stun, Root)
}

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Gameplay Effects/Status Effect")]
public class StatusEffect : ScriptableObject
{
    [Header("Core Information")]
    public string effectName;
    [TextArea] public string description;
    public Sprite icon;
    public StackingRule stackingRule = StackingRule.RefreshDuration;
    public EffectCategory category; // NEW PROPERTY

    [Header("Effect Details")]
    public float duration;
    public GameObject vfxPrefab;

    [Header("Modifiers")]
    [Tooltip("The stat changes this effect applies while active.")]
    public List<StatBonus> modifiers;

    [Header("Tick Effect (for DoTs/HoTs)")]
    [Tooltip("An optional GameplayEffect that is applied every second.")]
    public GameplayEffect tickEffect;
}