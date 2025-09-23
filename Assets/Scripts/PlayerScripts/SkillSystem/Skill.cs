using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EffectInSequence
{
    public GameplayEffect effect;
    [Tooltip("The delay in seconds AFTER this effect executes.")]
    public float delayAfterEffect = 0f;
}

public class Skill : ScriptableObject
{
    [Header("Core Information")]
    public string skillName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    public Archetype archetype;

    [Header("Skill Type")]
    [Tooltip("If this is a passive skill, assign the permanent Status Effect it applies here.")]
    public StatusEffect passiveEffectToApply;
    [Tooltip("If checked, this skill will always target the caster.")]
    public bool isSelfCast = false;

    [Header("Mechanics")]
    public float baseActivationTime = 0.5f;
    public float cooldown = 1f;

    [Header("Resource Costs")]
    public float manaCost = 0;
    public float energyCost = 0;
    public float healthCost = 0;
    public float anguishCost = 0;

    [Header("Gameplay Effects (for Active Skills)")]
    public List<EffectInSequence> effectSequence;

    public IEnumerator Activate(SkillManagerBase skillManager, GameObject caster, GameObject target)
    {
        foreach (EffectInSequence sequenceItem in effectSequence)
        {
            if (sequenceItem.effect != null)
            {
                // CORRECTED: Now passes 'this' as the sourceSkill.
                sequenceItem.effect.Execute(this, caster, target);
            }

            if (sequenceItem.delayAfterEffect > 0)
            {
                yield return new WaitForSeconds(sequenceItem.delayAfterEffect);
            }
        }

        skillManager.ApplyCooldown(this);
    }
}