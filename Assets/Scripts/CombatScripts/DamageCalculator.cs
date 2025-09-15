using UnityEngine;

// A static utility class for handling all damage calculations.
public static class DamageCalculator
{
    /// <summary>
    /// Calculates the final damage after considering attacker's stats and defender's resistances.
    /// </summary>
    /// <param name="attackerStats">The stats of the character dealing damage.</param>
    /// <param name="defenderStats">The stats of the character receiving damage.</param>
    /// <param name="baseDamage">The base damage of the weapon or skill being used.</param>
    /// <param name="damageType">The type of damage (Physical or Magical).</param>
    /// <returns>The final, calculated damage amount.</returns>
    public static float CalculateDamage(CharacterStatsBase attackerStats, CharacterStatsBase defenderStats, float baseDamage, DamageType damageType)
    {
        float totalDamage = baseDamage;

        // 1. Add bonus damage from the attacker's primary stat.
        if (damageType == DamageType.Physical)
        {
            // Physical damage scales with Strength. (e.g., 1 Strength = +1 damage)
            totalDamage += attackerStats.Strength.Value;
        }
        else if (damageType == DamageType.Magical)
        {
            // Magical damage scales with Intelligence.
            totalDamage += attackerStats.Intelligence.Value;
        }

        // 2. Calculate damage reduction from the defender's resistance.
        float resistance = 0;
        if (damageType == DamageType.Physical)
        {
            resistance = defenderStats.Armor.Value;
        }
        else if (damageType == DamageType.Magical)
        {
            resistance = defenderStats.MagicResistance.Value;
        }

        // A common formula for damage reduction: Damage * (100 / (100 + Resistance))
        // This provides diminishing returns for stacking resistance.
        float damageMultiplier = 100f / (100f + resistance);
        float finalDamage = totalDamage * damageMultiplier;

        // 3. Ensure a minimum amount of damage is always dealt.
        return Mathf.Max(1, finalDamage);
    }
}