using UnityEngine;
using System.Collections.Generic;

// This defines the different states a quest can be in.
public enum QuestState
{
    NotStarted,
    Active,
    ReadyForTurnIn,
    Completed
}

// NEW: A container class for a group of objectives that must be completed together.
[System.Serializable]
public class QuestStage
{
    [Tooltip("A name for this stage, for organizational purposes in the editor.")]
    public string stageName;

    [Tooltip("The list of objectives that must be completed to finish this stage.")]
    [SerializeReference]
    public List<QuestObjective> objectives = new List<QuestObjective>();
}


// This ScriptableObject is the template for every quest in the game.
[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Information")]
    public string questTitle;
    public string questGiverName; // The name of the NPC who gives the quest.

    [Header("Quest Descriptions")]
    [TextArea(3, 5)]
    public string briefDescription; // The short text shown on the main quest board.
    [TextArea(5, 10)]
    public string detailedDescription; // The full text shown in the player's quest log.
    [TextArea(5, 10)]
    public string bonusInsight; // The extra info the quest giver provides.

    [Header("Quest Structure")]
    [Tooltip("The list of stages for this quest. Stages will be completed in order.")]
    public List<QuestStage> stages = new List<QuestStage>();

    [Header("Final Quest Rewards")]
    [Tooltip("These rewards are given only when the entire quest is turned in.")]
    public int finalGoldReward;
    public int finalExperienceReward;
    public Skill finalSkillReward;

    // The current state of the quest. This will be managed by the player's QuestLog.
    [HideInInspector] public QuestState currentState = QuestState.NotStarted;
}

// This is the base class for all quest objectives.
[System.Serializable]
public abstract class QuestObjective
{
    [Header("Objective Description")]
    public string description;

    [HideInInspector] public bool isComplete;
    // NEW: A flag to track if this objective is currently active and can be progressed.
    [HideInInspector] public bool isUnlocked;

    [Header("Objective Rewards")]
    [Tooltip("Rewards given immediately upon completing this specific objective.")]
    public int goldReward;
    public int experienceReward;

    public abstract void CheckProgress(object data);
    public abstract QuestObjective Clone();
}

// --- Concrete Objective Types (Updated with new properties) ---

[System.Serializable]
public class KillObjective : QuestObjective
{
    [Header("Kill Objective Settings")]
    public string targetEnemyID;
    public int requiredAmount;
    [HideInInspector] public int currentAmount;

    public override void CheckProgress(object data)
    {
        // Only check progress if the objective is active and not already complete.
        if (!isUnlocked || isComplete) return;

        if (data is string enemyID && enemyID == targetEnemyID)
        {
            currentAmount++;
            if (currentAmount >= requiredAmount)
            {
                isComplete = true;
                Debug.Log($"Objective '{description}' complete!");
            }
        }
    }

    public override QuestObjective Clone()
    {
        return new KillObjective
        {
            description = this.description,
            goldReward = this.goldReward,
            experienceReward = this.experienceReward,
            targetEnemyID = this.targetEnemyID,
            requiredAmount = this.requiredAmount,
            isUnlocked = false, // Start locked
            isComplete = false
        };
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
        if (!isUnlocked || isComplete) return;

        if (data is string itemID && itemID == targetItemID)
        {
            // Logic to check inventory for requiredAmount.
        }
    }

    public override QuestObjective Clone()
    {
        return new CollectObjective
        {
            description = this.description,
            goldReward = this.goldReward,
            experienceReward = this.experienceReward,
            targetItemID = this.targetItemID,
            requiredAmount = this.requiredAmount,
            isUnlocked = false,
            isComplete = false
        };
    }
}

[System.Serializable]
public class ExploreObjective : QuestObjective
{
    [Header("Explore Objective Settings")]
    public string targetLocationID;

    public override void CheckProgress(object data)
    {
        if (!isUnlocked || isComplete) return;

        if (data is string locationID && locationID == targetLocationID)
        {
            isComplete = true;
            Debug.Log($"Objective '{description}' complete!");
        }
    }

    public override QuestObjective Clone()
    {
        return new ExploreObjective
        {
            description = this.description,
            goldReward = this.goldReward,
            experienceReward = this.experienceReward,
            targetLocationID = this.targetLocationID,
            isUnlocked = false,
            isComplete = false
        };
    }
}

[System.Serializable]
public class TalkObjective : QuestObjective
{
    [Header("Talk Objective Settings")]
    public string targetNpcID;

    public override void CheckProgress(object data)
    {
        if (!isUnlocked || isComplete) return;

        if (data is string npcID && npcID == targetNpcID)
        {
            isComplete = true;
            Debug.Log($"Objective '{description}' complete!");
        }
    }

    public override QuestObjective Clone()
    {
        return new TalkObjective
        {
            description = this.description,
            goldReward = this.goldReward,
            experienceReward = this.experienceReward,
            targetNpcID = this.targetNpcID,
            isUnlocked = false,
            isComplete = false
        };
    }
}