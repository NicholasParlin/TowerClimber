using UnityEngine;

// This adapter knows how to configure a QuestListingUI prefab with Quest data.
public class QuestDataAdapter : MonoBehaviour, IDataAdapter
{
    public void Setup(GameObject uiElement, object data)
    {
        QuestListingUI questUI = uiElement.GetComponent<QuestListingUI>();
        Quest questData = data as Quest;

        if (questUI != null && questData != null)
        {
            questUI.Setup(questData);
        }
    }
}