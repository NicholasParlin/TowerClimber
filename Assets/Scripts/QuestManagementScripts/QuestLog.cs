using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System; // Required for Action

// This component is attached to the player and manages all their quests, including saving and loading progress.
[RequireComponent(typeof(PlayerStats), typeof(PlayerSkillManager), typeof(InventoryManager))]
public class QuestLog : MonoBehaviour
{
    public event Action<Quest> OnQuestCompleted;
    // CORRECTLY DEFINED: An event that fires whenever an objective's progress changes.
    public event Action<QuestObjective> OnObjectiveProgressUpdated;

    #region Save System Integration
    [System.Serializable]
    public class QuestObjectiveSaveData
    {
        public int currentAmount;
    }

    [System.Serializable]
    public class QuestStatusSaveData
    {
        public string questID;
        public int currentStageIndex;
        public List<QuestObjectiveSaveData> objectiveProgress;
    }

    [System.Serializable]
    public class SaveData
    {
        public List<QuestStatusSaveData> activeQuests;
        public List<string> completedQuestIDs;

        public SaveData(QuestLog questLog)
        {
            completedQuestIDs = questLog._completedQuests.Select(q => q.name).ToList();
            activeQuests = new List<QuestStatusSaveData>();

            foreach (var activeQuestStatus in questLog._activeQuests)
            {
                var questStatusData = new QuestStatusSaveData
                {
                    questID = activeQuestStatus.quest.name,
                    currentStageIndex = activeQuestStatus.currentStageIndex,
                    objectiveProgress = new List<QuestObjectiveSaveData>()
                };

                foreach (var stage in activeQuestStatus.stages)
                {
                    foreach (var objective in stage.objectives)
                    {
                        var objectiveData = new QuestObjectiveSaveData();
                        if (objective is KillObjective killObjective)
                        {
                            objectiveData.currentAmount = killObjective.currentAmount;
                        }
                        questStatusData.objectiveProgress.Add(objectiveData);
                    }
                }
                activeQuests.Add(questStatusData);
            }
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

        foreach (string questID in data.completedQuestIDs)
        {
            Quest quest = questDatabase.GetQuestByName(questID);
            if (quest != null)
            {
                quest.currentState = QuestState.Completed;
                _completedQuests.Add(quest);
            }
        }

        foreach (var savedStatus in data.activeQuests)
        {
            Quest quest = questDatabase.GetQuestByName(savedStatus.questID);
            if (quest != null)
            {
                QuestStatus runtimeStatus = new QuestStatus(quest);
                runtimeStatus.currentStageIndex = savedStatus.currentStageIndex;

                int progressIndex = 0;
                for (int i = 0; i < runtimeStatus.stages.Count; i++)
                {
                    for (int j = 0; j < runtimeStatus.stages[i].objectives.Count; j++)
                    {
                        if (progressIndex < savedStatus.objectiveProgress.Count)
                        {
                            var objective = runtimeStatus.stages[i].objectives[j];
                            if (objective is KillObjective killObjective)
                            {
                                killObjective.currentAmount = savedStatus.objectiveProgress[progressIndex].currentAmount;
                                if (killObjective.currentAmount >= killObjective.requiredAmount)
                                {
                                    killObjective.isComplete = true;
                                }
                            }
                        }
                        progressIndex++;
                    }
                }

                _activeQuests.Add(runtimeStatus);

                if (runtimeStatus.AreAllObjectivesInCurrentStageComplete() && runtimeStatus.currentStageIndex >= runtimeStatus.stages.Count - 1)
                {
                    quest.currentState = QuestState.ReadyForTurnIn;
                }
                else
                {
                    quest.currentState = QuestState.Active;
                }
            }
        }
        Debug.Log("Player quests and progress loaded.");
    }
    #endregion

