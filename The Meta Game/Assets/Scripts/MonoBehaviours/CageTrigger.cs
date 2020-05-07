using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageTrigger : MonoBehaviour
{
    public GameObject cage;

    private int achTime = 600; // in seconds
    private int achInd = 23;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LockCage();
            StartCoroutine(AchTimer());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
        }
    }

    private void LockCage()
    {
        cage.SetActive(true);
    }

    private IEnumerator AchTimer()
    {
        yield return new WaitForSeconds(achTime);
        GameController.singleton.achievements[achInd].Unlock();
    }
}
