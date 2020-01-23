using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController singleton;

    public GameObject switchMenu;

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
        if (Input.GetButtonDown("Menu"))
        {
            ToggleSwitchMenu();
        }
    }

        public void SwitchMode(string newMode)
    {
        GameMode? mode = Parse(newMode);
        if (mode != null)
        {
            SwitchMode((GameMode)mode);
        }
    }

    private void SwitchMode(GameMode newMode)
    {
        equipped = newMode;

        BoxCollider2D[] cameraWalls;
        GameObject player;
        Mover[] movers;

        switch (equipped)
        {
            case GameMode.platformer:
                cameraWalls = Camera.main.GetComponentsInChildren<BoxCollider2D>();
                foreach (BoxCollider2D col in cameraWalls)
                {
                    if (col.name.Equals("CameraWall_L"))
                    {
                        col.gameObject.SetActive(true);
                    }
                    else
                    {
                        col.gameObject.SetActive(false);
                    }
                }

                player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<Rigidbody2D>().gravityScale = 1;
                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    if (mover.GetType().Equals(typeof(PFMover)))
                    {
                        mover.enabled = true;
                    }
                    else
                    {
                        mover.enabled = false;
                    }
                }
                break;

            case GameMode.rpg:
                cameraWalls = Camera.main.GetComponentsInChildren<BoxCollider2D>();
                foreach (BoxCollider2D col in cameraWalls)
                {
                    col.gameObject.SetActive(true);
                }

                player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<Rigidbody2D>().gravityScale = 0;
                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    if (mover.GetType().Equals(typeof(GridMover)))
                    {
                        mover.enabled = true;
                    }
                    else
                    {
                        mover.enabled = false;
                    }
                }

                player.transform.position = new Vector3(GridLocker(player.transform.position.x), GridLocker(player.transform.position.y), 1);
                break;

            default:
                Debug.Log("ERROR: INVALID GAME MODE");
                break;
        }

        ToggleSwitchMenu();
    }

    private float GridLocker(float pos)
    {
        pos = Mathf.Floor(pos * 4) / 4;

        if (pos - Mathf.Floor(pos) == 0 || pos - Mathf.Floor(pos) == 0.5f)
        {
            pos += 0.25f;
        }

        return pos;
    }

    private GameMode? Parse(string str)
    {
        try
        {
            GameMode mode = (GameMode)System.Enum.Parse(typeof(GameMode), str);
            return mode;
        }
        catch (System.Exception)
        {
            Debug.Log("ERROR: CANNOT CONVERT " + str + " TO ENUM");
            return null;
        }
    }

    public void ToggleSwitchMenu()
    {
        if (switchMenu.activeInHierarchy)
        {
            switchMenu.SetActive(false);
        }
        else
        {
            switchMenu.SetActive(true);
        }
    }
}
