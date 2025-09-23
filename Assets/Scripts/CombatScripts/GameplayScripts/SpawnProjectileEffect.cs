using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Spawn Projectile Effect", menuName = "Gameplay Effects/Spawn Projectile")]
public class SpawnProjectileEffect : GameplayEffect
{
    [Header("Projectile Settings")]
    [Tooltip("The projectile prefab to spawn.")]
    public GameObject projectilePrefab;

    [Tooltip("The number of projectiles to fire in a single volley.")]
    public int projectileCount = 1;

    [Tooltip("The position and rotation offset from the caster's transform.")]
    public Vector3 spawnOffset;

    [Header("Combat Data")]
    [Tooltip("The base damage of the projectile.")]
    public float baseDamage = 10f;
    [Tooltip("The type of damage the projectile deals.")]
    public DamageType damageType = DamageType.Physical;
    [Tooltip("Should this projectile always be a critical hit?")]
    public bool isGuaranteedCrit = false;
    [Tooltip("The amount of stagger damage this projectile deals.")]
    public float staggerPower = 10f;


    // UPDATED: Method signature now matches the base class.
    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("SpawnProjectileEffect: Projectile prefab is not assigned!");
            return;
        }

        string poolTag = projectilePrefab.GetComponent<Projectile>()?.PoolTag;
        if (string.IsNullOrEmpty(poolTag))
        {
            Debug.LogError($"The projectile prefab for {projectilePrefab.name} is missing a Projectile script or a pool tag!");
            return;
        }

        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 spawnPosition = caster.transform.position + caster.transform.TransformDirection(spawnOffset);
            Quaternion spawnRotation = caster.transform.rotation;

            GameObject projectileGO = ObjectPooler.Instance.GetFromPool(poolTag, spawnPosition, spawnRotation);
            if (projectileGO != null)
            {
                Projectile projectile = projectileGO.GetComponent<Projectile>();
                projectile.Fire(
                    caster.transform.forward,
                    caster.GetComponent<CharacterStatsBase>(),
                    baseDamage,
                    damageType,
                    isGuaranteedCrit,
                    staggerPower
                );
            }
        }
    }
}