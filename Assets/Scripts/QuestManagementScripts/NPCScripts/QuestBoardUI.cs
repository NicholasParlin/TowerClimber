using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class QuestBoardUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The main UI panel for the quest board.")]
    [SerializeField] private GameObject questBoardPanel;
    [Tooltip("The Virtualized Scroll View component that will display the quests.")]
    [SerializeField] private VirtualizedScrollView virtualizedScrollView;

    private List<QuestGiver> _allQuestGivers = new List<QuestGiver>();

    private void Start()
    {
        _allQuestGivers.AddRange(FindObjectsByType<QuestGiver>(FindObjectsSortMode.None));
        questBoardPanel.SetActive(false);
    }

    public void OpenQuestBoard()
    {
        questBoardPanel.SetActive(true);
        PopulateQuestList();
    }



    public void CloseQuestBoard()
    {
        questBoardPanel.SetActive(false);
    }

    private void PopulateQuestList()
    {
        if (virtualizedScrollView == null) return;

        List<object> availableQuests = new List<object>();

        foreach (QuestGiver giver in _allQuestGivers)
        {
            Quest questData = giver.QuestData;
            if (questData != null && questData.currentState == QuestState.NotStarted)
            {
                availableQuests.Add(questData);
            }
        }

        // Define the setup function for the quest UI elements.
        System.Action<GameObject, object> setupQuestItem = (uiObject, data) =>
        {
            QuestListingUI questUI = uiObject.GetComponent<QuestListingUI>();
            Quest questData = data as Quest;
            if (questUI != null && questData != null)
            {
                questUI.Setup(questData);
            }
        };

        // Pass both the data and the setup function to the scroll view.
        virtualizedScrollView.Initialize(availableQuests, setupQuestItem);
    }
}