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
    [Tooltip("The adapter that knows how to display Quest data.")]
    [SerializeField] private QuestDataAdapter questAdapter;

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
        if (virtualizedScrollView == null || questAdapter == null) return;

        List<object> availableQuests = new List<object>();

        // Go through all known QuestGivers in the scene.
        foreach (QuestGiver giver in _allQuestGivers)
        {
            Quest questData = giver.QuestData;
            // Add quests that are available to be started.
            if (questData != null && questData.currentState == QuestState.NotStarted)
            {
                availableQuests.Add(questData);
            }
        }

        // Pass both the data and the adapter to the scroll view.
        virtualizedScrollView.Initialize(availableQuests, questAdapter);
    }
}