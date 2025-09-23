using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent

// This is a generic component for any static object in the world the player can interact with.
public class WorldInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [Tooltip("The text that will appear in the UI when the player can interact with this object.")]
    [SerializeField] private string interactionPrompt = "Interact";

    [Header("Interaction Event")]
    [Tooltip("Assign the action(s) that should happen when the player interacts. " +
             "You can drag other GameObjects here and choose a public method to call.")]
    public UnityEvent OnInteract;


    // --- IInteractable Implementation ---

    public string InteractionPrompt
    {
        get { return interactionPrompt; }
    }

    public void Interact()
    {
        // When the player interacts, invoke all the events assigned in the Inspector.
        Debug.Log($"Interacting with {gameObject.name}.");
        OnInteract.Invoke();
    }
}