using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// This singleton manager controls the opening and closing of major UI panels,
// ensuring that only one panel can be open at a time.
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panel References")]
    [Tooltip("Assign the InventoryUI script/panel here.")]
    [SerializeField] private InventoryUI inventoryUI;
    [Tooltip("Assign the CharacterPanelUI script/panel here.")]
    [SerializeField] private CharacterPanelUI characterPanelUI;
    [Tooltip("Assign the QuestJournalUI script/panel here.")]
    [SerializeField] private QuestJournalUI questJournalUI;
    [Tooltip("Assign the PauseMenuUI script/panel here.")] // NEW
    [SerializeField] private PauseMenuUI pauseMenuUI;

    private List<UIPanel> _allPanels;
    private UIPanel _currentlyOpenPanel;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // MODIFIED: Added the pauseMenuUI to the list of managed panels.
        _allPanels = new List<UIPanel> { inventoryUI, characterPanelUI, questJournalUI, pauseMenuUI };
        _allPanels = _allPanels.Where(p => p != null).ToList();
    }

    /// <summary>
    /// Toggles a specific UI panel. If another panel is open, it will be closed first.
    /// If the requested panel is already open, it will be closed.
    /// </summary>
    public void TogglePanel(UIPanel panelToToggle)
    {
        if (_currentlyOpenPanel != null && _currentlyOpenPanel == panelToToggle)
        {
            _currentlyOpenPanel.Close();
            _currentlyOpenPanel = null;
            return;
        }

        if (_currentlyOpenPanel != null)
        {
            _currentlyOpenPanel.Close();
        }

        panelToToggle.Open();
        _currentlyOpenPanel = panelToToggle;
    }
}