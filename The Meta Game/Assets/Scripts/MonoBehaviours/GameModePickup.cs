using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModePickup : MonoBehaviour
{
    [Tooltip("Name of gamemode unlocked by pickup. Must match a mode in GameController.singleton.modes")]
    public string mode;

    /// <summary>
    /// Used to check whether or not the script has been able to check if its associated mode has already been unlocked yet
    /// </summary>
    private bool tested;

    // Start is called before the first frame update
    private void Start()
    {
        if (GameController.singleton != null)
        {
            if (GameController.singleton.IsUnlocked(mode))
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
    private void Update()
    {
        if (!tested)
        {
            if (GameController.singleton != null)
            {
                if (GameController.singleton.IsUnlocked(mode))
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
            GameController.singleton.Unlock(mode);
            GameController.singleton.ToggleSwitchPanel(true);
            SaveManager.singleton.UpdatePlayerData();
            SaveManager.singleton.SaveGame(true); // REMOVE THIS WHEN CHECKPOINTS ARE IMPLEMENTED
            Destroy(gameObject);
        }
    }
}
