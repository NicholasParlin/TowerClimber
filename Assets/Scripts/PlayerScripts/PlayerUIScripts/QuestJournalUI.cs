using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// MODIFIED: Inherits from our new UIPanel base class
public class QuestJournalUI : UIPanel
{
    [Header("UI References")]
    // MODIFIED: The main panel is now handled by the base class.
    // [SerializeField] private GameObject journalPanel;
    [Tooltip("The Virtualized Scroll View that will list the active quests.")]
    [SerializeField] private VirtualizedScrollView activeQuestsScrollView;
    [Tooltip("The data adapter that knows how to display quest details.")]
    [SerializeField] private QuestDetailAdapter questDetailAdapter;

    private QuestLog _playerQuestLog;
    // MODIFIED: The _isOpen state is now handled by the base class.
    // private bool _isOpen = false; 

    // MODIFIED: The base class now handles deactivating the panel on Start.
    protected override void Start()
    {
        base.Start();

        if (GameManager.Instance != null && GameManager.Instance.QuestLog != null)
        {
            _playerQuestLog = GameManager.Instance.QuestLog;
            _playerQuestLog.OnQuestCompleted += (quest) => RefreshJournal();
        }
        else
        {
            Debug.LogError("QuestLog not found via GameManager! Quest Journal will not function.", this);
        }
    }

    private void OnDestroy()
    {
        if (_playerQuestLog != null)
        {
            _playerQuestLog.OnQuestCompleted -= (quest) => RefreshJournal();
        }
    }

    // MODIFIED: The Toggle method is removed. The UIManager will call Open() and Close() directly.
    // public void ToggleJournal() { ... }

    // MODIFIED: We override the Open method to refresh the journal when it's opened.
    public override void Open()
    {
        base.Open();
        RefreshJournal();
    }


    private void RefreshJournal()
    {
        // Don't need to check IsOpen here anymore, as this is called by Open() or by events.
        if (_playerQuestLog == null || activeQuestsScrollView == null) return;

        List<object> activeQuestData = _playerQuestLog.GetActiveQuestsForUI().Cast<object>().ToList();

        activeQuestsScrollView.Initialize(activeQuestData, questDetailAdapter);
    }
}