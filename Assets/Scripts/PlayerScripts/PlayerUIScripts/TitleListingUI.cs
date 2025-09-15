using UnityEngine;
using UnityEngine.UI;
// using TMPro; // Uncomment and change Text to TextMeshProUGUI if you use TextMeshPro

// This script goes on the prefab for a single title listing in the Character Panel.
public class TitleListingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text titleNameText;
    [SerializeField] private Text titleDescriptionText;
    [SerializeField] private Button equipButton;
    [SerializeField] private Text equipButtonText;

    private Title _title;
    private TitleManager _titleManager;

    /// <summary>
    /// Initializes the UI element with the data from a Title asset.
    /// </summary>
    public void Setup(Title title, TitleManager titleManager)
    {
        _title = title;
        _titleManager = titleManager;

        // Populate the text fields.
        titleNameText.text = title.titleName;
        titleDescriptionText.text = title.description;

        // Set up the button listener.
        equipButton.onClick.RemoveAllListeners(); // Clear previous listeners
        equipButton.onClick.AddListener(OnEquipTitle);

        // Subscribe to the event so this button updates when ANY title is equipped.
        // This ensures that when you equip one title, all others correctly show "Equip".
        _titleManager.OnEquippedTitleChanged += UpdateButtonVisual;

        // Set the initial visual state of the button.
        UpdateButtonVisual();
    }

    // Called when the GameObject is destroyed.
    private void OnDestroy()
    {
        // Always unsubscribe from events to prevent memory leaks and errors.
        if (_titleManager != null)
        {
            _titleManager.OnEquippedTitleChanged -= UpdateButtonVisual;
        }
    }

    /// <summary>
    /// Called when the "Equip" button is clicked.
    /// </summary>
    private void OnEquipTitle()
    {
        if (_titleManager != null && _title != null)
        {
            // Tell the TitleManager to equip this title.
            _titleManager.EquipTitle(_title);
            // The OnEquippedTitleChanged event will handle updating the button's visuals automatically.
        }
    }

    /// <summary>
    /// Updates the text and interactivity of the button based on whether this title is equipped.
    /// </summary>
    private void UpdateButtonVisual()
    {
        if (_titleManager == null || _title == null) return;

        // Ask the TitleManager if this specific title is the currently equipped one.
        bool isEquipped = _titleManager.IsTitleEquipped(_title);

        // Update the button text and make it non-interactable if it's already equipped.
        equipButtonText.text = isEquipped ? "EQUIPPED" : "EQUIP";
        equipButton.interactable = !isEquipped;
    }
}