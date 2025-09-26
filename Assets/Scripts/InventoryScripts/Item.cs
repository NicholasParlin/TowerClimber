using UnityEngine;
using System.Collections.Generic;

public enum ItemType
{
    Consumable,
    Equipment,
    QuestItem,
    Default
}

public abstract class Item : ScriptableObject
{
    [Header("Core Information")]
    public string itemName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    public ItemType itemType;
    public bool isStackable = true;
    public int maxStackSize = 99;
    [Tooltip("If checked, this item cannot be sold or dropped. It can only be removed by completing a quest.")]
    public bool isQuestItem = false; // NEW a

    [Header("Item Economy")]
    public int buyPrice = 10;
    public int sellPrice = 5;

    [Header("Gameplay Effects")]
    [Tooltip("The list of effects that are executed when this item is used.")]
    public List<GameplayEffect> effects;

    /// <summary>
    /// The Use method is now universal. It finds the user and executes all assigned effects on them.
    /// </summary>
    public virtual void Use()
    {
        if (GameManager.Instance == null || GameManager.Instance.PlayerStats == null)
        {
            Debug.LogError("Item.Use(): GameManager or Player not found in scene!");
            return;
        }

        GameObject player = GameManager.Instance.PlayerStats.gameObject;

        foreach (GameplayEffect effect in effects)
        {
            effect.Execute(null, player, player);
        }
    }
}