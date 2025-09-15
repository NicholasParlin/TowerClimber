using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This component is attached to the player and manages all their quests, including saving and loading progress.
[RequireComponent(typeof(PlayerStats), typeof(PlayerSkillManager))]
public class QuestLog : MonoBehaviour
{
    #region Save System Integration
    // NOTE: A more complex save system would be needed to save the state of multi-stage quests
    // and the current kill counts of active objectives. This is a simplified version.
    [System.Serializable]
    public class SaveData
    {
        public List<string> activeQuestIDs;
        public List<string> completedQuestIDs;

        public SaveData(QuestLog questLog)
        {
            activeQuestIDs = questLog._activeQuests.Select(q => q.quest.name).ToList();
            completedQuestIDs = questLog._completedQuests.Select(q => q.name).ToList();
        }
    }

    /// <summary>
    /// Saves the current state of the quest log to a file.
    /// </summary>
    public void SaveState()
    {
        SaveSystem.SavePlayerQuests(this);
        Debug.Log("Player quests saved.");
    }

    /// <summary>
    /// Loads the state of the quest log from a file.
    /// </summary>
    public void LoadState(QuestDatabase questDatabase)
    {
        SaveData data = SaveSystem.LoadPlayerQuests();
        if (data == null || questDatabase == null)
        {
            Debug.Log("No quest save data found, starting fresh.");
            return;
        }

        // Clear existing quests before loading.
        _activeQuests.Clear();
        _completedQuests.Clear();

        // Load completed quests
        foreach (string questID in data.completedQuestIDs)
        {
            Quest quest = questDatabase.GetQuestByName(questID);
            if (quest != null)
            {
                quest.currentState = QuestState.Completed;
                _completedQuests.Add(quest);
            }
        }

        // Load active quests. Note: This simplified version resets their progress.
        foreach (string questID in data.activeQuestIDs)
        {
            Quest quest = questDatabase.GetQuestByName(questID);
            if (quest != null)
            {
                AddQuest(quest);
            }
        }
        Debug.Log("Player quests loaded.");
    }
    #endregion

    // A private class to track the runtime progress of a quest instance.
    private class QuestStatus
    {
        public Quest quest;
        public int currentStageIndex;
        public List<QuestStage> stages;

        public QuestStatus(Quest sourceQuest)
        {
            quest = sourceQuest;
            currentStageIndex = 0;
            stages = new List<QuestStage>();
            foreach (var stageTemplate in sourceQuest.stages)
            {
                QuestStage newStage = new QuestStage { stageName = stageTemplate.stageName };
                foreach (var objTemplate in stageTemplate.objectives)
                {
                    newStage.objectives.Add(objTemplate.Clone());
                }
                stages.Add(newStage);
            }
        }

        public QuestStage GetCurrentStage()
        {
            if (currentStageIndex < stages.Count)
            {
                return stages[currentStageIndex];
            }
            return null;
        }

        public bool AreAllObjectivesInCurrentStageComplete()
        {
            QuestStage currentStage = GetCurrentStage();
            if (currentStage == null) return false;
            return currentStage.objectives.All(obj => obj.isComplete);
        }
    }

    private List<QuestStatus> _activeQuests = new List<QuestStatus>();
    private List<Quest> _completedQuests = new List<Quest>();

