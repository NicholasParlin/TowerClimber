using UnityEngine;

/// <summary>
/// An interface for any data class that can be displayed by the VirtualizedScrollView.
/// </summary>
public interface IVirtualizationData
{
    /// <summary>
    /// This method contains the logic for how this data should be displayed on a UI element.
    /// </summary>
    /// <param name="displayObject">The UI GameObject (e.g., an InventorySlotUI prefab) that should display this data.</param>
    void SetupDisplay(GameObject displayObject);
}