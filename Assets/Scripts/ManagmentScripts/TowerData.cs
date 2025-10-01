using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// This ScriptableObject defines the entire structure of the tower, listing all floors in order.
[CreateAssetMenu(fileName = "Tower Data", menuName = "Game/Tower Data")]
public class TowerData : ScriptableObject
{
    [Tooltip("A list of all FloorData assets, in ascending order from floor 1.")]
    public List<FloorData> floors;

    /// <summary>
    /// A helper method to find the FloorData for a specific floor number.
    /// </summary>
    public FloorData GetFloorByNumber(int floorNumber)
    {
        // Use LINQ to find the first floor in the list that matches the given number.
        return floors.FirstOrDefault(f => f.floorNumber == floorNumber);
    }
}