using UnityEngine;
using System.Collections.Generic;

// A helper class to define a stat bonus within the Inspector.
[System.Serializable]
public class StatBonus
{
    public StatType statToBuff; // e.g., Strength, Dexterity
    public float value;         // e.g., 5, -10
}

// This ScriptableObject is the template for every Title in the game.
[CreateAssetMenu(fileName = "New Title", menuName = "Titles/Title")]
public class Title : ScriptableObject
{
    [Header("Title Information")]
    public string titleName;
    [TextArea(3, 5)]
    public string description;
    [Tooltip("The list of permanent stat bonuses this title provides when equipped.")]
    public List<StatBonus> statBonuses = new List<StatBonus>();
}
