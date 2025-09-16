using UnityEngine;

// A specialized skill type for skills that fire a projectile.
[CreateAssetMenu(fileName = "New Projectile Skill", menuName = "Skills/Projectile Skill")]
public class ProjectileSkill : Skill
{
    [Header("Projectile Skill Settings")]
    [Tooltip("The base damage of the projectile. This will be modified by the user's stats.")]
    public float baseDamage = 10f;
    [Tooltip("The type of damage the projectile deals.")]
    public DamageType damageType;

    public override void Activate(GameObject user)
    {
        // Ensure this skill has a projectile prefab assigned.
        if (projectilePrefab == null)
        {
            Debug.LogError($"Projectile Skill '{skillName}' has no projectile prefab assigned!");
            return;
        }

        // Get the pool tag from the projectile's script.
        string poolTag = projectilePrefab.GetComponent<Projectile>()?.PoolTag;
        if (string.IsNullOrEmpty(poolTag))
        {
            Debug.LogError($"The projectile prefab for '{skillName}' is missing a Projectile script or a pool tag!");
            return;
        }

        // --- This is the core logic ---
        // 1. Get a projectile from the object pool.
        GameObject projectileGO = ObjectPooler.Instance.GetFromPool(poolTag, user.transform.position, user.transform.rotation);

        if (projectileGO != null)
        {
            // 2. Get the projectile's script component.
            Projectile projectile = projectileGO.GetComponent<Projectile>();

            // 3. Initialize and fire the projectile.
            // We pass it the direction the user is facing and a reference to the user's stats.
            projectile.Fire(user.transform.forward, user.GetComponent<CharacterStatsBase>());

            // A more advanced version would pass the baseDamage and damageType to the projectile
            // so it can handle the damage calculation itself upon collision.
        }
    }
}