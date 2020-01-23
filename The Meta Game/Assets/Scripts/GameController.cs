using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController singleton;

    public enum GameMode
    {
        platformer,
        rpg
    };

    [Tooltip("Currently equipped gamemode. Should default to platformer.")]
    public GameMode equipped;

    [Tooltip("Resets equipped gamemode to platformer at start if enabled.")]
    public bool resetMode;

    [System.Serializable]
    public struct Mode
    {
        public string name;
        public bool unlocked;
    }

    [Tooltip("Modes Unlocked. Should default to only platformer.")]
    public Mode[] modes;

    [Tooltip("Resets unlocked gamemodes to platformer only at start if enabled")]
    public bool resetUnlocks;

    [Tooltip("Reference to prefab for game mode buttons in switch menu")]
    public GameObject modeButton;

    /// <summary>
    /// Object reference to the UI object which will hold game mode buttons
    /// </summary>
    private GameObject switchMenu;

    /// <summary>
    /// keeps track of the number of game modes unlocked
    /// </summary>
    private int numUnlocked;

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
            Destroy(gameObject);
        }

        equipped = GameMode.platformer;

        RectTransform[] rects = GetComponentsInChildren<RectTransform>(true);

        foreach(RectTransform rect in rects)
        {
            if (rect.name.Equals("SwitchMenu"))
            {
                switchMenu = rect.gameObject;
            }
        }

        if (resetMode)
        {
            equipped = GameMode.platformer;
        }

        if (resetUnlocks)
        {
            for (int i = 0; i < modes.Length; i++)
            {
                if (modes[i].name.Equals("Platformer"))
                {
                    modes[i].unlocked = true;
                }
                else
                {
                    modes[i].unlocked = false;
                }
            }
        }

        numUnlocked = 0;
        foreach(Mode mode in modes)
        {
            if (mode.unlocked)
            {
                numUnlocked++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            if (numUnlocked > 1)
            {
                ToggleSwitchMenu();
            }
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
                cameraWalls = Camera.main.GetComponentsInChildren<BoxCollider2D>(true);
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
                    if (mover.GetType().Equals(typeof(PFController)))
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
                cameraWalls = Camera.main.GetComponentsInChildren<BoxCollider2D>(true);
                foreach (BoxCollider2D col in cameraWalls)
                {
                    col.gameObject.SetActive(true);
                }

                player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<Rigidbody2D>().gravityScale = 0;
                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    if (mover.GetType().Equals(typeof(RPGController)))
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
            GameMode mode = (GameMode)System.Enum.Parse(typeof(GameMode), str.ToLower());
            return mode;
        }
        catch (System.Exception)
        {
            Debug.Log("ERROR: CANNOT CONVERT \"" + str.ToLower() + "\" TO ENUM");
            return null;
        }
    }

    public void ToggleSwitchMenu()
    {
        if (switchMenu.activeInHierarchy)
        {
            Button[] buttons = switchMenu.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                Destroy(button.gameObject);
            }

            switchMenu.SetActive(false);
        }
        else
        {
            switchMenu.SetActive(true);
            foreach(Mode mode in modes)
            {
                if (mode.unlocked)
                {
                    GameObject button = Instantiate(modeButton, switchMenu.transform);
                    button.GetComponent<Button>().onClick.AddListener(() => SwitchMode(mode.name));
                    button.GetComponentInChildren<Text>().text = mode.name;
                }
            }
        }
    }

    public void Unlock(string mode)
    {
        bool found = false;
        for (int i = 0; i < modes.Length && !found; i++)
        {
            if (modes[i].name.Equals(mode))
            {
                modes[i].unlocked = true;
                found = true;
            }
        }

        numUnlocked++;

        if (!found)
        {
            Debug.Log("ERROR: MODE STRING DID NOT MATCH ANY NAMES IN MODES ARRAY");
        }
    }
}
