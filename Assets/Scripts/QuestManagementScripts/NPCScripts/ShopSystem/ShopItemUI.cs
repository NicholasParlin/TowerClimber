using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    // Events that other scripts (like the main ShopUI) can listen to.
    public event Action<Item> OnBuyButtonClicked;
    public event Action<Item> OnSellButtonClicked;

    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemPriceText;
    [SerializeField] private Button actionButton;

    private Item _item;
    private bool _isShopItem; // Is this an item for sale, or one the player is selling?

    /// <summary>
    /// A new, cleaner method for populating the UI element with data.
    /// </summary>
    public void DisplayItem(Item item, bool isShopItem)
    {
        _item = item;
        _isShopItem = isShopItem;

        itemIcon.sprite = item.icon;
        itemNameText.text = item.itemName;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnActionButtonClicked);

        if (_isShopItem)
        {
            // This is an item for sale in the shop.
            itemPriceText.text = $"{item.buyPrice} G";
        }
        else
        {
            // This is an item in the player's inventory for selling.
            itemPriceText.text = $"{item.sellPrice} G";
        }
    }

    /// <summary>
    /// When the button is clicked, fire the appropriate event.
    /// </summary>
    private void OnActionButtonClicked()
    {
        if (_isShopItem)
        {
            OnBuyButtonClicked?.Invoke(_item);
        }
        else
        {
            OnSellButtonClicked?.Invoke(_item);
        }
    }
}