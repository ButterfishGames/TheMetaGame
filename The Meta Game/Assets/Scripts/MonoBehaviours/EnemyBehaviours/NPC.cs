using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : EnemyBehaviour
{
    public Dialogue firstDialogue, otherDialogue;
    public bool interacted;

    public void Start()
    {
        interacted = false;
    }

    public void Interact()
    {
        if (interacted)
        {
            DialogueManager.singleton.StartDialogue(otherDialogue);
        }
        else
        {
            DialogueManager.singleton.StartDialogue(firstDialogue);
            interacted = true;
            SaveManager.singleton.UpdateSceneData();
        }
    }
}
