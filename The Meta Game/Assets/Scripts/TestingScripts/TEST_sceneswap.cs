using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_sceneswap : MonoBehaviour
{
    public static TEST_sceneswap singleton;

    // Start is called before the first frame update
    void Start()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GameController.singleton.StartCoroutine(GameController.singleton.FadeAndLoad(1));
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            GameController.singleton.StartCoroutine(GameController.singleton.FadeAndLoad(3));
        }
    }
}
