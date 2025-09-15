using System;
using UnityEngine;

// A static class to manage game-wide events.
// This allows different systems to communicate without direct references to each other.
public static class GameEvents
{
    // --- Quest System Events ---

    // This event is fired whenever any enemy is killed in the game.
    // It passes the string ID of the enemy that was defeated.
    public static event Action<string> OnEnemyKilled;

    // Call this method from your enemy's health script when it dies.
    public static void ReportEnemyKilled(string enemyID)
    {
        OnEnemyKilled?.Invoke(enemyID);
    }
}
