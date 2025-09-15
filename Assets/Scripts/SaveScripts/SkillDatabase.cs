using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This ScriptableObject acts as a central database for all skills in the game.
// This is essential for the save/load system to be able to find a skill asset from a saved string ID.
[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Game/Skill Database")]
public class SkillDatabase : ScriptableObject
{
    public List<Skill> allSkills;

    // A dictionary for fast lookups.
    private Dictionary<string, Skill> _skillDictionary;

    public void Initialize()
    {
        _skillDictionary = allSkills.ToDictionary(skill => skill.skillName, skill => skill);
    }

    public Skill GetSkillByName(string skillName)
    {
        if (_skillDictionary == null)
        {
            Initialize();
        }

        _skillDictionary.TryGetValue(skillName, out Skill skill);
        return skill;
    }
}