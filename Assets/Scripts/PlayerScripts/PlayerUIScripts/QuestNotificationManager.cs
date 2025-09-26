using UnityEngine;
using System.Collections.Generic;

// This manager listens for quest progress and displays UI notifications ("toasts").
// It should be placed on a persistent UI Canvas in your scene.
public class QuestNotificationManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The prefab for the quest progress toast UI element.")]
    [SerializeField] private GameObject toastUIPrefab;
    [Tooltip("The parent transform where toast notifications will be instantiated.")]
    [SerializeField] private Transform toastContainer;

    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 3;

    private QuestLog _playerQuestLog;
    private Queue<QuestProgressToastUI> _toastPool = new Queue<QuestProgressToastUI>();

    private void Start()
    {
        InitializePool();

        if (GameManager.Instance != null && GameManager.Instance.QuestLog != null)
        {
            _playerQuestLog = GameManager.Instance.QuestLog;
            _playerQuestLog.OnObjectiveProgressUpdated += HandleObjectiveProgressUpdated;
        }
        else
        {
            Debug.LogError("QuestLog not found! Quest Notification Manager will not function.", this);
        }
    }

    private void OnDestroy()
    {
        if (_playerQuestLog != null)
        {
            _playerQuestLog.OnObjectiveProgressUpdated -= HandleObjectiveProgressUpdated;
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject toastGO = Instantiate(toastUIPrefab, toastContainer);
            toastGO.SetActive(false);
            _toastPool.Enqueue(toastGO.GetComponent<QuestProgressToastUI>());
        }
    }

    private void HandleObjectiveProgressUpdated(QuestObjective objective)
    {
        string message = "";

        // Format the message based on the objective type
        if (objective is KillObjective killObjective)
        {
            message = $"{killObjective.description} ({killObjective.currentAmount}/{killObjective.requiredAmount})";
        }
        else
        {
            // For other types, just show the description and a checkmark if completed
            message = objective.isComplete ? $"{objective.description} (Complete)" : objective.description;
        }

        ShowNotification(message);
    }

    private void ShowNotification(string message)
    {
        if (_toastPool.Count > 0)
        {
            QuestProgressToastUI toast = _toastPool.Dequeue();
            toast.Show(message);
            // Re-queue the toast after its animation is done (handled by the toast script itself)
            // For a more robust pool, we'd have the toast call back to the manager to be re-queued.
            // For simplicity, we'll just put it back in the queue after a delay.
            StartCoroutine(ReturnToastToPool(toast, 5f)); // 5s is a safe delay
        }
    }

    private System.Collections.IEnumerator ReturnToastToPool(QuestProgressToastUI toast, float delay)
    {
        yield return new WaitForSeconds(delay);
        _toastPool.Enqueue(toast);
    }
}