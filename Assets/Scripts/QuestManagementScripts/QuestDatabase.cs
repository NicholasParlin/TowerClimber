using System.Collections.Generic;
using UnityEngine;

// This ScriptableObject will act as a centralized database for all Quest assets.
// This is essential for our save/load system.
[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quests/Quest Database")]
public class QuestDatabase : ScriptableObject
{
    public List<Quest> allQuests = new List<Quest>();

    // A dictionary for fast lookups.
    private Dictionary<string, Quest> _questDictionary;

    private void OnEnable()
    {
        _questDictionary = new Dictionary<string, Quest>();
        foreach (var quest in allQuests)
        {
            if (quest != null && !_questDictionary.ContainsKey(quest.name))
            {
                _questDictionary.Add(quest.name, quest);
            }
        }
    }

    public Quest GetQuestByName(string questName)
    {
        _questDictionary.TryGetValue(questName, out Quest quest);
        return quest;
    }
}
