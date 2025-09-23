using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuffManager : MonoBehaviour
{
    private class ActiveStatusEffect
    {
        public StatusEffect Effect { get; }
        public GameObject Caster { get; }
        public Skill SourceSkill { get; } // NEW: Reference to the skill that applied the effect
        public float TimeRemaining { get; set; }
        public float TimeSinceLastTick { get; set; }

        public ActiveStatusEffect(StatusEffect effect, GameObject caster, Skill sourceSkill)
        {
            Effect = effect;
            Caster = caster;
            SourceSkill = sourceSkill;
            TimeRemaining = effect.duration;
            TimeSinceLastTick = 0f;
        }
    }

    private List<ActiveStatusEffect> _activeStatusEffects = new List<ActiveStatusEffect>();
    private CharacterStatsBase _stats;
    private PlayerStats _playerStats;

    private void Awake()
    {
        _stats = GetComponent<CharacterStatsBase>();
        _playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        for (int i = _activeStatusEffects.Count - 1; i >= 0; i--)
        {
            ActiveStatusEffect activeEffect = _activeStatusEffects[i];

            if (activeEffect.Effect.duration > 0)
            {
                activeEffect.TimeRemaining -= Time.deltaTime;
                if (activeEffect.TimeRemaining <= 0)
                {
                    RemoveStatusEffect(activeEffect);
                    _activeStatusEffects.RemoveAt(i);
                    continue;
                }
            }

            if (activeEffect.Effect.tickEffect != null)
            {
                activeEffect.TimeSinceLastTick += Time.deltaTime;
                if (activeEffect.TimeSinceLastTick >= 1.0f)
                {
                    // CORRECTED: Now passes the source skill to the tick effect.
                    activeEffect.Effect.tickEffect.Execute(activeEffect.SourceSkill, activeEffect.Caster, this.gameObject);
                    activeEffect.TimeSinceLastTick -= 1.0f;
                }
            }
        }
    }

    // UPDATED: Method now requires the sourceSkill.
    public void ApplyStatusEffect(StatusEffect effect, GameObject caster, Skill sourceSkill)
    {
        if (effect.stackingRule == StackingRule.RefreshDuration)
        {
            var existingEffect = _activeStatusEffects.FirstOrDefault(e => e.Effect == effect);
            if (existingEffect != null)
            {
                existingEffect.TimeRemaining = effect.duration;
                return;
            }
        }

        ActiveStatusEffect newActiveEffect = new ActiveStatusEffect(effect, caster, sourceSkill);
        _activeStatusEffects.Add(newActiveEffect);

        foreach (var bonus in effect.modifiers)
        {
            var statModifierInstance = new StatModifier(bonus.value, bonus.type, effect);
            _stats.GetStat(bonus.statToBuff)?.AddModifier(statModifierInstance);
        }

        NotifyPlayerStatsChanged();
    }

    private void RemoveStatusEffect(ActiveStatusEffect effectToRemove)
    {
        _stats.RemoveAllModifiersFromSource(effectToRemove.Effect);
        NotifyPlayerStatsChanged();
    }

    public void Cleanse(List<EffectCategory> categoriesToCleanse)
    {
        List<ActiveStatusEffect> effectsToRemove = _activeStatusEffects
            .Where(activeEffect => categoriesToCleanse.Contains(activeEffect.Effect.category))
            .ToList();

        foreach (var effect in effectsToRemove)
        {
            RemoveStatusEffect(effect);
            _activeStatusEffects.Remove(effect);
        }
    }

    public void ClearAllModifiers()
    {
        for (int i = _activeStatusEffects.Count - 1; i >= 0; i--)
        {
            RemoveStatusEffect(_activeStatusEffects[i]);
            _activeStatusEffects.RemoveAt(i);
        }
    }

    private void NotifyPlayerStatsChanged()
    {
        if (_playerStats != null)
        {
            _playerStats.NotifyStatsUpdated();
        }
    }

    // --- LEGACY METHOD SUPPORT ---
    public void AddModifier(Stat stat, StatModifier modifier)
    {
        stat.AddModifier(modifier);
        NotifyPlayerStatsChanged();
    }

    public void RemoveAllModifiersFromSource(object source)
    {
        _stats.RemoveAllModifiersFromSource(source);
        NotifyPlayerStatsChanged();
    }
}