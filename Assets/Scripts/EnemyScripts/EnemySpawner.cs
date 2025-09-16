using UnityEngine;

// This component spawns enemies from the Object Pooler when the player is nearby.
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("The unique pool tag of the enemy prefab to spawn (e.g., 'GoblinScrapper').")]
    [SerializeField] private string enemyPoolTag;
    [Tooltip("The radius around the spawner in which the player must be for enemies to spawn.")]
    [SerializeField] private float spawnTriggerRadius = 20f;
    [SerializeField] private float spawnInterval = 5f;

    private Transform _playerTransform;
    private float _spawnTimer;

    private void Start()
    {
        // Find the player's transform at the start.
        _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_playerTransform == null)
        {
            Debug.LogError("Player not found! Enemy Spawner will not function.");
            this.enabled = false;
        }
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        // Check the distance to the player.
        if (Vector3.Distance(transform.position, _playerTransform.position) <= spawnTriggerRadius)
        {
            // Player is in range, handle spawning.
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0)
            {
                SpawnEnemy();
                _spawnTimer = spawnInterval;
            }
        }
    }

    private void SpawnEnemy()
    {
        // Get an enemy from the pool and place it at the spawner's location.
        ObjectPooler.Instance.GetFromPool(enemyPoolTag, transform.position, transform.rotation);
    }
}