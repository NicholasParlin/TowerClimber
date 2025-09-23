using UnityEngine;
using System.Collections.Generic;

public enum QuestState { NotStarted, Active, ReadyForTurnIn, Completed }

[System.Serializable]
public class QuestStage
{
    public string stageName;
    [SerializeReference]
    public List<QuestObjective> objectives = new List<QuestObjective>();
}

// The Quest class is now a pure data container.
[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Information")]
    public string questTitle;
    public string questGiverName;

    [Header("Quest Descriptions")]
    [TextArea(3, 5)] public string briefDescription;
    [TextArea(5, 10)] public string detailedDescription;
    [TextArea(5, 10)] public string bonusInsight;

    [Header("Objectives")]
    public List<QuestStage> stages = new List<QuestStage>();

    [Header("Final Quest Rewards")]
    public int finalGoldReward;
    public int finalExperienceReward;
    public Skill finalSkillReward;

    [HideInInspector] public QuestState currentState = QuestState.NotStarted;
}

[System.Serializable]
public abstract class QuestObjective
{
    [Header("Objective Description")]
    public string description;
    [HideInInspector] public bool isComplete;
    [HideInInspector] public bool isUnlocked;

    [Header("Objective Rewards")]
    public int goldReward;
    public int experienceReward;

    public abstract void CheckProgress(object data);
    public abstract QuestObjective Clone();
}

[System.Serializable]
public class KillObjective : QuestObjective
{
    [Header("Kill Objective Settings")]
    public string targetEnemyID;
    public int requiredAmount;
    [HideInInspector] public int currentAmount;

    public override void CheckProgress(object data)
    {
        if (isComplete) return;
        if (data is string enemyID && enemyID == targetEnemyID)
        {
            currentAmount++;
            if (currentAmount >= requiredAmount)
            {
                isComplete = true;
            }
        }
    }

    public override QuestObjective Clone()
    {
        return new KillObjective { description = this.description, goldReward = this.goldReward, experienceReward = this.experienceReward, targetEnemyID = this.targetEnemyID, requiredAmount = this.requiredAmount };
    }
}

[System.Serializable]
public class CollectObjective : QuestObjective
{
    [Header("Collect Objective Settings")]
    public string targetItemID;
    public int requiredAmount;

    public override void CheckProgress(object data)
    {
        if (isComplete) return;

        if (data is string collectedItemID && collectedItemID == targetItemID)
        {
            if (InventoryManager.Instance != null)
            {
                int currentAmount = InventoryManager.Instance.GetItemQuantity(targetItemID);
                if (currentAmount >= requiredAmount)
                {
                    isComplete = true;
                }
            }
        }
    }

    public override QuestObjective Clone()
    {
        return new CollectObjective { description = this.description, goldReward = this.goldReward, experienceReward = this.experienceReward, targetItemID = this.targetItemID, requiredAmount = this.requiredAmount };
    }
}

[System.Serializable]
public class ExploreObjective : QuestObjective
{
    [Header("Explore Objective Settings")]
    public string targetLocationID;

    public override void CheckProgress(object data)
    {
        if (isComplete) return;
        if (data is string locationID && locationID == targetLocationID)
        {
            isComplete = true;
        }
    }

    public override QuestObjective Clone()
    {
        return new ExploreObjective { description = this.description, goldReward = this.goldReward, experienceReward = this.experienceReward, targetLocationID = this.targetLocationID };
    }
}

[System.Serializable]
public class TalkObjective : QuestObjective
{
    [Header("Talk Objective Settings")]
    public string targetNpcID;

    public override void CheckProgress(object data)
    {
        if (isComplete) return;
        if (data is string npcID && npcID == targetNpcID)
            isComplete = true;
    }

    public override QuestObjective Clone()
    {
        return new TalkObjective { description = this.description, goldReward = this.goldReward, experienceReward = this.experienceReward, targetNpcID = this.targetNpcID };
    }
}