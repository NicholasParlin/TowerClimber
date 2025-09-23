using UnityEngine;

[CreateAssetMenu(fileName = "New Apply Status Effect", menuName = "Gameplay Effects/Apply Status Effect")]
public class ApplyStatusEffect : GameplayEffect
{
    [Header("Status Effect Settings")]
    [Tooltip("The StatusEffect asset to apply to the target.")]
    public StatusEffect statusEffectToApply;

    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        if (statusEffectToApply == null)
        {
            Debug.LogWarning("ApplyStatusEffect has no StatusEffect asset assigned.");
            return;
        }

        BuffManager targetBuffManager = target.GetComponent<BuffManager>();
        if (targetBuffManager == null)
        {
            Debug.LogWarning($"ApplyStatusEffect: Target {target.name} has no BuffManager component.");
            return;
        }

        // CORRECTED: Now correctly passes the sourceSkill to the BuffManager.
        targetBuffManager.ApplyStatusEffect(statusEffectToApply, caster, sourceSkill);
    }
}