using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : EnemyBehaviour
{
    [Tooltip("Array of dialogue lines to display when interacted with")]
    [TextArea(3, 10)]
    public string[] dialogueLines;

    private void Start()
    {
        
    }

    public void Interact()
    {
        DialogueManager.singleton.StartDialogue(dialogueLines);
    }
}
