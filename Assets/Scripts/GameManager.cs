using UnityEngine;

// A central singleton to manage the game's global state, like floor number and enemy speed.
public class GameManager : MonoBehaviour
{
    // Singleton pattern: A static instance that can be accessed from anywhere.
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private int currentFloor = 1;

    [Header("Player Reference")]
    [Tooltip("Assign the player's GameObject here in the Inspector.")]
    [SerializeField] private PlayerStats playerStats;

    // --- Enemy Game Speed Calculation ---
    private float _baseEnemySpeedPerFloor = 0.10f; // 10% speed increase per floor
    private float _spikeFloorSpeedBonus = 0.10f;  // Additional 10% (for 20% total) on spike floors
    private float _cachedEnemyGameSpeed = 1.0f;   // The final, calculated speed multiplier that enemies will use.

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keeps the GameManager across scene loads.
        }
    }

    private void Start()
    {
        // Subscribe to the player's stat change event.
        // This is the core of our efficient, event-driven system.
        if (playerStats != null)
        {
            playerStats.OnCoreStatChanged += UpdateEnemySpeedCache;
        }

        // Calculate the initial value when the game starts.
        UpdateEnemySpeedCache();
    }

    private void OnDestroy()
    {
        // Always unsubscribe from events when this object is destroyed to prevent errors.
        if (playerStats != null)
        {
            playerStats.OnCoreStatChanged -= UpdateEnemySpeedCache;
        }
    }

    /// <summary>
    /// This method ONLY runs when the player's Sense stat changes (or the floor changes).
    /// It recalculates the global enemy speed and caches it.
    /// </summary>
    private void UpdateEnemySpeedCache()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats reference is not set on the GameManager!");
            return;
        }

        // 1. Calculate the floor-based speed bonus.
        float floorBonus = (currentFloor - 1) * _baseEnemySpeedPerFloor;
        // Add bonus for spike floors (5, 10, 25, 50)
        if (currentFloor == 5 || currentFloor == 10 || currentFloor == 25 || currentFloor == 50)
        {
            floorBonus += _spikeFloorSpeedBonus;
        }

        float currentEnemySpeed = 1.0f + floorBonus;

        // 2. Calculate the reduction from the player's Sense stat.
        // Each point of Sense reduces the final speed by 1%.
        float senseModifier = playerStats.Sense.Value * 0.01f;

        // 3. Calculate and cache the final value.
        float finalSpeed = currentEnemySpeed * (1 - senseModifier);

        // Clamp the speed to a minimum value to prevent enemies from stopping completely.
        _cachedEnemyGameSpeed = Mathf.Max(finalSpeed, 0.25f);

        Debug.Log($"Enemy game speed recalculated and cached to: {_cachedEnemyGameSpeed}");
    }

    /// <summary>
    /// This is the public method that all enemies will call to get the current speed multiplier.
    /// It's very fast because it just returns the pre-calculated, cached value.
    /// </summary>
    public float GetCurrentEnemyGameSpeed()
    {
        return _cachedEnemyGameSpeed;
    }

    /// <summary>
    /// Call this method when the player ascends to the next floor.
    /// </summary>
    public void GoToNextFloor()
    {
        currentFloor++;
        // Recalculate the speed cache since the floor number has changed.
        UpdateEnemySpeedCache();
        Debug.Log($"Player has ascended to Floor {currentFloor}.");
    }
}