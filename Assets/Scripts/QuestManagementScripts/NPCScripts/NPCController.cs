using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// A helper class to link a specific Quest and its state to a Dialogue asset.
[System.Serializable]
public class QuestStateDialogue
{
    public Quest quest;
    public QuestState requiredState;
    public Dialogue dialogue;
}

// NEW: Enum to define what action to take when a prerequisite quest is completed.
public enum PostQuestAction
{
    DoNothing,
    Activate,
    Deactivate,
    Relocate,
    ActivateAndRelocate,
    DeactivateAndRelocate
}

// This is the base interaction controller for all non-hostile NPCs.
public class NPCController : MonoBehaviour, IInteractable
{
    [Header("NPC Configuration")]
    [SerializeField] private string npcID;

    [Header("Quest-Driven Dialogue")]
    [Tooltip("A prioritized list of dialogues to play based on quest states. The first condition met from top to bottom will be used.")]
    [SerializeField] private List<QuestStateDialogue> questDialogues;
    [Tooltip("The dialogue to play if no quest conditions are met.")]
    [SerializeField] private Dialogue defaultDialogue;

    [Header("Quest-Driven State Change")]
    [Tooltip("(Optional) The quest that must be completed to affect this NPC's state.")]
    [SerializeField] private Quest prerequisiteQuest;
    [Tooltip("The action to perform on this NPC once the prerequisite quest is completed.")]
    [SerializeField] private PostQuestAction postQuestAction = PostQuestAction.DoNothing;
    [Tooltip("(Optional) An empty GameObject marking the new position and rotation for this NPC.")]
    [SerializeField] private Transform postQuestTransform;


    public string InteractionPrompt { get; private set; }

    public void Interact()
    {
        if (!string.IsNullOrEmpty(npcID))
        {
            GameEvents.ReportNpcTalkedTo(npcID);
        }

        Dialogue dialogueToPlay = GetCurrentDialogue();
        if (dialogueToPlay != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueToPlay);
            return;
        }

        if (_shopkeeper != null) { _shopkeeper.OpenShop(); return; }
        if (_questGiver != null) { _questGiver.Interact(); return; }
        Debug.Log($"Hello, my name is {gameObject.name}.");
    }

    private Dialogue GetCurrentDialogue()
    {
        if (_playerQuestLog != null)
        {
            foreach (var stateDialogue in questDialogues)
            {
                if (_playerQuestLog.GetQuestState(stateDialogue.quest) == stateDialogue.requiredState)
                {
                    return stateDialogue.dialogue;
                }
            }
        }
        return defaultDialogue;
    }

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
            _playerQuestLog.OnQuestCompleted += HandleQuestCompleted;
            UpdateNpcState();
        }
        else
        {
            Debug.LogError("QuestLog not found via GameManager! NPC systems will not work correctly.", this);
        }
    }

    private void OnDestroy()
    {
        if (_playerQuestLog != null)
        {
            _playerQuestLog.OnQuestCompleted -= HandleQuestCompleted;
        }
    }

    private void HandleQuestCompleted(Quest completedQuest)
    {
        if (prerequisiteQuest != null && completedQuest == prerequisiteQuest)
        {
            UpdateNpcState();
        }
    }

    private void UpdateNpcState()
    {
        if (prerequisiteQuest == null || _playerQuestLog == null) return;

        bool isPrerequisiteMet = _playerQuestLog.IsQuestCompleted(prerequisiteQuest);

        if (isPrerequisiteMet)
        {
            // Prerequisite is met, so perform the chosen action.
            switch (postQuestAction)
            {
                case PostQuestAction.Activate:
                    gameObject.SetActive(true);
                    break;
                case PostQuestAction.Deactivate:
                    gameObject.SetActive(false);
                    break;
                case PostQuestAction.Relocate:
                    if (postQuestTransform != null)
                    {
                        transform.position = postQuestTransform.position;
                        transform.rotation = postQuestTransform.rotation;
                    }
                    break;
                case PostQuestAction.ActivateAndRelocate:
                    gameObject.SetActive(true);
                    if (postQuestTransform != null)
                    {
                        transform.position = postQuestTransform.position;
                        transform.rotation = postQuestTransform.rotation;
                    }
                    break;
                case PostQuestAction.DeactivateAndRelocate:
                    // This case is illogical but handled. The object will move then deactivate.
                    if (postQuestTransform != null)
                    {
                        transform.position = postQuestTransform.position;
                        transform.rotation = postQuestTransform.rotation;
                    }
                    gameObject.SetActive(false);
                    break;
            }
        }
        else
        {
            // Prerequisite is not met, so ensure the default state.
            // If the action is to activate on complete, it should start deactivated.
            if (postQuestAction == PostQuestAction.Activate || postQuestAction == PostQuestAction.ActivateAndRelocate)
            {
                gameObject.SetActive(false);
            }
            else // Otherwise, it should start activated.
            {
                gameObject.SetActive(true);
            }
        }
    }
}