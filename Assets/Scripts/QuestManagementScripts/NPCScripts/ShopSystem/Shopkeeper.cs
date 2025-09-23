using System.Collections.Generic;
using UnityEngine;

// This component is attached to an NPC to make them a vendor.
public class Shopkeeper : MonoBehaviour
{
    [Header("Shop Inventory")]
    [Tooltip("The list of items that this shopkeeper has for sale.")]
    public List<Item> itemsForSale = new List<Item>();

    private InventoryManager _playerInventory;

    private void Start()
    {
        // Get a reference to the singleton InventoryManager.
        _playerInventory = InventoryManager.Instance;
    }

    /// <summary>
    /// Opens the main shop UI, passing this shopkeeper's data to it.
    /// This is called by the NPCController.
    /// </summary>
    public void OpenShop()
    {
        if (ShopUI.Instance != null)
        {
            ShopUI.Instance.OpenShopPanel(this);
        }
    }

    /// <summary>
    /// Handles the logic for the player buying an item from this shop.
    /// </summary>
    public void BuyItem(Item item)
    {
        if (_playerInventory.SpendGold(item.buyPrice))
        {
            _playerInventory.AddItem(item);
            Debug.Log($"Player bought {item.itemName} for {item.buyPrice} gold.");
        }
        else
        {
            Debug.Log("Not enough gold to buy " + item.itemName);
        }
    }

    /// <summary>
    /// Handles the logic for the player selling an item to this shop.
    /// </summary>
    public void SellItem(Item item)
    {
        // Grant the player gold for the item.
        _playerInventory.AddGold(item.sellPrice);

        // Remove the item from the player's inventory.
        _playerInventory.RemoveItem(item);

        Debug.Log($"Player sold {item.itemName} for {item.sellPrice} gold.");
    }
}