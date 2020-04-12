using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateMapManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ChooseLoc(int sceneInd)
    {
        GameController.singleton.StartCoroutine(GameController.singleton.FadeAndLoad(sceneInd));
    }
}
