using UnityEngine;

// This adapter knows how to configure an InventorySlotUI prefab with InventorySlot data.
public class InventorySlotAdapter : MonoBehaviour, IDataAdapter
{
    public void Setup(GameObject uiElement, object data)
    {
        InventorySlotUI slotUI = uiElement.GetComponent<InventorySlotUI>();
        InventorySlot slotData = data as InventorySlot;

        if (slotUI != null && slotData != null)
        {
            slotUI.DisplayItem(slotData);
        }
    }
}