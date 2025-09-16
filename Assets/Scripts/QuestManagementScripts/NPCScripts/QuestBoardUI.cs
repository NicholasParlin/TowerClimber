using UnityEngine;
using System.Collections.Generic;

// This script manages the UI for the Climber's Guild Quest Board and now uses an Object Pooler.
public class QuestBoardUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The parent object where quest listing prefabs will be instantiated.")]
    [SerializeField] private Transform questListContentArea;
    [Tooltip("The main UI panel for the quest board.")]
    [SerializeField] private GameObject questBoardPanel;

    private List<QuestGiver> _allQuestGivers = new List<QuestGiver>();
    // A list to keep track of the currently active UI objects from the pool.
    private List<GameObject> _activeListingObjects = new List<GameObject>();

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
        // --- REFACTORED: Return existing objects to the pool ---
        foreach (GameObject listingObject in _activeListingObjects)
        {
            // Use the tag you will set up in the ObjectPooler's Inspector
            ObjectPooler.Instance.ReturnToPool("QuestListing", listingObject);
        }
        _activeListingObjects.Clear();

        // Go through all known QuestGivers in the scene.
        foreach (QuestGiver giver in _allQuestGivers)
        {
            Quest questData = giver.QuestData;
            if (questData != null && questData.currentState == QuestState.NotStarted)
            {
                // --- REFACTORED: Get object from the pool instead of instantiating ---
                GameObject listingGO = ObjectPooler.Instance.GetFromPool("QuestListing", questListContentArea.position, Quaternion.identity);

                if (listingGO != null)
                {
                    listingGO.transform.SetParent(questListContentArea);
                    listingGO.transform.localScale = Vector3.one;

                    QuestListingUI listingUI = listingGO.GetComponent<QuestListingUI>();
                    if (listingUI != null)
                    {
                        listingUI.Setup(questData);
                        _activeListingObjects.Add(listingGO); // Track the active object
                    }
                }
            }
        }
    }
}