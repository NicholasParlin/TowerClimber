using UnityEngine;
using System.Linq; // Required for LINQ queries like .All()

// This component is attached to any NPC who can give a quest to the player.
public class QuestGiver : MonoBehaviour
{
    [Header("Quest Configuration")]
    [Tooltip("Assign the Quest ScriptableObject asset that this NPC offers.")]
    [SerializeField] private Quest quest;

    // Public property to allow other scripts (like the UI) to safely read the quest data.
    public Quest QuestData => quest;

    [Header("Quest State Visuals")]
    [Tooltip("An indicator to show the quest is available (e.g., a '!')")]
    [SerializeField] private GameObject questAvailableIndicator;
    [Tooltip("An indicator to show the quest is ready to be turned in (e.g., a '?')")]
    [SerializeField] private GameObject questReadyForTurnInIndicator;

    private QuestLog _playerQuestLog;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterQuestGiver(this);
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterQuestGiver(this);
        }
    }

    private void Start()
    {
        SetIndicatorStates(false, false);
    }

    private void Update()
    {
        if (_playerQuestLog != null && quest != null)
        {
            // MODIFIED: Check if all prerequisite quests have been completed.
            bool prerequisitesMet = quest.prerequisiteQuests == null || !quest.prerequisiteQuests.Any() || quest.prerequisiteQuests.All(prereq => _playerQuestLog.IsQuestCompleted(prereq));

            bool isAvailable = quest.currentState == QuestState.NotStarted && prerequisitesMet;
            bool isReadyForTurnIn = quest.currentState == QuestState.ReadyForTurnIn;
            SetIndicatorStates(isAvailable, isReadyForTurnIn);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // MODIFIED: Get the QuestLog from the GameManager for robustness.
            if (GameManager.Instance != null)
            {
                _playerQuestLog = GameManager.Instance.QuestLog;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerQuestLog = null;
        }
    }

    public void Interact()
    {
        if (_playerQuestLog == null || quest == null) return;

        switch (quest.currentState)
        {
            case QuestState.NotStarted:
                // MODIFIED: Check if all prerequisite quests have been completed.
                bool prerequisitesMet = quest.prerequisiteQuests == null || !quest.prerequisiteQuests.Any() || quest.prerequisiteQuests.All(prereq => _playerQuestLog.IsQuestCompleted(prereq));
                if (prerequisitesMet)
                {
                    Debug.Log($"Offering Quest: {quest.questTitle}");
                    _playerQuestLog.AddQuest(quest);
                }
                else
                {
                    Debug.Log($"Cannot offer quest '{quest.questTitle}' yet. Prerequisites not met.");
                }
                break;

            case QuestState.Active:
                Debug.Log($"You are still working on: {quest.questTitle}.");
                break;

            case QuestState.ReadyForTurnIn:
                Debug.Log($"Turning in Quest: {quest.questTitle}");
                _playerQuestLog.CompleteQuest(quest);
                break;

            case QuestState.Completed:
                Debug.Log($"Thank you again for completing {quest.questTitle}.");
                break;
        }
    }

    private void SetIndicatorStates(bool available, bool readyForTurnIn)
    {
        if (questAvailableIndicator != null)
        {
            questAvailableIndicator.SetActive(available);
        }
        if (questReadyForTurnInIndicator != null)
        {
            questReadyForTurnInIndicator.SetActive(readyForTurnIn);
        }
    }
}