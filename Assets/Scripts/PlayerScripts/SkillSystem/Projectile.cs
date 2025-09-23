using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private string poolTag;

    [Header("VFX on Hit")]
    [Tooltip("The pool tag of the visual effect to spawn on impact.")]
    [SerializeField] private string hitVFXPoolTag;

    public string PoolTag => poolTag;

    private Rigidbody _rb;
    private CharacterStatsBase _ownerStats;
    // We will now store the damage data directly, not the skill asset.
    private float _baseDamage;
    private DamageType _damageType;
    private bool _isGuaranteedCrit;
    private float _staggerPower;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Initializes and fires the projectile with all necessary combat data.
    /// </summary>
    public void Fire(Vector3 direction, CharacterStatsBase owner, float baseDamage, DamageType damageType, bool isGuaranteedCrit, float staggerPower)
    {
        _ownerStats = owner;
        _baseDamage = baseDamage;
        _damageType = damageType;
        _isGuaranteedCrit = isGuaranteedCrit;
        _staggerPower = staggerPower;

        transform.forward = direction;
        _rb.WakeUp();
        _rb.AddForce(direction * speed, ForceMode.VelocityChange);

        Invoke(nameof(ReturnToPool), lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_ownerStats != null && other.gameObject == _ownerStats.gameObject)
        {
            return;
        }

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        CharacterStatsBase defenderStats = other.GetComponent<CharacterStatsBase>();

        if (enemyHealth != null && defenderStats != null)
        {
            if (!string.IsNullOrEmpty(hitVFXPoolTag))
            {
                ObjectPooler.Instance.GetFromPool(hitVFXPoolTag, transform.position, Quaternion.identity);
            }

            float finalDamage = DamageCalculator.CalculateDamage(
                _ownerStats,
                defenderStats,
                _baseDamage,
                _damageType,
                _isGuaranteedCrit
            );
            enemyHealth.TakeDamage(finalDamage);
            defenderStats.TakeStaggerDamage(_staggerPower);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        CancelInvoke();
        _rb.Sleep();
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }
}