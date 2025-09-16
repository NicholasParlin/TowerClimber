// This interface is a contract for any object that can be interacted with by the player.
public interface IInteractable
{
    // A property to get the text that should be displayed on the UI (e.g., "Talk to [NPC Name]").
    string InteractionPrompt { get; }

    // The method that will be called when the player interacts with the object.
    void Interact();
}