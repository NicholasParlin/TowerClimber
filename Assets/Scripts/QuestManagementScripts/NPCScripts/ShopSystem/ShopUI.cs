using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Text playerGoldText;

    [Header("Virtualized Lists")]
    [SerializeField] private VirtualizedScrollView shopItemsScrollView;
    [SerializeField] private ShopItemDataAdapter shopItemAdapter; // NEW: Adapter reference
    [SerializeField] private VirtualizedScrollView playerItemsScrollView;
    [SerializeField] private ShopItemDataAdapter playerItemAdapter; // NEW: Adapter reference

    private Shopkeeper _currentShopkeeper;
    private InventoryManager _playerInventory;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    private void Start()
    {
        _playerInventory = InventoryManager.Instance;
        if (_playerInventory != null)
        {
            _playerInventory.OnInventoryChanged += RefreshUI;
        }

        if (shopItemsScrollView != null) shopItemsScrollView.OnItemCreated += OnShopItemCreated;
        if (playerItemsScrollView != null) playerItemsScrollView.OnItemCreated += OnPlayerItemCreated;

        shopPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_playerInventory != null)
        {
            _playerInventory.OnInventoryChanged -= RefreshUI;
        }
    }

    public void OpenShopPanel(Shopkeeper shopkeeper)
    {
        _currentShopkeeper = shopkeeper;
        shopPanel.SetActive(true);
        RefreshUI();
    }

    public void CloseShopPanel()
    {
        shopPanel.SetActive(false);
    }

    private void RefreshUI()
    {
        if (_currentShopkeeper == null || !shopPanel.activeSelf) return;

        // Populate Shop's Inventory
        List<object> shopData = _currentShopkeeper.itemsForSale.Cast<object>().ToList();
        shopItemsScrollView.Initialize(shopData, shopItemAdapter);

        // Populate Player's Inventory for Selling
        List<object> playerData = _playerInventory.inventory.Cast<object>().ToList();
        playerItemsScrollView.Initialize(playerData, playerItemAdapter);

        playerGoldText.text = $"Gold: {_playerInventory.currentGold}";
    }

    // --- Event Handlers ---

    private void OnShopItemCreated(GameObject itemObject)
    {
        ShopItemUI itemUI = itemObject.GetComponent<ShopItemUI>();
        if (itemUI != null)
        {
            itemUI.OnBuyButtonClicked += OnBuyItem;
        }
    }

    private void OnPlayerItemCreated(GameObject itemObject)
    {
        ShopItemUI itemUI = itemObject.GetComponent<ShopItemUI>();
        if (itemUI != null)
        {
            itemUI.OnSellButtonClicked += OnSellItem;
        }
    }

    private void OnBuyItem(Item item)
    {
        if (_currentShopkeeper != null)
        {
            _currentShopkeeper.BuyItem(item);
        }
    }

    private void OnSellItem(Item item)
    {
        if (_currentShopkeeper != null)
        {
            _currentShopkeeper.SellItem(item);
        }
    }
}