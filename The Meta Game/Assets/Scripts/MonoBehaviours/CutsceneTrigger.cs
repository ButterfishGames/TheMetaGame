using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public CutsceneTrigger linked;
    public Cutscene scene;
    public bool oneTime;
    public bool triggerable;
    public bool walled = true;
    public bool modeReq = false;
    public GameController.GameMode reqMode;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerable && collision.CompareTag("Player"))
        {
            if (!modeReq || GameController.singleton.equipped == reqMode)
            {
                CutsceneManager.singleton.StartScene(scene, walled);
                if (oneTime)
                {
                    triggerable = false;
                    if (linked != null)
                    {
                        linked.triggerable = false;
                    }
                    StartCoroutine(UpdateSave());
                }
            }
            else if (oneTime)
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
