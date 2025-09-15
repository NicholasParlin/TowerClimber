using UnityEngine;

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

    private void Start()
    {
        // Initially, all indicators are off.
        SetIndicatorStates(false, false);
    }

    private void Update()
    {
        // This is a simple polling method to check quest states. In a larger game,
        // this logic would be driven by events from the QuestLog for better performance.
        if (_playerQuestLog != null && quest != null)
        {
            bool isAvailable = quest.currentState == QuestState.NotStarted;
            bool isReadyForTurnIn = quest.currentState == QuestState.ReadyForTurnIn;
            SetIndicatorStates(isAvailable, isReadyForTurnIn);
        }
    }

    // This method is called when the player enters the NPC's trigger collider.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerQuestLog = other.GetComponent<QuestLog>();
        }
    }

    // This method is called when the player leaves the NPC's trigger collider.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerQuestLog = null;
        }
    }

    /// <summary>
    /// This method would be called when the player interacts with the NPC.
    /// It handles the logic for offering, discussing, or completing the quest.
    /// </summary>
    public void Interact()
    {
        if (_playerQuestLog == null || quest == null) return;

        switch (quest.currentState)
        {
            case QuestState.NotStarted:
                // Offer the quest to the player.
                Debug.Log($"Offering Quest: {quest.questTitle}");
                _playerQuestLog.AddQuest(quest);
                break;

            case QuestState.Active:
                // Provide a reminder or hint about the quest.
                Debug.Log($"You are still working on: {quest.questTitle}.");
                break;

            case QuestState.ReadyForTurnIn:
                // Complete the quest.
                Debug.Log($"Turning in Quest: {quest.questTitle}");
                _playerQuestLog.CompleteQuest(quest);
                break;

            case QuestState.Completed:
                // Thank the player for their help.
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