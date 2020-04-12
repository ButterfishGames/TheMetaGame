using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConceptArtPickup : MonoBehaviour
{
    public int artInd;

    private bool tested;

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.singleton != null)
        {
            if (GameController.singleton.artList[artInd].unlocked)
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
                if (GameController.singleton.artList[artInd].unlocked)
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
            GameController.singleton.artList[artInd].unlocked = true;
            SaveManager.singleton.UpdatePlayerData();
            // TODO: Add Collectible SFX Event
            Destroy(gameObject);
        }
    }
}
