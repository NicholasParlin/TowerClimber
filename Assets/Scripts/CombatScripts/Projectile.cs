using UnityEngine;

// This is the base script for all projectiles.
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 5f; // Time in seconds before the projectile is automatically returned to the pool.
    [SerializeField] private string poolTag; // The tag of this projectile in the ObjectPooler.

    [Header("VFX on Hit")]
    [Tooltip("The pool tag of the visual effect to spawn on impact (e.g., 'HitSparkVFX'). Leave empty for no effect.")]
    [SerializeField] private string hitVFXPoolTag;

    // This is a public property that allows other scripts to read the private poolTag variable.
    public string PoolTag => poolTag;

    private Rigidbody _rb;
    private CharacterStatsBase _ownerStats; // The stats of the character who fired this projectile.

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Initializes and fires the projectile. This is called by the skill that spawns it.
    /// </summary>
    public void Fire(Vector3 direction, CharacterStatsBase owner)
    {
        _ownerStats = owner;
        transform.forward = direction;

        _rb.WakeUp();
        _rb.AddForce(direction * speed, ForceMode.VelocityChange);

        Invoke(nameof(ReturnToPool), lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Prevent the projectile from colliding with its owner.
        if (_ownerStats != null && other.gameObject == _ownerStats.gameObject)
        {
            return;
        }

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            // If a VFX tag is specified, get a VFX object from the pool at the impact point.
            if (!string.IsNullOrEmpty(hitVFXPoolTag))
            {
                ObjectPooler.Instance.GetFromPool(hitVFXPoolTag, transform.position, Quaternion.identity);
            }

            // This is where you would get the base damage from the skill that fired the projectile.
            // For now, we'll use a placeholder value.
            float baseDamage = 10f;
            float finalDamage = DamageCalculator.CalculateDamage(_ownerStats, other.GetComponent<CharacterStatsBase>(), baseDamage, DamageType.Physical);

            enemyHealth.TakeDamage(finalDamage);
        }

        // The projectile has hit something, so return it to the pool.
        ReturnToPool();
    }

    /// <summary>
    /// Deactivates the projectile and returns it to the Object Pooler.
    /// </summary>
    private void ReturnToPool()
    {
        CancelInvoke();
        _rb.Sleep();
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }
}