using System.Collections.Generic;
using UnityEngine;

// This script manages the main inventory UI panel.
public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The parent object where inventory slot prefabs will be instantiated.")]
    [SerializeField] private Transform itemsParent;
    [Tooltip("The main UI panel for the inventory.")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Prefabs")]
    [Tooltip("The UI prefab for a single inventory slot.")]
    [SerializeField] private GameObject inventorySlotPrefab;

    private InventoryManager _inventoryManager;

    void Start()
    {
        // Get the singleton instance of the InventoryManager.
        _inventoryManager = InventoryManager.Instance;

        // Subscribe to the inventory changed event. This is the core of the efficient UI.
        // The UpdateUI method will now only be called when the inventory actually changes.
        if (_inventoryManager != null)
        {
            _inventoryManager.OnInventoryChanged += UpdateUI;
        }

        // Start with the inventory panel closed.
        inventoryPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // Always unsubscribe from events when this object is destroyed to prevent errors.
        if (_inventoryManager != null)
        {
            _inventoryManager.OnInventoryChanged -= UpdateUI;
        }
    }

    /// <summary>
    /// Toggles the visibility of the inventory panel. This method will be called
    /// by the PlayerInputManager.
    /// </summary>
    public void Toggle()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        // When we open the panel, we should always refresh the UI to ensure it's up to date.
        if (inventoryPanel.activeSelf)
        {
            UpdateUI();
        }
    }

    /// <summary>
    /// Redraws the entire inventory UI based on the current state of the InventoryManager.
    /// </summary>
    private void UpdateUI()
    {
        if (_inventoryManager == null || !inventoryPanel.activeSelf) return;

        // Clear all the existing slot UIs to prevent duplicates.
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }

        // For each slot in the player's inventory, create a new UI element.
        foreach (InventorySlot slot in _inventoryManager.inventory)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, itemsParent);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                // Tell the individual slot UI to display the item's data.
                slotUI.DisplayItem(slot);
            }
        }
    }
}