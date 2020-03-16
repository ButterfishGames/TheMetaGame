using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTransitioner : MonoBehaviour
{
    [Tooltip("-1 is used to quit the game, 0 is used to go to credits, 1 is used to play")]
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
                // GameController.singleton.ignoreHints = false;
                GameController.singleton.onMenu = false;
                StartCoroutine(SongFade());
                GameController.singleton.StartCoroutine(GameController.singleton.FadeAndLoad(sceneInd));
            }
        }
    }

    private IEnumerator SongFade()
    {
        AudioSource song = GameObject.Find("Song").GetComponent<AudioSource>();
        while (song.volume > 0)
        {
            song.volume = Mathf.Clamp01(song.volume - (Time.deltaTime * GameController.singleton.levelFadeTime * 0.5f));
            yield return new WaitForEndOfFrame();
        }
    }
}
