using UnityEngine;
using UnityEngine.UI; // Use this if using standard UI
// using TMPro; // Uncomment if you use TextMeshPro

// This script goes on the prefab for a single inventory slot.
public class InventorySlotUI : MonoBehaviour
{
    [Header("UI Element References")]
    [SerializeField] private Image icon;
    [SerializeField] private Text quantityText; // Or TextMeshProUGUI

    /// <summary>
    /// Displays the data from an InventorySlot in this UI element.
    /// </summary>
    public void DisplayItem(InventorySlot slot)
    {
        if (slot.item == null)
        {
            ClearSlot();
            return;
        }

        // Update the UI elements with the item's data.
        icon.sprite = slot.item.icon;
        icon.enabled = true;

        // Show or hide the quantity text based on the item and its quantity.
        if (slot.item.isStackable && slot.quantity > 1)
        {
            quantityText.text = slot.quantity.ToString();
            quantityText.enabled = true;
        }
        else
        {
            quantityText.enabled = false;
        }
    }

    /// <summary>
    /// Clears the slot, making it appear empty.
    /// </summary>
    public void ClearSlot()
    {
        icon.sprite = null;
        icon.enabled = false;
        quantityText.enabled = false;
    }
}