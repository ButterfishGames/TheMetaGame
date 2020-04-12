using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongPickup : MonoBehaviour
{
    public int songInd;

    private bool tested;

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.singleton != null)
        {
            if (GameController.singleton.songList[songInd].unlocked)
            {
                Destroy(gameObject);
            }
            tested = true;
        }
        else
        {
            tested = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!tested)
        {
            if (GameController.singleton != null)
            {
                if (GameController.singleton.songList[songInd].unlocked)
                {
                    Destroy(gameObject);
                }
                tested = true;
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameController.singleton.songList[songInd].unlocked = true;
            SaveManager.singleton.UpdatePlayerData();
            // TODO: Add Collectible SFX Event
            Destroy(gameObject);
        }
    }
}
