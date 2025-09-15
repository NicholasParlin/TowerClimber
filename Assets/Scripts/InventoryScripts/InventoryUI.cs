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

    // A list to keep track of the instantiated slot UI objects.
    private List<InventorySlotUI> _slotUIs = new List<InventorySlotUI>();
    private InventoryManager _inventoryManager;

    void Start()
    {
        // Get the singleton instance of the InventoryManager.
        _inventoryManager = InventoryManager.Instance;

        // Subscribe to an event that tells us when the inventory has changed.
        // We will need to add this event to the InventoryManager.
        // For now, we will just call UpdateUI() manually.

        inventoryPanel.SetActive(false); // Start with the inventory closed.
    }

    void Update()
    {
        // Simple toggle logic for opening/closing the inventory.
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (inventoryPanel.activeSelf)
            {
                UpdateUI();
            }
        }
    }

    /// <summary>
    /// Redraws the entire inventory UI based on the current state of the InventoryManager.
    /// </summary>
    public void UpdateUI()
    {
        // First, clear all the existing slot UIs to prevent duplicates.
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }
        _slotUIs.Clear();

        // For each slot in the player's inventory, create a new UI element.
        foreach (InventorySlot slot in _inventoryManager.inventory)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, itemsParent);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.DisplayItem(slot);
                _slotUIs.Add(slotUI);
            }
        }
    }
}