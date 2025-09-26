using UnityEngine;

// This is the base interaction controller for all non-hostile NPCs.
// It can be activated, deactivated, or moved based on quest completion.
public class NPCController : MonoBehaviour, IInteractable
{
    [Header("NPC Configuration")]
    [Tooltip("A unique string ID for this NPC (e.g., 'GuardWilliam', 'ShopkeeperMary'). This MUST match the ID used in Talk Objectives.")]
    [SerializeField] private string npcID;
    [Tooltip("Assign a Dialogue asset here for this NPC's main conversation.")]
    [SerializeField] private Dialogue dialogue;

    [Header("Quest-Driven Activation")]
    [Tooltip("(Optional) The quest that must be completed to affect this NPC's state.")]
    [SerializeField] private Quest prerequisiteQuest;
    [Tooltip("If checked, this NPC will be activated when the quest is complete. If unchecked, it will be deactivated.")]
    [SerializeField] private bool activateOnQuestComplete = true;
    [Tooltip("(Optional) An empty GameObject marking the position and rotation the NPC should move to after the quest is complete.")]
    [SerializeField] private Transform postQuestTransform;


    // --- IInteractable Implementation ---
    public string InteractionPrompt { get; private set; }

    public void Interact()
    {
        if (!string.IsNullOrEmpty(npcID))
        {
            GameEvents.ReportNpcTalkedTo(npcID);
        }

        if (_shopkeeper != null) { _shopkeeper.OpenShop(); return; }
        if (dialogue != null) { DialogueManager.Instance.StartDialogue(dialogue); return; }
        if (_questGiver != null) { _questGiver.Interact(); return; }

        Debug.Log($"Hello, my name is {gameObject.name}.");
    }

    // --- Component References ---
    private QuestGiver _questGiver;
    private Shopkeeper _shopkeeper;
    private QuestLog _playerQuestLog;

    private void Awake()
    {
        _questGiver = GetComponent<QuestGiver>();
        _shopkeeper = GetComponent<Shopkeeper>();

        if (_shopkeeper != null) { InteractionPrompt = $"Trade with {gameObject.name}"; }
        else { InteractionPrompt = $"Talk to {gameObject.name}"; }
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.QuestLog != null)
        {
            _playerQuestLog = GameManager.Instance.QuestLog;
            // Subscribe to the event for dynamic updates during gameplay
            _playerQuestLog.OnQuestCompleted += HandleQuestCompleted;
            // Check the initial state when the scene loads
            UpdateNpcState();
        }
        else
        {
            Debug.LogError("QuestLog not found via GameManager! NPC activation system will not work.", this);
        }
    }

    private void OnDestroy()
    {
        // Always unsubscribe from events
        if (_playerQuestLog != null)
        {
            _playerQuestLog.OnQuestCompleted -= HandleQuestCompleted;
        }
    }

    // This method is called by the OnQuestCompleted event from the QuestLog
    private void HandleQuestCompleted(Quest completedQuest)
    {
        // If the completed quest is the one we're waiting for, update our state.
        if (prerequisiteQuest != null && completedQuest == prerequisiteQuest)
        {
            UpdateNpcState();
        }
    }

    // Central logic to check the prerequisite and update the NPC's state
    private void UpdateNpcState()
    {
        if (prerequisiteQuest == null || _playerQuestLog == null)
        {
            // If no prerequisite is set, do nothing.
            return;
        }

        bool isPrerequisiteMet = _playerQuestLog.IsQuestCompleted(prerequisiteQuest);

        // Determine the desired active state based on the quest and the inspector setting.
        // If we want to ACTIVATE on complete, our desired state is TRUE if the prereq is met.
        // If we want to DEACTIVATE on complete, our desired state is FALSE if the prereq is met.
        bool desiredActiveState = isPrerequisiteMet ? activateOnQuestComplete : !activateOnQuestComplete;

        gameObject.SetActive(desiredActiveState);

        // If the prerequisite is met and a new location is assigned, move the NPC.
        if (isPrerequisiteMet && postQuestTransform != null)
        {
            transform.position = postQuestTransform.position;
            transform.rotation = postQuestTransform.rotation;
        }
    }
}