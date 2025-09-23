using UnityEngine;

[CreateAssetMenu(fileName = "New Life Steal Effect", menuName = "Gameplay Effects/Life Steal")]
public class LifeStealEffect : GameplayEffect
{
    [Header("Damage Settings")]
    public float baseDamage;
    public DamageType damageType;
    public bool isGuaranteedCrit = false;
    public bool canCrit = true;

    [Header("Life Steal Settings")]
    [Tooltip("The percentage of damage dealt that is returned to the caster as health.")]
    [Range(0f, 1f)]
    public float lifestealPercentage = 0.25f;

    // UPDATED: Method signature now matches the base class.
    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        CharacterStatsBase casterStats = caster.GetComponent<CharacterStatsBase>();
        CharacterStatsBase targetStats = target.GetComponent<CharacterStatsBase>();
        EnemyHealth targetHealth = target.GetComponent<EnemyHealth>();
        PlayerStats casterPlayerStats = caster.GetComponent<PlayerStats>();

        if (casterStats == null || targetStats == null || targetHealth == null || casterPlayerStats == null)
        {
            Debug.LogWarning("LifeStealEffect: Caster or Target is missing a required component.");
            return;
        }

        float finalDamage = DamageCalculator.CalculateDamage(
            casterStats,
            targetStats,
            baseDamage,
            damageType,
            isGuaranteedCrit,
            canCrit
        );

        targetHealth.TakeDamage(finalDamage);

        float healthToRestore = finalDamage * lifestealPercentage;

        casterPlayerStats.RestoreHealth(healthToRestore);
        Debug.Log($"{caster.name} lifesteals {healthToRestore} health from {target.name}.");
    }
}