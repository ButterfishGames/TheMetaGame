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
                GameController.singleton.StartCoroutine(GameController.singleton.FadeAndLoad(buildIndex));
            }
        }
    }
}
