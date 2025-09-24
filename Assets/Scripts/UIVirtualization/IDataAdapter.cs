using UnityEngine;

/// <summary>
/// An interface for a dedicated adapter that knows how to set up a specific UI element
/// with a specific piece of data.
/// </summary>
public interface IDataAdapter
{
    /// <summary>
    /// Configures a UI element to display the given data.
    /// </summary>
    /// <param name="uiElement">The UI GameObject (e.g., a prefab instance) to be configured.</param>
    /// <param name="data">The data object (e.g., a Skill, an InventorySlot) to display.</param>
    void Setup(GameObject uiElement, object data);
}