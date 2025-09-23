using UnityEngine;

// A static utility class for handling all damage calculations.
public static class DamageCalculator
{
    /// <summary>
    /// Calculates final damage after considering stats, resistances, and critical hits.
    /// </summary>
    public static float CalculateDamage(CharacterStatsBase attackerStats, CharacterStatsBase defenderStats, float baseDamage, DamageType damageType, bool isGuaranteedCrit = false, bool canCrit = true)
    {
        float totalDamage = baseDamage;

        // 1. Add bonus damage from the attacker's primary stat.
        if (damageType == DamageType.Physical)
        {
            totalDamage += attackerStats.Strength.Value;
        }
        else // All magical damage types scale with Intelligence
        {
            totalDamage += attackerStats.Intelligence.Value;
        }

        // 2. Check for a critical hit, if allowed.
        if (canCrit)
        {
            float critChance = Mathf.Sqrt(attackerStats.Sense.Value / 2500f);
            bool isCriticalHit = isGuaranteedCrit || Random.value <= critChance;

            if (isCriticalHit)
            {
                totalDamage *= attackerStats.CriticalDamage.Value;
                Debug.Log("CRITICAL HIT!");
            }
        }

        // 3. Calculate damage reduction from the defender's appropriate resistances.
        float totalResistance = 0;
        if (damageType == DamageType.Physical)
        {
            totalResistance = defenderStats.Armor.Value;
        }
        else
        {
            totalResistance = defenderStats.MagicResistance.Value;
            switch (damageType)
            {
                case DamageType.Fire:
                    totalResistance += defenderStats.FireResistance.Value;
                    break;
                case DamageType.Water:
                    totalResistance += defenderStats.WaterResistance.Value;
                    break;
                case DamageType.Nature:
                    totalResistance += defenderStats.NatureResistance.Value;
                    break;
                case DamageType.Hex:
                    totalResistance += defenderStats.HexResistance.Value;
                    break;
                case DamageType.Necro:
                    totalResistance += defenderStats.NecroResistance.Value;
                    break;
                case DamageType.Wind:
                    totalResistance += defenderStats.WindResistance.Value;
                    break;
                case DamageType.Lightning:
                    totalResistance += defenderStats.LightningResistance.Value;
                    break;
            }
        }

        // 4. Apply the resistance using the diminishing returns formula.
        float damageMultiplier = 100f / (100f + totalResistance);
        float finalDamage = totalDamage * damageMultiplier;

        return Mathf.Max(1, finalDamage); // Ensure at least 1 damage is always dealt.
    }
}