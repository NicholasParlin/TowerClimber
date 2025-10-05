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

// The class declaration should be "public class Skill : ScriptableObject"
// If your file has "public abstract class Item : ScriptableObject", you have opened the wrong file.
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
    // NEW PROPERTY: Add this boolean to your Skill script.
    [Tooltip("If checked, the player cannot be staggered or knocked down while activating this skill.")]
    public bool hasSuperArmor = false;

    [Header("Resource Costs")]
    public float manaCost = 0;
    public float energyCost = 0;
    public float healthCost = 0;
    public float anguishCost = 0;

    [Header("Gameplay Effects (for Active Skills)")]
    public List<EffectInSequence> effectSequence;

    [Header("AI Behavior")]
    [Tooltip("The base score or 'desire' for an AI to use this skill.")]
    public float baseUtilityScore = 20f;
    [Tooltip("A list of considerations that will modify the base score.")]
    public List<AIAction> aiActions;

    public IEnumerator Activate(SkillManagerBase skillManager, GameObject caster, GameObject target)
    {
        foreach (EffectInSequence sequenceItem in effectSequence)
        {
            if (sequenceItem.effect != null)
            {
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