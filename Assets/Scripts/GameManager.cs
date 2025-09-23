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

    [Header("Game Balance Settings")]
    [Tooltip("The multiplier for how effective Dexterity and Sense are at reducing enemy speed. Default: 1.5")]
    [SerializeField] private float defenseMultiplier = 1.5f;

    // --- Enemy Game Speed Calculation ---
    private float _baseEnemySpeedPerFloor = 0.10f; // 10% speed increase per floor
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
            // We now subscribe to OnStatsUpdated to catch changes in Dex OR Sense.
            playerStats.OnStatsUpdated += UpdateEnemySpeedCache;
        }

        // Calculate the initial value when the game starts.
        UpdateEnemySpeedCache();
    }



    private void OnDestroy()
    {
        // Always unsubscribe from events when this object is destroyed to prevent errors.
        if (playerStats != null)
        {
            playerStats.OnStatsUpdated -= UpdateEnemySpeedCache;
        }
    }

    /// <summary>
    /// This method runs whenever the player's stats change or the floor changes.
    /// It recalculates the global enemy speed using the "Power vs. Defense" formula.
    /// </summary>
    private void UpdateEnemySpeedCache()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats reference is not set on the GameManager!");
            return;
        }

        // --- NEW "Power vs. Defense" FORMULA ---

        // 1. Calculate the base speed for the current floor.
        float baseFloorSpeed = 1.0f + (currentFloor - 1) * _baseEnemySpeedPerFloor;

        // 2. Calculate the floor's "Power".
        float floorPower = 100f + (currentFloor - 1) * 10f;

        // 3. Calculate the player's "Defense" from stats.
        float playerDefense = (playerStats.Dexterity.Value + playerStats.Sense.Value) * defenseMultiplier;

        // 4. Calculate the final speed multiplier using the diminishing returns formula.
        float speedMultiplier = floorPower / (floorPower + playerDefense);

        // 5. Calculate and cache the final game speed.
        float finalSpeed = baseFloorSpeed * speedMultiplier;

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
    /// This can be triggered by the WorldInteractable script on a portal or staircase.
    /// </summary>
    public void GoToNextFloor()
    {
        currentFloor++;
        // Recalculate the speed cache since the floor number has changed.
        UpdateEnemySpeedCache();
        Debug.Log($"Player has ascended to Floor {currentFloor}.");
    }
}