using UnityEngine;

// This is the base interaction controller for all non-hostile NPCs.
// It now implements the IInteractable interface and can trigger dialogues.
public class NPCController : MonoBehaviour, IInteractable
{
    [Header("NPC Configuration")]
    [Tooltip("Assign a Dialogue asset here to make this NPC start a conversation on interact.")]
    [SerializeField] private Dialogue dialogue;

    // --- IInteractable Implementation ---
    public string InteractionPrompt { get; private set; }

    /// <summary>
    /// This is the method that will be called when the player interacts with this NPC.
    /// It decides what to do based on the other components attached to this NPC.
    /// </summary>
    public void Interact()
    {
        // Prioritize starting a dialogue if one is assigned.
        if (dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
            return; // Return to prevent other interactions from happening at the same time.
        }

        // If no dialogue, fall back to quest interaction.
        if (_questGiver != null)
        {
            _questGiver.Interact();
            return;
        }

        // If no other components are found, just have a default debug message.
        Debug.Log($"Hello, my name is {gameObject.name}.");
    }

    // --- Original Logic ---
    private QuestGiver _questGiver;

    private void Awake()
    {
        // Get references to any interaction components on this NPC.
        _questGiver = GetComponent<QuestGiver>();

        // Set the initial interaction prompt.
        InteractionPrompt = $"Talk to {gameObject.name}";
    }
}