using UnityEngine;

// A helper class for a single line of dialogue.
[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(3, 5)]
    public string lineText;
}

// This ScriptableObject is the template for a complete conversation.
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] lines;
}
