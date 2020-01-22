using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController singleton;

    public enum GameMode
    {
        platformer,
        rpg
    };

    public GameMode equipped;

    // Start is called before the first frame update
    void Start()
    {
        if (singleton == null)
        {
            DontDestroyOnLoad(this);
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }

        equipped = GameMode.platformer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
