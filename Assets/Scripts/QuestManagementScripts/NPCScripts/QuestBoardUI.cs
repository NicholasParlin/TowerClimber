using UnityEngine;
using System.Collections.Generic;

// This script manages the UI for the Climber's Guild Quest Board.
public class QuestBoardUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The parent object where quest listing prefabs will be instantiated.")]
    [SerializeField] private Transform questListContentArea;
    [Tooltip("The UI prefab for a single quest listing.")]
    [SerializeField] private GameObject questListingPrefab;
    [Tooltip("The main UI panel for the quest board.")]
    [SerializeField] private GameObject questBoardPanel;

    private List<QuestGiver> _allQuestGivers = new List<QuestGiver>();

    private void Start()
    {
        // Find all QuestGivers using the modern, non-obsolete method.
        _allQuestGivers.AddRange(FindObjectsByType<QuestGiver>(FindObjectsSortMode.None));
        questBoardPanel.SetActive(false); // Start with the panel closed.
    }

    /// <summary>
    /// Opens the quest board UI and populates it with available quests.
    /// This would be called by a player interaction script (e.g., clicking on the board).
    /// </summary>
    public void OpenQuestBoard()
    {
        questBoardPanel.SetActive(true);
        PopulateQuestList();
    }

    /// <summary>
    /// Closes the quest board UI.
    /// </summary>
    public void CloseQuestBoard()
    {
        questBoardPanel.SetActive(false);
    }

    /// <summary>
    /// Clears and then rebuilds the list of quests on the board.
    /// </summary>
    private void PopulateQuestList()
    {
        // Clear any existing listings before repopulating.
        foreach (Transform child in questListContentArea)
        {
            Destroy(child.gameObject);
        }

        // Go through all known QuestGivers in the scene.
        foreach (QuestGiver giver in _allQuestGivers)
        {
            // Use the public property to access the quest data.
            Quest questData = giver.QuestData;
            if (questData != null && questData.currentState == QuestState.NotStarted)
            {
                // Create a new UI element from the prefab.
                GameObject listingGO = Instantiate(questListingPrefab, questListContentArea);

                // Get the component that holds the text fields on the prefab.
                QuestListingUI listingUI = listingGO.GetComponent<QuestListingUI>();
                if (listingUI != null)
                {
                    // Pass the quest data to the UI element to display it.
                    listingUI.Setup(questData);
                }
            }
        }
    }
}