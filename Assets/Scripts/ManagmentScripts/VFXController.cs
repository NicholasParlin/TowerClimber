using UnityEngine;
using System.Collections;

// This script is attached to a VFX prefab (e.g., a hit spark, an explosion).
// It handles playing the particle effect and automatically returning to the pool.
[RequireComponent(typeof(ParticleSystem))]
public class VFXController : MonoBehaviour
{
    [Header("VFX Settings")]
    [Tooltip("The unique tag of this VFX in the ObjectPooler. This MUST match a tag in the ObjectPooler's list.")]
    [SerializeField] private string poolTag;

    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    // This method is called when the object is enabled (i.e., retrieved from the pool).
    private void OnEnable()
    {
        // Play the particle system and start the coroutine to return it to the pool.
        if (_particleSystem != null)
        {
            _particleSystem.Play();
            StartCoroutine(ReturnToPoolAfterDuration());
        }
    }

    private IEnumerator ReturnToPoolAfterDuration()
    {
        // Wait for the duration of the particle system's main module.
        // This ensures the effect has time to play fully before being deactivated.
        yield return new WaitForSeconds(_particleSystem.main.duration);

        // Return the object to the pool.
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }
}