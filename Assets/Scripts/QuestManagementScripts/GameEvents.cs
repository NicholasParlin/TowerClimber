using System;
using UnityEngine;

// A static class to manage game-wide events.
public static class GameEvents
{
    // --- Quest System Events ---

    // Fired whenever any enemy is killed.
    public static event Action<string> OnEnemyKilled;
    public static void ReportEnemyKilled(string enemyID) => OnEnemyKilled?.Invoke(enemyID);

    // --- NEW: Event for Inventory System ---
    // Fired whenever an item is added to the player's inventory.
    public static event Action<string> OnItemCollected;
    public static void ReportItemCollected(string itemID) => OnItemCollected?.Invoke(itemID);
}
