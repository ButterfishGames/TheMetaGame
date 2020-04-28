using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTransitioner : MonoBehaviour
{
    [Tooltip("-1 is used to quit the game, 0 is used to go to credits, 1 is used to play, 2 is used to delete save data")]
    public int function;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (function == -1)
            {
                Application.Quit();
            }
            else if (function == 1)
            {
                SaveManager.singleton.LoadGame();
                int sceneInd = SaveManager.singleton.GetCurrentScene();
                collision.enabled = false;
                GameController.singleton.onMenu = false;
                AkSoundEngine.PostEvent("sfx_start_game", gameObject);
                GameController.singleton.StartCoroutine(GameController.singleton.FadeAndLoad(sceneInd));
            }
            else if (function == 2)
            {
                SaveManager.singleton.DeleteSave();
            }
        }
    }
}
