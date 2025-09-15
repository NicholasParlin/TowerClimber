using UnityEngine;
using UnityEngine.UI; // Use this if using standard UI Text
// using TMPro; // Use this if using TextMeshPro

// This script goes on the prefab for a single quest listing in the Quest Board UI.
public class QuestListingUI : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private Text questTitleText; // Or TextMeshProUGUI
    [SerializeField] private Text questGiverText;
    [SerializeField] private Text goldRewardText;

    /// <summary>
    /// Populates the UI fields with data from a Quest ScriptableObject.
    /// </summary>
    public void Setup(Quest quest)
    {
        if (quest == null) return;

        questTitleText.text = quest.questTitle;
        questGiverText.text = $"From: {quest.questGiverName}";
        goldRewardText.text = $"Reward: {quest.finalGoldReward} Gold";
    }
}