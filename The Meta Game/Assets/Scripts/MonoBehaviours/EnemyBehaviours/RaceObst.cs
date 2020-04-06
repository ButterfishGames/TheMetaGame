using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceObst : EnemyBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enabled)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            RaceController raceCon = collision.gameObject.GetComponent<RaceController>();
            raceCon.StartCoroutine(raceCon.Crash());
        }
        else if (collision.gameObject.CompareTag("EnemyRacer"))
        {
            RaceEnemy raceEnemy = collision.gameObject.GetComponent<RaceEnemy>();
            if (raceEnemy != null)
            {
                raceEnemy.StartCoroutine(raceEnemy.Crash());
            }
        }
    }
}
