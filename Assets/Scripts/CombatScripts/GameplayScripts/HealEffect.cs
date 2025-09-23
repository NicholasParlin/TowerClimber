using UnityEngine;

// This effect is responsible for restoring health to a target.
[CreateAssetMenu(fileName = "New Heal Effect", menuName = "Gameplay Effects/Heal")]
public class HealEffect : GameplayEffect
{
    [Header("Heal Settings")]
    [Tooltip("The base amount of health to restore, before any stat modifications.")]
    public float baseHealthToRestore;

    // UPDATED: Method signature now matches the base class.
    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        PlayerStats targetStats = target.GetComponent<PlayerStats>();
        if (targetStats == null)
        {
            Debug.LogWarning($"HealEffect: Target {target.name} has no PlayerStats component to heal.");
            return;
        }

        targetStats.RestoreHealth(baseHealthToRestore);
        Debug.Log($"{caster.name} healed {target.name} for {baseHealthToRestore} health.");
    }
}