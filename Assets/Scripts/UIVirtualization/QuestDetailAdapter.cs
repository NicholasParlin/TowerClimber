using UnityEngine;

// This adapter knows how to configure a QuestDetailViewUI prefab with QuestStatus data.
// It should be attached to the same GameObject as the QuestJournalUI script.
public class QuestDetailAdapter : MonoBehaviour, IDataAdapter
{
    public void Setup(GameObject uiElement, object data)
    {
        QuestDetailViewUI detailView = uiElement.GetComponent<QuestDetailViewUI>();

        // MODIFIED: Use the fully qualified name for the nested class during the cast.
        if (detailView != null && data is QuestLog.QuestStatus questStatus)
        {
            detailView.DisplayQuest(questStatus);
        }
    }
}