using System.Collections.Generic;
using UnityEngine;

// This ScriptableObject will act as a centralized database for all Item assets.
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> allItems = new List<Item>();
    private Dictionary<string, Item> _itemDictionary;

    private void OnEnable()
    {
        _itemDictionary = new Dictionary<string, Item>();
        foreach (var item in allItems)
        {
            if (item != null && !_itemDictionary.ContainsKey(item.name))
            {
                _itemDictionary.Add(item.name, item);
            }
        }
    }

    public Item GetItemByName(string itemName)
    {
        _itemDictionary.TryGetValue(itemName, out Item item);
        return item;
    }
}