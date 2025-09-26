using UnityEngine;
using UnityEngine.UI;
using System.Text; // Required for StringBuilder

// This script goes on the UI prefab for a single quest entry in the Quest Journal.
// It's responsible for displaying all the details of one active quest.
public class QuestDetailViewUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text questTitleText;
    [SerializeField] private Text questDescriptionText;
    [SerializeField] private Text questObjectivesText;

    /// <summary>
    /// Populates all the UI fields with the data from a QuestStatus object.
    /// This will be called by our data adapter.
    /// </summary>
    // MODIFIED: Use the fully qualified name for the nested class.
    public void DisplayQuest(QuestLog.QuestStatus questStatus)
    {
        if (questStatus == null)
        {
            questTitleText.text = "No Quest Selected";
            questDescriptionText.text = "";
            questObjectivesText.text = "";
            return;
        }

        questTitleText.text = questStatus.quest.questTitle;
        questDescriptionText.text = questStatus.quest.detailedDescription;

        StringBuilder objectivesBuilder = new StringBuilder();
        QuestStage currentStage = questStatus.GetCurrentStage();

        if (currentStage != null)
        {
            foreach (var objective in currentStage.objectives)
            {
                if (objective.isComplete)
                {
                    objectivesBuilder.AppendLine($"<s>- {objective.description}</s>");
                }
                else
                {
                    if (objective is KillObjective killObjective)
                    {
                        objectivesBuilder.AppendLine($"- {killObjective.description} ({killObjective.currentAmount}/{killObjective.requiredAmount})");
                    }
                    else
                    {
                        objectivesBuilder.AppendLine($"- {objective.description}");
                    }
                }
            }
        }
        questObjectivesText.text = objectivesBuilder.ToString();
    }
}