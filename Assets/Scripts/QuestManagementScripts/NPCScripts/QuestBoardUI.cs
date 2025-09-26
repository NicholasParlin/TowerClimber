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

    private QuestLog _playerQuestLog;

    private void Start()
    {
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
        if (GameManager.Instance == null || GameManager.Instance.QuestLog == null)
        {
            Debug.LogError("GameManager or QuestLog not found! Quest Board cannot be populated.");
            return;
        }

        _playerQuestLog = GameManager.Instance.QuestLog;
        List<object> availableQuests = new List<object>();

        foreach (QuestGiver giver in GameManager.Instance.AllQuestGivers)
        {
            Quest questData = giver.QuestData;

            if (questData != null && questData.currentState == QuestState.NotStarted)
            {
                // MODIFIED: Check if all quests in the prerequisite list are completed.
                bool prerequisitesMet = questData.prerequisiteQuests == null ||
                                        !questData.prerequisiteQuests.Any() ||
                                        questData.prerequisiteQuests.All(prereq => _playerQuestLog.IsQuestCompleted(prereq));

                if (prerequisitesMet)
                {
                    availableQuests.Add(questData);
                }
            }
        }

        virtualizedScrollView.Initialize(availableQuests, questAdapter);
    }
}