using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public Cutscene scene;
    public bool triggerable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerable && collision.CompareTag("Player"))
        {
            GameController.singleton.SwitchMode("platformer");
            scene.StartScene();
            triggerable = false;
        }
    }
}
