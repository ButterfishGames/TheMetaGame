using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Tooltip("The time in seconds it takes for the flash from taking damage from damage floors to fade")]
    [Range(0.5f, 3.0f)]
    public float damageFadeTime;

    [Tooltip("The time in seconds it takes for the black screen to fade in and out when the level reloads")]
    [Range(0.5f, 3.0f)]
    public float levelFadeTime;

    [Tooltip("The maximum HP for the player (only used in certain gamemodes)")]
    public int maxHP;

    [Tooltip("The amount of damage the player takes each step on a damage floor while in RPG mode")]
    public int floorDamage;

    /// <summary>
    /// Determines whether the game is paused or not
    /// </summary>
    private bool paused;

    /// <summary>
    /// Stores the player's current HP
    /// </summary>
    private int currHP;

    /// <summary>
    /// Used to make flash fade time calculation more efficient
    /// </summary>
    private float inverseDamageFadeTime;

    /// <summary>
    /// Used to make level fade time calculations more efficient
    /// </summary>
    private float inverseLevelFadeTime;

    /// <summary>
    /// Object reference to the UI object which will hold game mode buttons
    /// </summary>
    private GameObject switchMenu;

    /// <summary>
    /// Object reference to the UI object for the red flash when damage is taken from damage floors
    /// </summary>
    private GameObject damageFlash;

    /// <summary>
    /// Object reference to the UI object for the black fade when the player dies/the level reloads
    /// </summary>
    private GameObject levelFade;

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

        inverseDamageFadeTime = 1.0f / damageFadeTime;

        inverseLevelFadeTime = 1.0f / levelFadeTime;

        currHP = maxHP;

        RectTransform[] rects = GetComponentsInChildren<RectTransform>(true);

        foreach(RectTransform rect in rects)
        {
            if (rect.name.Equals("SwitchMenu"))
            {
                switchMenu = rect.gameObject;
            }

            if (rect.name.Equals("DamageFlash"))
            {
                damageFlash = rect.gameObject;
            }

            if (rect.name.Equals("LevelFade"))
            {
                levelFade = rect.gameObject;
            }
        }

        StartCoroutine(LevelFade(true));

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

                RPGController rpgCon = player.GetComponent<RPGController>();
                if (rpgCon.mvmtCoroutine != null)
                {
                    rpgCon.StopCoroutine(rpgCon.mvmtCoroutine);
                    rpgCon.SetMoving(false);
                    rpgCon.mvmtCoroutine = null;
                }

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
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

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

            Time.timeScale = 1;
            paused = false;

            switchMenu.SetActive(false);
        }
        else
        {
            paused = true;
            Time.timeScale = 0;

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

    public void FloorDamage()
    {
        currHP = Mathf.Clamp(currHP - floorDamage, 0, maxHP);
        StartCoroutine(DamageFlash());
        if (currHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        Image img = damageFlash.GetComponent<Image>();
        Color temp = img.color;
        temp.a = 1;
        img.color = temp;
        while (img.color.a > 0)
        {
            temp.a -= inverseDamageFadeTime * Time.deltaTime;
            img.color = temp;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Die()
    {
        paused = true;
        StartCoroutine(ReloadLevel());
        currHP = maxHP;
    }

    private IEnumerator ReloadLevel()
    {
        StartCoroutine(LevelFade(false));
        yield return new WaitForSeconds(levelFadeTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SwitchMode(GameMode.platformer);
        ToggleSwitchMenu();
        StartCoroutine(LevelFade(true));
    }

    private IEnumerator LevelFade(bool fadeIn)
    {
        Image img = levelFade.GetComponent<Image>();
        Color temp = img.color;
        if (fadeIn)
        {
            temp.a = 1;
            img.color = temp;
            while (img.color.a > 0)
            {
                temp.a -= inverseLevelFadeTime * Time.deltaTime;
                img.color = temp;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            temp.a = 0;
            img.color = temp;
            while (img.color.a < 1)
            {
                temp.a += inverseLevelFadeTime * Time.deltaTime;
                img.color = temp;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public bool GetPaused()
    {
        return paused;
    }
}