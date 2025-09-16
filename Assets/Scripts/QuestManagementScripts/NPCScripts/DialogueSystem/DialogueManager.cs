using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using TMPro; // Uncomment if you use TextMeshPro

// This singleton manager handles the display of all dialogue conversations.
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text speakerNameText;
    [SerializeField] private Text lineText;

    private Queue<DialogueLine> _lineQueue;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }

        _lineQueue = new Queue<DialogueLine>();
    }

    /// <summary>
    /// Starts a new dialogue conversation. Called by an NPCController.
    /// </summary>
    public void StartDialogue(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);
        _lineQueue.Clear();

        foreach (DialogueLine line in dialogue.lines)
        {
            _lineQueue.Enqueue(line);
        }

        DisplayNextLine();
    }

    /// <summary>
    /// Displays the next line in the queue. This would be called by a button in the UI.
    /// </summary>
    public void DisplayNextLine()
    {
        if (_lineQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = _lineQueue.Dequeue();
        speakerNameText.text = currentLine.speakerName;
        lineText.text = currentLine.lineText;
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}