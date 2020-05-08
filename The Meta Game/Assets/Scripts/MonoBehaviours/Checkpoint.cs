using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Checkpoint active;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (active != this)
            {
                if (active != null)
                {
                    active.Deactivate();
                }
                Activate();
            }
        }
    }

    public void Activate()
    {
        StartCoroutine(ActivateRtn());
    }

    public IEnumerator ActivateRtn()
    {
        active = this;
        Vector3 loadPos = transform.position;
        loadPos.y += 0.5f;
        yield return new WaitUntil(() => NPC.shopkeeper != null);
        SaveManager.singleton.UpdateCheckpointPos(loadPos);
        SaveManager.singleton.SaveGame(true);
        animator.SetBool("active", true);
        AkSoundEngine.PostEvent("sfx_flagcheckpoint", gameObject);
    }

    public void Deactivate()
    {
        animator.SetBool("active", false);
    }
}
