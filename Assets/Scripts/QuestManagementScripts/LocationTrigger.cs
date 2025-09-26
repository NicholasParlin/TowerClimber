using UnityEngine;
using UnityEngine.Events;
using System.Collections;

// This script is placed on a trigger volume in the scene.
// It can fire various events based on its configuration when the player enters.
[RequireComponent(typeof(Collider))]
public class LocationTrigger : MonoBehaviour
{
    // Enum to define the different behaviors this trigger can have.
    public enum TriggerType
    {
        QuestObjective,
        UINotification,
        GenericEvent
    }

    [Header("Core Trigger Configuration")]
    [Tooltip("The behavior of this trigger.")]
    [SerializeField] private TriggerType triggerType = TriggerType.QuestObjective;

    [Header("Quest Objective Settings")]
    [Tooltip("A unique string ID for this location to complete Explore Objectives.")]
    [SerializeField] private string locationID;

    [Header("UI Notification Settings")]
    [Tooltip("The UI GameObject to activate. Should have a script to display the area name.")]
    [SerializeField] private GameObject notificationUI;
    [Tooltip("The cooldown in seconds before this notification can be shown again.")]
    [SerializeField] private float notificationCooldown = 60f;

    [Header("Generic Event Settings")]
    [Tooltip("The event(s) to fire when the player enters this trigger.")]
    public UnityEvent OnPlayerEnter;

    [Header("State Management (for One-Time Triggers)")]
    [Tooltip("If checked, this trigger will only fire once. Requires a 'Hidden Quest' to be assigned below to save its state.")]
    [SerializeField] private bool triggerOnce = false;
    [Tooltip("Assign a 'hidden' Quest ScriptableObject here to save the state of this one-time trigger.")]
    [SerializeField] private Quest stateTrackingQuest;


    private bool _isOnCooldown = false;
    private QuestLog _playerQuestLog;

    private void Start()
    {
        // MODIFIED: Get the reference to the QuestLog from the central GameManager.
        if (GameManager.Instance != null)
        {
            _playerQuestLog = GameManager.Instance.QuestLog;
        }
        else
        {
            Debug.LogError("GameManager not found in scene! LocationTrigger requires a GameManager to function.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _isOnCooldown) return;

        // If this is a one-time trigger, check the QuestLog first.
        if (triggerOnce)
        {
            if (stateTrackingQuest == null)
            {
                Debug.LogWarning($"LocationTrigger '{gameObject.name}' is set to trigger once but has no State Tracking Quest assigned. Its state will not be saved.");
            }
            else if (_playerQuestLog != null && _playerQuestLog.IsQuestCompleted(stateTrackingQuest))
            {
                // The quest is already complete, so this trigger has already fired. Do nothing.
                return;
            }
        }

        // --- Execute logic based on the selected TriggerType ---
        switch (triggerType)
        {
            case TriggerType.QuestObjective:
                Debug.Log($"Player entered quest location trigger: {locationID}");
                GameEvents.ReportLocationDiscovered(locationID);
                break;

            case TriggerType.UINotification:
                if (notificationUI != null)
                {
                    StartCoroutine(HandleNotification());
                }
                break;

            case TriggerType.GenericEvent:
                Debug.Log($"Player entered generic event trigger: {gameObject.name}");
                OnPlayerEnter.Invoke();
                break;
        }

        // If this was a one-time trigger with a valid quest, complete the quest now to save its state.
        if (triggerOnce && stateTrackingQuest != null && _playerQuestLog != null)
        {
            _playerQuestLog.AddQuest(stateTrackingQuest);
            _playerQuestLog.CompleteQuest(stateTrackingQuest);
            Debug.Log($"State tracking quest '{stateTrackingQuest.name}' completed for trigger '{gameObject.name}'.");
        }
    }

    private IEnumerator HandleNotification()
    {
        _isOnCooldown = true;
        notificationUI.SetActive(true);
        // A simple notification might just disable itself after a few seconds.
        // For now, we'll just rely on the cooldown.
        yield return new WaitForSeconds(notificationCooldown);
        _isOnCooldown = false;
    }

    private void OnValidate()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }
}