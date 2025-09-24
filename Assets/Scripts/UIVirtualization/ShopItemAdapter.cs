using UnityEngine;

// This adapter knows how to configure a ShopItemUI prefab with either Item or InventorySlot data.
public class ShopItemDataAdapter : MonoBehaviour, IDataAdapter
{
    // A reference to the shopkeeper is needed to handle buy/sell transactions.
    [SerializeField] private Shopkeeper shopkeeper;

    public void Setup(GameObject uiElement, object data)
    {
        ShopItemUI itemUI = uiElement.GetComponent<ShopItemUI>();
        if (itemUI == null) return;

        // The shop can display two types of data: raw Items (for buying)
        // or InventorySlots (for selling). We need to handle both cases.
        if (data is Item itemData)
        {
            // This is for the "Buy" panel.
            itemUI.DisplayItem(itemData, true); // true = isShopItem
        }
        else if (data is InventorySlot slotData)
        {
            // This is for the "Sell" panel.
            itemUI.DisplayItem(slotData.item, false); // false = isPlayerItem
        }
    }
}