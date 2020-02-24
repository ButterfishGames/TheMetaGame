using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTransitioner : MonoBehaviour
{
    [Tooltip("-1 is used to quit the game")]
    public int buildIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (buildIndex == -1)
            {
                Application.Quit();
            }
            else
            {
                collision.enabled = false;
                // GameController.singleton.ignoreHints = false;
                GameController.singleton.onMenu = false;
                StartCoroutine(SongFade());
                GameController.singleton.StartCoroutine(GameController.singleton.FadeAndLoad(buildIndex));
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
