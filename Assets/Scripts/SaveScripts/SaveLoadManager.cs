using UnityEngine;

// This component acts as the central controller for the entire save and load process.
// You should have one instance of this in your initial scene.
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    [Header("Player Component References")]
    [SerializeField] private GameObject playerObject;

    // NEW: Reference to the SkillbarManager, which is often on a UI object.
    [Header("UI Manager References")]
    [SerializeField] private SkillbarManager skillbarManager;

    [Header("Database References")]
    [SerializeField] private SkillDatabase skillDatabase;
    [SerializeField] private QuestDatabase questDatabase;
    [SerializeField] private TitleDatabase titleDatabase;
    [SerializeField] private ItemDatabase itemDatabase;

    // Private references to all player components
    private PlayerStats _playerStats;
    private PlayerSkillManager _playerSkillManager;
    private QuestLog _questLog;
    private InventoryManager _inventoryManager;
    private TitleManager _titleManager;

    [Header("Debug Settings")]
    [SerializeField] private bool loadOnStart = true;

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
            DontDestroyOnLoad(gameObject);
        }

        // Get references to all the necessary components.
        if (playerObject != null)
        {
            _playerStats = playerObject.GetComponent<PlayerStats>();
            _playerSkillManager = playerObject.GetComponent<PlayerSkillManager>();
            _questLog = playerObject.GetComponent<QuestLog>();
            _inventoryManager = playerObject.GetComponent<InventoryManager>();
            _titleManager = playerObject.GetComponent<TitleManager>();
        }
        else
        {
            Debug.LogError("Player Object is not assigned in the SaveLoadManager!");
        }
    }

    private void Start()
    {
        if (loadOnStart)
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        if (!AreReferencesValid()) return;

        _playerStats.SaveState();
        _playerSkillManager.SaveState();
        _questLog.SaveState();
        _inventoryManager.SaveState();
        _titleManager.SaveState();
        skillbarManager.SaveState(); // NEW: Save the skillbar state.

        Debug.Log("--- GAME SAVED ---");
    }

    public void LoadGame()
    {
        if (!AreReferencesValid() || !AreDatabasesValid()) return;

        _playerStats.LoadState();
        _playerSkillManager.LoadState(skillDatabase);
        _questLog.LoadState(questDatabase);
        _titleManager.LoadState(titleDatabase);
        _inventoryManager.LoadState(itemDatabase);
        skillbarManager.LoadState(skillDatabase); // NEW: Load the skillbar state.

        Debug.Log("--- GAME LOADED ---");
    }

    private bool AreReferencesValid()
    {
        if (_playerStats == null || _playerSkillManager == null || _questLog == null || _inventoryManager == null || _titleManager == null || skillbarManager == null)
        {
            Debug.LogError("Cannot save/load game, one or more component references are missing in the SaveLoadManager!");
            return false;
        }
        return true;
    }

    private bool AreDatabasesValid()
    {
        if (skillDatabase == null || questDatabase == null || titleDatabase == null || itemDatabase == null)
        {
            Debug.LogError("Cannot load game, one or more databases are not assigned in the SaveLoadManager!");
            return false;
        }
        return true;
    }
}