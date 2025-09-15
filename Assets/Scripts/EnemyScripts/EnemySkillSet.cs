using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScriptableObject that defines a set of skills for a specific type of enemy.
/// This allows for easy reuse and management of enemy abilities.
/// </summary>
[CreateAssetMenu(fileName = "New Enemy Skill Set", menuName = "Enemies/Skill Set")]
public class EnemySkillSet : ScriptableObject
{
    public List<Skill> skills = new List<Skill>();
}
