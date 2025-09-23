using UnityEngine;

// This is the base interaction controller for all non-hostile NPCs.
// It now implements the IInteractable interface and can trigger dialogues or shops.
public class NPCController : MonoBehaviour, IInteractable
{
    [Header("NPC Configuration")]
    [Tooltip("Assign a Dialogue asset here for this NPC's main conversation.")]
    [SerializeField] private Dialogue dialogue;

    // --- IInteractable Implementation ---
    public string InteractionPrompt { get; private set; }

    /// <summary>
    /// This is the method that will be called when the player interacts with this NPC.
    /// It decides what to do based on the other components attached.
    /// </summary>
    public void Interact()
    {
        // Interaction priority: Shop > Dialogue > Quest > Default.

        // 1. If this NPC is a shopkeeper, open the shop.
        if (_shopkeeper != null)
        {
            _shopkeeper.OpenShop();
            return;
        }

        // 2. If they have a dialogue, start the conversation.
        if (dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
            return;
        }

        // 3. If they are a quest giver, interact with the quest system.
        if (_questGiver != null)
        {
            _questGiver.Interact();
            return;
        }

        // Default interaction if no other components are found.
        Debug.Log($"Hello, my name is {gameObject.name}.");
    }

    // --- Component References ---
    private QuestGiver _questGiver;
    private Shopkeeper _shopkeeper;

    private void Awake()
    {
        // Get references to any potential interaction components on this NPC.
        _questGiver = GetComponent<QuestGiver>();
        _shopkeeper = GetComponent<Shopkeeper>();

        // Update the interaction prompt based on the NPC's primary function.
        if (_shopkeeper != null)
        {
            InteractionPrompt = $"Trade with {gameObject.name}";
        }
        else
        {
            InteractionPrompt = $"Talk to {gameObject.name}";
        }
    }
}