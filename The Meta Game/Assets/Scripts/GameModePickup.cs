using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModePickup : MonoBehaviour
{
    [Tooltip("Name of gamemode unlocked by pickup. Must match a mode in GameController.singleton.modes")]
    public string mode;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameController.singleton.Unlock(mode);
            Destroy(gameObject);
        }
    }
}
