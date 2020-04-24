using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobRoomController : MonoBehaviour
{
    public Spawner[] spawners;
    public GameObject blockers;
    public int numEnemies;
    private int spawnCounter;
    private int defeatCounter;

    private void Start()
    {
        foreach(Spawner spawner in spawners)
        {
            spawner.SetMRC(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartMobRoom();
        }
    }

    private void StartMobRoom()
    {
        blockers.SetActive(true);
        foreach (Spawner spawner in spawners)
        {
            spawner.enabled = true;
        }
        spawnCounter = 0;
        defeatCounter = 0;
    }

    private void StopMobRoom()
    {
        blockers.SetActive(false);
    }

    public void IncSpawnCounter()
    {
        spawnCounter++;
        if (spawnCounter >= numEnemies)
        {
            foreach(Spawner spawner in spawners)
            {
                spawner.enabled = false;
            }
        }
        Debug.Log(spawnCounter);
    }

    public void IncDefeatCounter()
    {
        defeatCounter++;
        if (defeatCounter >= numEnemies)
        {
            StopMobRoom();
        }
        Debug.Log(defeatCounter);
    }
}
