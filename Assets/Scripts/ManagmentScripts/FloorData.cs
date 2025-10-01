using UnityEngine;

// This ScriptableObject defines the essential properties for a single floor in the tower.
[CreateAssetMenu(fileName = "New Floor Data", menuName = "Game/Floor Data")]
public class FloorData : ScriptableObject
{
    [Header("Floor Information")]
    public int floorNumber;
    public string floorName;
    [TextArea(3, 5)]
    public string floorDescription;

    [Header("Scene Management")]
    [Tooltip("The name of the scene to load for this floor.")]
    public string sceneName;

    [Header("Trial Objective")]
    [Tooltip("The primary quest that must be completed to clear this floor.")]
    public Quest mainTrialQuest;
}