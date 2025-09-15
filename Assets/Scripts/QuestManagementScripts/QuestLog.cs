using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This component is attached to the player and manages all their quests, including saving and loading progress.
[RequireComponent(typeof(PlayerStats), typeof(PlayerSkillManager), typeof(InventoryManager))]
public class QuestLog : MonoBehaviour
{
    #region Save System Integration
    [System.Serializable]
    public class SaveData
    {
        public List<string> activeQuestIDs;
        public List<string> completedQuestIDs;
        // NOTE: A more complex save system would also save objective progress (e.g., kill counts).
        // This version resets active quest progress on load for simplicity.

        public SaveData(QuestLog questLog)
        {
            activeQuestIDs = questLog._activeQuests.Select(q => q.quest.name).ToList();
            completedQuestIDs = questLog._completedQuests.Select(q => q.name).ToList();
        }
    }

    public void SaveState()
    {
        SaveSystem.SavePlayerQuests(this);
    }

    public void LoadState(QuestDatabase questDatabase)
    {
        SaveData data = SaveSystem.LoadPlayerQuests();
        if (data == null || questDatabase == null)
        {
            Debug.Log("No quest save data found, starting fresh.");
            return;
        }

        _activeQuests.Clear();
        _completedQuests.Clear();

        // Load completed quests from the save data.
        foreach (string questID in data.completedQuestIDs)
        {
            Quest quest = questDatabase.GetQuestByName(questID);
            if (quest != null)
            {
                quest.currentState = QuestState.Completed;
                _completedQuests.Add(quest);
            }
        }

        // Load active quests from the save data.
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
        public QuestStage GetCurrentStage() => currentStageIndex < stages.Count ? stages[currentStageIndex] : null;
        public bool AreAllObjectivesInCurrentStageComplete() => GetCurrentStage()?.objectives.All(obj => obj.isComplete) ?? false;
    }

    private List<QuestStatus> _activeQuests = new List<QuestStatus>();
    private List<Quest> _completedQuests = new List<Quest>();

    // References to other core player components for giving rewards.
    private PlayerStats _playerStats;
    private PlayerSkillManager _playerSkillManager;
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _playerSkillManager = GetComponent<PlayerSkillManager>();
        _inventoryManager = GetComponent<InventoryManager>();
    }

    private void OnEnable()
    {
        // Subscribe to the global game events.
        GameEvents.OnEnemyKilled += HandleEnemyKilled;
        GameEvents.OnItemCollected += HandleItemCollected;
    }

    private void OnDisable()
    {
        // Always unsubscribe to prevent errors.
        GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        GameEvents.OnItemCollected -= HandleItemCollected;
    }

    public void AddQuest(Quest newQuest)
    {
        if (_activeQuests.Any(q => q.quest == newQuest) || _completedQuests.Contains(newQuest)) return;
        QuestStatus status = new QuestStatus(newQuest);
        _activeQuests.Add(status);
        status.quest.currentState = QuestState.Active;
        UnlockObjectivesForStage(status, 0);
    }

    private void HandleEnemyKilled(string enemyID)
    {
        CheckAllQuestProgress(enemyID);
    }

    private void HandleItemCollected(string itemID)
    {
        CheckAllQuestProgress(itemID);
    }

    private void CheckAllQuestProgress(object progressData)
    {
        foreach (var questStatus in _activeQuests.ToList())
        {
            if (questStatus.quest.currentState != QuestState.Active) continue;
            QuestStage currentStage = questStatus.GetCurrentStage();
            if (currentStage == null) continue;

            foreach (var objective in currentStage.objectives)
            {
                if (objective.isUnlocked && !objective.isComplete)
                {
                    objective.CheckProgress(progressData);
                    if (objective.isComplete)
                    {
                        GrantObjectiveRewards(objective);
                    }
                }
            }

            if (questStatus.AreAllObjectivesInCurrentStageComplete())
            {
                questStatus.currentStageIndex++;
                if (questStatus.currentStageIndex < questStatus.stages.Count)
                {
                    UnlockObjectivesForStage(questStatus, questStatus.currentStageIndex);
                }
                else
                {
                    questStatus.quest.currentState = QuestState.ReadyForTurnIn;
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
        if (objective.experienceReward > 0) { _playerStats.AddExperience(objective.experienceReward); }
        if (objective.goldReward > 0) { _inventoryManager.AddGold(objective.goldReward); }
    }

    public void CompleteQuest(Quest questToComplete)
    {
        QuestStatus status = _activeQuests.FirstOrDefault(q => q.quest == questToComplete);
        if (status != null && status.quest.currentState == QuestState.ReadyForTurnIn)
        {
            _activeQuests.Remove(status);
            _completedQuests.Add(questToComplete);
            questToComplete.currentState = QuestState.Completed;

            if (questToComplete.finalExperienceReward > 0) { _playerStats.AddExperience(questToComplete.finalExperienceReward); }
            if (questToComplete.finalGoldReward > 0) { _inventoryManager.AddGold(questToComplete.finalGoldReward); }
            if (questToComplete.finalSkillReward != null)
            {
                _playerSkillManager.TryLearnNewSkill(questToComplete.finalSkillReward, SkillAcquisitionCategory.QuestReward);
            }
        }
    }
}