using UnityEngine;

// This is the base interaction controller for all non-hostile NPCs.
public class NPCController : MonoBehaviour
{
    // A reference to a QuestGiver component, if one exists on this NPC.
    private QuestGiver _questGiver;
    // You could add references to other components here, like a Shopkeeper.
    // private Shopkeeper _shopkeeper;

    private void Awake()
    {
        // Get references to any interaction components on this NPC.
        _questGiver = GetComponent<QuestGiver>();
        // _shopkeeper = GetComponent<Shopkeeper>();
    }

    /// <summary>
    /// This is the main interaction method. It will be called by a player's interaction script.
    /// It decides what to do based on the other components attached to this NPC.
    /// </summary>
    public void Interact()
    {
        // For now, our only interaction is with the quest system.
        if (_questGiver != null)
        {
            _questGiver.Interact();
            return; // We return so we don't try to open a shop and a quest dialogue at the same time.
        }

        // if (_shopkeeper != null)
        // {
        //     _shopkeeper.OpenShop();
        //     return;
        // }

        // If no other components are found, just have a default dialogue.
        Debug.Log($"Hello, my name is {gameObject.name}.");
    }
}