    public class QuestStatus
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
        GameEvents.OnEnemyKilled += HandleEnemyKilled;
        GameEvents.OnItemCollected += HandleItemCollected;
        GameEvents.OnNpcTalkedTo += HandleNpcTalkedTo;
        GameEvents.OnLocationDiscovered += HandleLocationDiscovered;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        GameEvents.OnItemCollected -= HandleItemCollected;
        GameEvents.OnNpcTalkedTo -= HandleNpcTalkedTo;
        GameEvents.OnLocationDiscovered -= HandleLocationDiscovered;
    }

    public void AddQuest(Quest newQuest)
    {
        if (_activeQuests.Any(q => q.quest == newQuest) || _completedQuests.Contains(newQuest)) return;
        QuestStatus status = new QuestStatus(newQuest);
        _activeQuests.Add(status);
        status.quest.currentState = QuestState.Active;
        if (status.AreAllObjectivesInCurrentStageComplete())
        {
            status.quest.currentState = QuestState.ReadyForTurnIn;
        }
        else
        {
            UnlockObjectivesForStage(status, 0);
        }
    }

    private void HandleEnemyKilled(string enemyID) { CheckAllQuestProgress(enemyID); }
    private void HandleItemCollected(string itemID) { CheckAllQuestProgress(itemID); }
    private void HandleNpcTalkedTo(string npcID) { CheckAllQuestProgress(npcID); }
    private void HandleLocationDiscovered(string locationID) { CheckAllQuestProgress(locationID); }

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
                    int previousAmount = (objective is KillObjective ko) ? ko.currentAmount : 0;
                    objective.CheckProgress(progressData);

                    bool justCompleted = objective.isComplete && previousAmount == 0;
                    bool progressMade = (objective is KillObjective k) && k.currentAmount > previousAmount;

                    if (justCompleted || progressMade)
                    {
                        OnObjectiveProgressUpdated?.Invoke(objective);
                    }

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

        if (status != null && (status.quest.currentState == QuestState.ReadyForTurnIn || !status.stages.Any(s => s.objectives.Any())))
        {
            RemoveQuestItems(status);
            _activeQuests.Remove(status);
            _completedQuests.Add(questToComplete);
            questToComplete.currentState = QuestState.Completed;

            if (questToComplete.finalExperienceReward > 0) { _playerStats.AddExperience(questToComplete.finalExperienceReward); }
            if (questToComplete.finalGoldReward > 0) { _inventoryManager.AddGold(questToComplete.finalGoldReward); }
            if (questToComplete.finalSkillReward != null)
            {
                _playerSkillManager.TryLearnNewSkill(questToComplete.finalSkillReward, SkillAcquisitionCategory.QuestReward);
            }

            OnQuestCompleted?.Invoke(questToComplete);
        }
    }

    private void RemoveQuestItems(QuestStatus status)
    {
        foreach (var stage in status.stages)
        {
            foreach (var objective in stage.objectives)
            {
                if (objective is CollectObjective collectObjective && collectObjective.removeItemsOnCompletion)
                {
                    Item itemToRemove = _inventoryManager.inventory.FirstOrDefault(slot => slot.item.name == collectObjective.targetItemID)?.item;
                    if (itemToRemove != null)
                    {
                        _inventoryManager.RemoveItem(itemToRemove, collectObjective.requiredAmount);
                        Debug.Log($"Removed {collectObjective.requiredAmount} of {itemToRemove.name} from inventory for quest completion.");
                    }
                }
            }
        }
    }

    public bool IsQuestCompleted(Quest quest)
    {
        return _completedQuests.Contains(quest);
    }

    public QuestState GetQuestState(Quest quest)
    {
        if (_completedQuests.Contains(quest))
        {
            return QuestState.Completed;
        }
        if (_activeQuests.Any(q => q.quest == quest))
        {
            QuestStatus status = _activeQuests.FirstOrDefault(q => q.quest == quest);
            if (status != null)
            {
                return status.quest.currentState;
            }
        }
        return QuestState.NotStarted;
    }

    public List<QuestStatus> GetActiveQuestsForUI()
    {
        return _activeQuests;
    }
}