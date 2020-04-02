using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateMapManager : MonoBehaviour
{
    public void ChooseLoc(int sceneInd)
    {
        GameController.singleton.StartCoroutine(GameController.singleton.FadeAndLoad(sceneInd));
    }
}
