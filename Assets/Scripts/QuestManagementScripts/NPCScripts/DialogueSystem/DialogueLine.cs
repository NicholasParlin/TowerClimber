using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent
using System;

// This is the simplest possible data structure for a line of dialogue.
[Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(3, 5)]
    public string lineText;

    // NEW: An event that will be fired when this line of dialogue is displayed.
    public UnityEvent onShowLine;
}

// This ScriptableObject is the template for a complete, linear conversation.
// It simply holds a list of the lines to be spoken in order.
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] lines;
}