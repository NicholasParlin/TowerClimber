using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// MODIFIED: Inherits from our new UIPanel base class
public class InventoryUI : UIPanel
{
    [Header("UI References")]
    // MODIFIED: The main panel is now handled by the base class, so we can remove this line.
    // [SerializeField] private GameObject inventoryPanel; 
    [SerializeField] private VirtualizedScrollView virtualizedScrollView;
    [Tooltip("The adapter that knows how to display InventorySlot data.")]
    [SerializeField] private InventorySlotAdapter inventorySlotAdapter;

    private InventoryManager _inventoryManager;
    // MODIFIED: The _isOpen state is now handled by the base class.
    // private bool _isOpen = false; 

    // MODIFIED: The Start method is now in the base class. We use 'override' to add to it.
    protected override void Start()
    {
        base.Start(); // This calls the Start() method in UIPanel, which deactivates the panel.

        _inventoryManager = InventoryManager.Instance;
        if (_inventoryManager != null)
        {
            _inventoryManager.OnInventoryChanged += UpdateUI;
        }
        // The panel is already set to inactive by the base class's Start() method.
    }

    private void OnDestroy()
    {
        if (_inventoryManager != null)
        {
            _inventoryManager.OnInventoryChanged -= UpdateUI;
        }
    }

    // MODIFIED: The Toggle method is removed. The UIManager will call Open() and Close() directly.
    // public void Toggle() { ... }

    // MODIFIED: We override the Open method to add our refresh logic.
    public override void Open()
    {
        base.Open(); // Calls the base Open() which activates the panel GameObject.
        UpdateUI();
    }

    private void UpdateUI()
    {
        // We no longer need to check if it's open, because this is now only called when opening or when the inventory changes.
        if (_inventoryManager == null || virtualizedScrollView == null) return;

        List<object> inventoryData = _inventoryManager.inventory.Cast<object>().ToList();

        virtualizedScrollView.Initialize(inventoryData, inventorySlotAdapter);
    }
}