    private PlayerStats _playerStats;
    private PlayerSkillManager _playerSkillManager;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _playerSkillManager = GetComponent<PlayerSkillManager>();
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyKilled += HandleEnemyKilled;
        // You would add other event subscriptions here, e.g.:
        // GameEvents.OnLocationReached += HandleLocationReached;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        // GameEvents.OnLocationReached -= HandleLocationReached;
    }

    public void AddQuest(Quest newQuest)
    {
        if (_activeQuests.Any(q => q.quest == newQuest) || _completedQuests.Contains(newQuest)) return;

        QuestStatus status = new QuestStatus(newQuest);
        _activeQuests.Add(status);
        status.quest.currentState = QuestState.Active;

        // Unlock the objectives for the very first stage.
        UnlockObjectivesForStage(status, 0);

        Debug.Log($"Quest Added: {newQuest.questTitle}");
    }

    private void HandleEnemyKilled(string enemyID)
    {
        // When an enemy dies, check all active quests.
        CheckAllQuestProgress(enemyID);
    }

    // Example for another event type
    // private void HandleLocationReached(string locationID)
    // {
    //     CheckAllQuestProgress(locationID);
    // }

    /// <summary>
    /// The central method for checking all quest progress based on game events.
    /// </summary>
    private void CheckAllQuestProgress(object progressData)
    {
        // Iterate through a copy of the list to allow for modification if quests are completed.
        foreach (var questStatus in _activeQuests.ToList())
        {
            if (questStatus.quest.currentState != QuestState.Active) continue;

            QuestStage currentStage = questStatus.GetCurrentStage();
            if (currentStage == null) continue;

            // Check progress on all unlocked objectives in the current stage.
            foreach (var objective in currentStage.objectives)
            {
                if (objective.isUnlocked && !objective.isComplete)
                {
                    objective.CheckProgress(progressData);
                    // If this objective was just completed, grant its immediate rewards.
                    if (objective.isComplete)
                    {
                        GrantObjectiveRewards(objective);
                    }
                }
            }

            // After checking all objectives, see if the entire stage is now complete.
            if (questStatus.AreAllObjectivesInCurrentStageComplete())
            {
                questStatus.currentStageIndex++; // Advance to the next stage index.

                // Check if there is a next stage to unlock.
                if (questStatus.currentStageIndex < questStatus.stages.Count)
                {
                    UnlockObjectivesForStage(questStatus, questStatus.currentStageIndex);
                    Debug.Log($"Stage '{currentStage.stageName}' complete for quest '{questStatus.quest.questTitle}'. New stage unlocked.");
                }
                else // This was the final stage.
                {
                    questStatus.quest.currentState = QuestState.ReadyForTurnIn;
                    Debug.Log($"All stages complete for '{questStatus.quest.questTitle}'. Quest is ready for turn-in.");
                }
            }
        }
    }

    private void UnlockObjectivesForStage(QuestStatus status, int stageIndex)
    {
        if (stageIndex < status.stages.Count)
        {
            foreach (var objective in status.stages[stageIndex].objectives)
            {
                objective.isUnlocked = true;
            }
        }
    }

    private void GrantObjectiveRewards(QuestObjective objective)
    {
        if (objective.experienceReward > 0)
        {
            _playerStats.AddExperience(objective.experienceReward);
        }
        if (objective.goldReward > 0)
        {
            // InventoryManager.Instance.AddGold(objective.goldReward);
            Debug.Log($"Player received {objective.goldReward} gold for completing an objective.");
        }
    }

    public void CompleteQuest(Quest questToComplete)
    {
        QuestStatus status = _activeQuests.FirstOrDefault(q => q.quest == questToComplete);
        if (status != null && status.quest.currentState == QuestState.ReadyForTurnIn)
        {
            _activeQuests.Remove(status);
            _completedQuests.Add(questToComplete);
            questToComplete.currentState = QuestState.Completed;

            // --- FINAL REWARD PAYOUT LOGIC ---
            if (questToComplete.finalExperienceReward > 0)
            {
                _playerStats.AddExperience(questToComplete.finalExperienceReward);
            }
            if (questToComplete.finalGoldReward > 0)
            {
                // InventoryManager.Instance.AddGold(questToComplete.finalGoldReward);
                Debug.Log($"Player received {questToComplete.finalGoldReward} gold as final quest reward.");
            }
            if (questToComplete.finalSkillReward != null)
            {
                _playerSkillManager.TryLearnNewSkill(questToComplete.finalSkillReward, SkillAcquisitionCategory.QuestReward);
            }

            Debug.Log($"Quest Completed: {questToComplete.questTitle}");
        }
    }
}