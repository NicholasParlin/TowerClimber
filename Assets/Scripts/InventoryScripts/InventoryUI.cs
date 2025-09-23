using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private VirtualizedScrollView virtualizedScrollView;

    private InventoryManager _inventoryManager;
    private bool _isOpen = false;

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
        if (_inventoryManager != null)
        {
            _inventoryManager.OnInventoryChanged -= UpdateUI;
        }
    }

    public void Toggle()
    {
        _isOpen = !_isOpen;
        inventoryPanel.SetActive(_isOpen);

        if (_isOpen)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (!_isOpen || _inventoryManager == null || virtualizedScrollView == null) return;

        List<object> inventoryData = _inventoryManager.inventory.Cast<object>().ToList();

        // Define the setup function for the inventory UI elements.
        System.Action<GameObject, object> setupInventoryItem = (uiObject, data) =>
        {
            InventorySlotUI slotUI = uiObject.GetComponent<InventorySlotUI>();
            InventorySlot slotData = data as InventorySlot;
            if (slotUI != null && slotData != null)
            {
                slotUI.DisplayItem(slotData);
            }
        };

        // Pass both the data and the setup function to the scroll view.
        virtualizedScrollView.Initialize(inventoryData, setupInventoryItem);
    }
}