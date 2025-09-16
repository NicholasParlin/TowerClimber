using System.Collections.Generic;
using UnityEngine;

// This script manages the main inventory UI panel and now uses the Object Pooler for performance.
public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The parent object where inventory slot prefabs will be instantiated.")]
    [SerializeField] private Transform itemsParent;
    [Tooltip("The main UI panel for the inventory.")]
    [SerializeField] private GameObject inventoryPanel;

    // A list to keep track of the currently active slot GameObjects being used from the pool.
    private List<GameObject> _activeSlotObjects = new List<GameObject>();
    private InventoryManager _inventoryManager;

    void Start()
    {
        _inventoryManager = InventoryManager.Instance;
        if (_inventoryManager != null)
        {
            _inventoryManager.OnInventoryChanged += UpdateUI;
        }
        inventoryPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // Always unsubscribe from events to prevent errors.
        if (_inventoryManager != null)
        {
            _inventoryManager.OnInventoryChanged -= UpdateUI;
        }
    }

    /// <summary>
    /// Toggles the visibility of the inventory panel. This will be called by the PlayerInputManager.
    /// </summary>
    public void Toggle()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf)
        {
            UpdateUI();
        }
    }

    /// <summary>
    /// Redraws the entire inventory UI using the Object Pooler.
    /// </summary>
    private void UpdateUI()
    {
        if (!_inventoryManager || !inventoryPanel.activeSelf) return;

        // --- Step 1: Return all currently active UI slots to the pool ---
        foreach (GameObject slotObject in _activeSlotObjects)
        {
            // We use the tag "InventorySlot", which we will define in the ObjectPooler's Inspector.
            ObjectPooler.Instance.ReturnToPool("InventorySlot", slotObject);
        }
        // Clear the list of active objects.
        _activeSlotObjects.Clear();

        // --- Step 2: Get a new UI slot from the pool for each item in the inventory ---
        foreach (InventorySlot slot in _inventoryManager.inventory)
        {
            // Instead of Instantiate, we now call the pooler.
            GameObject slotGO = ObjectPooler.Instance.GetFromPool("InventorySlot", itemsParent.position, Quaternion.identity);

            if (slotGO != null)
            {
                // Set the parent to the content area and reset its scale.
                slotGO.transform.SetParent(itemsParent);
                slotGO.transform.localScale = Vector3.one;

                // Get the UI component and display the item's data.
                InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
                if (slotUI != null)
                {
                    slotUI.DisplayItem(slot);
                    // Add the object to our list of active slots for the next refresh.
                    _activeSlotObjects.Add(slotGO);
                }
            }
        }
    }
}