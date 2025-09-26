using System;
using UnityEngine;

// A static class to manage game-wide events.
public static class GameEvents
{
    // --- Quest System Events ---

    // Fired whenever any enemy is killed.
    public static event Action<string> OnEnemyKilled;
    public static void ReportEnemyKilled(string enemyID) => OnEnemyKilled?.Invoke(enemyID);

    // Fired whenever an item is added to the player's inventory.
    public static event Action<string> OnItemCollected;
    public static void ReportItemCollected(string itemID) => OnItemCollected?.Invoke(itemID);

    // NEW: Fired whenever the player interacts with an NPC.
    public static event Action<string> OnNpcTalkedTo;
    public static void ReportNpcTalkedTo(string npcID) => OnNpcTalkedTo?.Invoke(npcID);

    // NEW: Fired whenever the player enters a specific location trigger.
    public static event Action<string> OnLocationDiscovered;
    public static void ReportLocationDiscovered(string locationID) => OnLocationDiscovered?.Invoke(locationID);
}