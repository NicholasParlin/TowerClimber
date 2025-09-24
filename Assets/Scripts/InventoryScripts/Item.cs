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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Item.Use(): Player not found in scene!");
            return;
        }

        foreach (GameplayEffect effect in effects)
        {
            // By default, most item effects will target the user.
            effect.Execute(null, player, player); // Source skill is null for items.
        }
    }
}