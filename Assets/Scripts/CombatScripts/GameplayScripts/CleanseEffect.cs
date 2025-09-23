using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Cleanse Effect", menuName = "Gameplay Effects/Cleanse")]
public class CleanseEffect : GameplayEffect
{
    [Header("Cleanse Settings")]
    [Tooltip("The categories of harmful effects that this skill will remove.")]
    public List<EffectCategory> categoriesToCleanse;

    // UPDATED: Method signature now matches the base class.
    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        if (categoriesToCleanse == null || categoriesToCleanse.Count == 0)
        {
            Debug.LogWarning("CleanseEffect has no categories assigned to cleanse.");
            return;
        }

        BuffManager targetBuffManager = target.GetComponent<BuffManager>();
        if (targetBuffManager == null)
        {
            Debug.LogWarning($"CleanseEffect: Target {target.name} has no BuffManager component.");
            return;
        }

        targetBuffManager.Cleanse(categoriesToCleanse);
        Debug.Log($"{caster.name} cleansed {string.Join(", ", categoriesToCleanse)} from {target.name}.");
    }
}