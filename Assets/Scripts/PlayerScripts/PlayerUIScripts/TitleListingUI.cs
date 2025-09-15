using UnityEngine;
using UnityEngine.UI;
// using TMPro;

// This script goes on the prefab for a single title listing in the Character Panel.
public class TitleListingUI : MonoBehaviour
{
    [SerializeField] private Text titleNameText;
    [SerializeField] private Text titleDescriptionText;
    [SerializeField] private Button equipButton;

    private Title _title;
    private TitleManager _titleManager;

    public void Setup(Title title, TitleManager titleManager)
    {
        _title = title;
        _titleManager = titleManager;

        titleNameText.text = title.titleName;
        titleDescriptionText.text = title.description;

        equipButton.onClick.AddListener(OnEquipTitle);

        // Update the button's text to show if this title is currently equipped
        UpdateButtonVisual();
    }

    private void OnEquipTitle()
    {
        if (_titleManager != null && _title != null)
        {
            _titleManager.EquipTitle(_title);
            // We would need an event to refresh the entire panel after equipping
            // to update all button visuals.
            Debug.Log("Equipped title: " + _title.titleName);
        }
    }

    private void UpdateButtonVisual()
    {
        // The TitleManager would need a method to check the currently equipped title
        // bool isEquipped = _titleManager.IsTitleEquipped(_title);
        // equipButton.GetComponentInChildren<Text>().text = isEquipped ? "Equipped" : "Equip";
    }
}