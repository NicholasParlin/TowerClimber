using UnityEngine;
using System.Collections.Generic;

// This ScriptableObject is the template for every Title in the game.
[CreateAssetMenu(fileName = "New Title", menuName = "Titles/Title")]
public class Title : ScriptableObject
{
    [Header("Title Information")]
    public string titleName;
    [TextArea(3, 5)]
    public string description;

    [Header("Stat Bonuses")]
    [Tooltip("The list of permanent stat bonuses this title provides when equipped.")]
    public List<StatBonus> statBonuses = new List<StatBonus>();
}