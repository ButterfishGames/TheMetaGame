using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "ScriptableObjects/Dialogue", order = 0)]
public class Dialogue : ScriptableObject
{
    [Tooltip("List of dialogue lines for the dialogue")]
    public DialogueLine[] lines;

    [Tooltip("Whether dialogue ends in a branch or the end of a conversation")]
    public DialogueType type;

    [Tooltip("Branch option text. Only used if Type is Branch. Should have 2-3 branches")]
    public string[] branches;

    [Tooltip("Dialogues that happen after branches. Only used if Type is Branch. Refers to indices in Character's dialogue array")]
    public Dialogue[] branchDialogues;

    [Tooltip("Relationship changes for different branch options. Only used if Type is Branch")]
    public int[] relationshipChanges;

    public Dialogue(string name, string line)
    {
        DialogueLine dialogueLine = new DialogueLine();
        dialogueLine.name = name;
        dialogueLine.line = line;

        lines = new DialogueLine[] { dialogueLine };
        type = DialogueType.end;
    }
}

[System.Serializable]
public struct DialogueLine
{
    [Tooltip("The name to display with this line")]
    public string name;

    [Tooltip("The text of the dialogue line")]
    [TextArea(3, 10)]
    public string line;

    [Tooltip("The character sprite associated with this line")]
    public Sprite sprite;
}

public enum DialogueType
{
    branch,
    end,
}