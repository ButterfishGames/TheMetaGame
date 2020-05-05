using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public Cutscene scene;
    public bool oneTime;
    public bool triggerable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerable && collision.CompareTag("Player"))
        {
            CutsceneManager.singleton.StartScene(scene);
            if (oneTime)
            {
                triggerable = false;
                StartCoroutine(UpdateSave());
            }
        }
    }

    private IEnumerator UpdateSave()
    {
        yield return new WaitUntil(() => NPC.shopkeeper != null);
        SaveManager.singleton.UpdateSceneData();
    }
}
