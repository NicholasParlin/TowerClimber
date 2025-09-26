using UnityEngine;
using System.Collections.Generic;

// A central singleton to manage the game's global state and hold references to essential systems.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private int currentFloor = 1;

    // --- System References ---
    [Header("System Component References")]
    [Tooltip("Assign the GameObject that has the PlayerStats, QuestLog, etc., components.")]
    [SerializeField] private GameObject playerObject;

    // Public properties for easy, fast access from any other script
    public PlayerStats PlayerStats { get; private set; }
    public QuestLog QuestLog { get; private set; }
    public InventoryManager InventoryManager { get; private set; }
    public PlayerSkillManager PlayerSkillManager { get; private set; }

    // NEW: List to hold all active QuestGivers in the scene.
    public List<QuestGiver> AllQuestGivers { get; private set; } = new List<QuestGiver>();


    [Header("Game Balance Settings")]
    [Tooltip("The multiplier for how effective Dexterity and Sense are at reducing enemy speed. Default: 1.5")]
    [SerializeField] private float defenseMultiplier = 1.5f;

    private float _baseEnemySpeedPerFloor = 0.10f;
    private float _cachedEnemyGameSpeed = 1.0f;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Find and assign all essential components from the player object.
        if (playerObject != null)
        {
            PlayerStats = playerObject.GetComponent<PlayerStats>();
            QuestLog = playerObject.GetComponent<QuestLog>();
            InventoryManager = playerObject.GetComponent<InventoryManager>();
            PlayerSkillManager = playerObject.GetComponent<PlayerSkillManager>();
        }
        else
        {
            Debug.LogError("Player Object is not assigned in the GameManager Inspector!", this);
        }
    }

    private void Start()
    {
        if (PlayerStats != null)
        {
            PlayerStats.OnStatsUpdated += UpdateEnemySpeedCache;
        }
        UpdateEnemySpeedCache();
    }

    private void OnDestroy()
    {
        if (PlayerStats != null)
        {
            PlayerStats.OnStatsUpdated -= UpdateEnemySpeedCache;
        }
    }

    // --- NEW: Quest Giver Registration ---
    public void RegisterQuestGiver(QuestGiver giver)
    {
        if (!AllQuestGivers.Contains(giver))
        {
            AllQuestGivers.Add(giver);
        }
    }

    public void UnregisterQuestGiver(QuestGiver giver)
    {
        if (AllQuestGivers.Contains(giver))
        {
            AllQuestGivers.Remove(giver);
        }
    }


    private void UpdateEnemySpeedCache()
    {
        if (PlayerStats == null)
        {
            Debug.LogError("PlayerStats reference is not available on the GameManager!");
            return;
        }

        float baseFloorSpeed = 1.0f + (currentFloor - 1) * _baseEnemySpeedPerFloor;
        float floorPower = 100f + (currentFloor - 1) * 10f;
        float playerDefense = (PlayerStats.Dexterity.Value + PlayerStats.Sense.Value) * defenseMultiplier;
        float speedMultiplier = floorPower / (floorPower + playerDefense);
        float finalSpeed = baseFloorSpeed * speedMultiplier;
        _cachedEnemyGameSpeed = Mathf.Max(finalSpeed, 0.25f);
    }

    public float GetCurrentEnemyGameSpeed()
    {
        return _cachedEnemyGameSpeed;
    }

    public void GoToNextFloor()
    {
        currentFloor++;
        UpdateEnemySpeedCache();
        Debug.Log($"Player has ascended to Floor {currentFloor}.");
    }
}