using UnityEngine;

[CreateAssetMenu(fileName = "New Deal Damage Effect", menuName = "Gameplay Effects/Deal Damage")]
public class DealDamageEffect : GameplayEffect
{
    [Header("Damage Settings")]
    public float baseDamage;
    public DamageType damageType;
    public bool isGuaranteedCrit = false;
    public bool canCrit = true;

    // UPDATED: Method signature now matches the base class.
    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        CharacterStatsBase casterStats = caster.GetComponent<CharacterStatsBase>();
        CharacterStatsBase targetStats = target.GetComponent<CharacterStatsBase>();
        EnemyHealth targetHealth = target.GetComponent<EnemyHealth>();

        if (casterStats == null || targetStats == null || targetHealth == null)
        {
            Debug.LogWarning("DealDamageEffect: Caster or Target is missing a required stats component.");
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
    }
}