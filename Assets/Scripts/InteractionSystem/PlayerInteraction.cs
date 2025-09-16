using UnityEngine;
using UnityEngine.UI; // This line is required to use the 'Text' class.
// using TMPro; // Uncomment if you use TextMeshPro for your UI text

// This script handles the player's ability to interact with objects in the world.
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer; // Set this in the Inspector to only detect interactable objects.

    [Header("UI References")]
    [Tooltip("Assign a UI Text element that will show the interaction prompt (e.g., 'E - Talk').")]
    [SerializeField] private GameObject interactionPromptUI;
    [SerializeField] private Text promptText; // Or TextMeshProUGUI

    // We will use a reference to the main camera to cast our ray from.
    private Camera _mainCamera;
    private IInteractable _currentInteractable;

    private void Start()
    {
        _mainCamera = Camera.main;
        interactionPromptUI.SetActive(false);
    }

    private void Update()
    {
        CheckForInteractables();
    }

    private void CheckForInteractables()
    {
        _currentInteractable = null;
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            // We hit something on the interactable layer.
            // Try to get the IInteractable component from it.
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                _currentInteractable = interactable;
                promptText.text = _currentInteractable.InteractionPrompt;
                interactionPromptUI.SetActive(true);
            }
            else
            {
                interactionPromptUI.SetActive(false);
            }
        }
        else
        {
            interactionPromptUI.SetActive(false);
        }
    }

    /// <summary>
    /// This is the public method that the PlayerInputManager will call when the interact key is pressed.
    /// </summary>
    public void TryInteract()
    {
        if (_currentInteractable != null)
        {
            _currentInteractable.Interact();
        }
    }
}