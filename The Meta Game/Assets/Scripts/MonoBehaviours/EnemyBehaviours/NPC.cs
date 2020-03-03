using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : EnemyBehaviour
{
    [Tooltip("Array of dialogue lines to display when interacted with")]
    public Dialogue dialogue;

    public void Interact()
    {
        DialogueManager.singleton.StartDialogue(dialogue);
    }
}
