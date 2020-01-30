using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameController : MonoBehaviour
{
    /// <summary>
    /// A static variable storing a reference to the GameController singleton
    /// </summary>
    public static GameController singleton;

    public enum GameMode
    {
        platformer,
        rpg,
        fps,
        fighting
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

    [Tooltip("The maximum MP for the player (only used in certain gamemodes)")]
    public int maxMP;

    [Tooltip("The amount of damage the player takes each step on a damage floor while in RPG mode")]
    public int floorDamage;

    [Tooltip("The amount of time in seconds for which the error text will be displayed before it begins to fade")]
    public float errDispTime;

    [Tooltip("The amount of time in seconds over which the error text will fade")]
    public float errFadeTime;

    /// <summary>
    /// Array of object references to HintDisp objects on in-game hints
    /// </summary>
    private HintDisp[] hints;

    /// <summary>
    /// Used to store between scenes whether the hints should be displayed
    /// </summary>
    private bool[] shouldDisp = { true, true, true, false, false, false };

    /// <summary>
    /// Determines whether the game is paused or not
    /// </summary>
    private bool paused;

    /// <summary>
    /// Stores the player's current HP
    /// </summary>
    private int currHP;

    /// <summary>
    /// Stores the player's current MP
    /// </summary>
    private int currMP;

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
    /// Object reference to the TextMeshPro UGUI component that will hold joke error text
    /// </summary>
    private TextMeshProUGUI errText;

    /// <summary>
    /// keeps track of the number of game modes unlocked
    /// </summary>
    private int numUnlocked;

    // Start is called before the first frame update
    void Start()
    {
        if (singleton == null)
        {
            DontDestroyOnLoad(gameObject);
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }

        inverseDamageFadeTime = 1.0f / damageFadeTime;

        inverseLevelFadeTime = 1.0f / levelFadeTime;

        currHP = maxHP;

        currMP = maxMP;

        RectTransform[] rects = GetComponentsInChildren<RectTransform>(true);

        foreach (RectTransform rect in rects)
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

        errText = GetComponentInChildren<TextMeshProUGUI>();

        StartCoroutine(LevelFade(true));

        hints = new HintDisp[] { null };
        FindHints();

        if (resetMode)
        {
            equipped = GameMode.platformer;
        }
        SwitchMode(equipped);

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
        foreach (Mode mode in modes)
        {
            if (mode.unlocked)
            {
                numUnlocked++;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Menu") && numUnlocked > 1 && !paused)
        {
            ToggleSwitchMenu();
        }

        if (Input.GetButtonUp("Cancel"))
        {
            if (switchMenu.activeInHierarchy)
            {
                ToggleSwitchMenu();
            }
            else
            {
                ExitGame();
            }
        }

        if (Cursor.lockState != CursorLockMode.Locked && !Cursor.visible && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {
            Cursor.visible = true;
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

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float aspect = (float)Screen.width / (float)Screen.height;

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

                foreach (GameObject enemy in enemies)
                {
                    EnemyBehaviour[] behaviours = enemy.GetComponents<EnemyBehaviour>();

                    foreach (EnemyBehaviour behaviour in behaviours)
                    {
                        if (behaviour.GetType().Equals(typeof(PFEnemy)))
                        {
                            behaviour.enabled = true;
                        }
                        else
                        {
                            behaviour.enabled = false;
                        }
                    }
                }

                player = GameObject.Find("Player");

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

                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                Camera.main.projectionMatrix = Matrix4x4.Ortho(-5.3f * aspect, 5.3f * aspect, -5.3f, 5.3f, 0.3f, 1000.0f);
                Camera.main.GetComponent<FPSController>().enabled = false;
                Camera.main.GetComponent<CameraScroll>().enabled = true;

                FindHints();

                SetHintDisp(0, true);
                SetHintDisp(1, true);
                SetHintDisp(2, shouldDisp[2]);
                SetHintDisp(3, false);
                SetHintDisp(4, false);
                SetHintDisp(5, false);
                break;

            case GameMode.rpg:
                cameraWalls = Camera.main.GetComponentsInChildren<BoxCollider2D>(true);
                foreach (BoxCollider2D col in cameraWalls)
                {
                    col.gameObject.SetActive(true);
                }

                foreach (GameObject enemy in enemies)
                {
                    EnemyBehaviour[] behaviours = enemy.GetComponents<EnemyBehaviour>();

                    foreach (EnemyBehaviour behaviour in behaviours)
                    {
                        if (behaviour.GetType().Equals(typeof(NPC)))
                        {
                            behaviour.enabled = true;
                        }
                        else
                        {
                            behaviour.enabled = false;
                        }
                    }

                    enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    enemy.transform.position = new Vector3(GridLocker(enemy.transform.position.x), GridLocker(enemy.transform.position.y), 1);
                }

                player = GameObject.Find("Player");

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

                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                Camera.main.projectionMatrix = Matrix4x4.Ortho(-5.3f * aspect, 5.3f * aspect, -5.3f, 5.3f, 0.3f, 1000.0f);
                Camera.main.GetComponent<FPSController>().enabled = false;
                Camera.main.GetComponent<CameraScroll>().enabled = true;

                FindHints();

                SetHintDisp(0, false);
                SetHintDisp(1, false);
                SetHintDisp(2, shouldDisp[2]);
                SetHintDisp(3, true);
                SetHintDisp(4, true);
                SetHintDisp(5, false);
                break;

            case GameMode.fps:
                cameraWalls = Camera.main.GetComponentsInChildren<BoxCollider2D>(true);
                foreach (BoxCollider2D col in cameraWalls)
                {
                    col.gameObject.SetActive(false);
                }

                foreach (GameObject enemy in enemies)
                {
                    EnemyBehaviour[] behaviours = enemy.GetComponents<EnemyBehaviour>();

                    foreach (EnemyBehaviour behaviour in behaviours)
                    {
                        if (behaviour.GetType().Equals(typeof(PFEnemy)))
                        {
                            behaviour.enabled = true;
                        }
                        else
                        {
                            behaviour.enabled = false;
                        }
                    }
                }

                player = GameObject.Find("Player");

                player.GetComponent<Rigidbody2D>().gravityScale = 1;

                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    mover.enabled = false;
                }

                Camera.main.transform.position = new Vector3(
                    Mathf.Clamp(player.transform.position.x, Camera.main.GetComponent<CameraScroll>().min.x, Camera.main.GetComponent<CameraScroll>().max.x),
                    Mathf.Clamp(player.transform.position.y, Camera.main.GetComponent<CameraScroll>().min.y, Camera.main.GetComponent<CameraScroll>().max.y) + Camera.main.GetComponent<CameraScroll>().yOffset,
                    Camera.main.transform.position.z);
                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                Camera.main.projectionMatrix = Matrix4x4.Perspective(60, aspect, 0.3f, 1000.0f);
                Camera.main.GetComponent<CameraScroll>().enabled = false;
                Camera.main.GetComponent<FPSController>().enabled = true;

                FindHints();

                SetHintDisp(0, false);
                SetHintDisp(1, false);
                SetHintDisp(2, shouldDisp[2]);
                SetHintDisp(3, false);
                SetHintDisp(4, false);
                SetHintDisp(5, true);
                break;

            case GameMode.fighting:
                cameraWalls = Camera.main.GetComponentsInChildren<BoxCollider2D>(true);
                foreach (BoxCollider2D col in cameraWalls)
                {
                    if (col.name.Equals("CameraWall_L") || col.name.Equals("CameraWall_R"))
                    {
                        col.gameObject.SetActive(true);
                    }
                    else
                    {
                        col.gameObject.SetActive(false);
                    }
                }

                foreach (GameObject enemy in enemies)
                {
                    EnemyBehaviour[] behaviours = enemy.GetComponents<EnemyBehaviour>();

                    foreach (EnemyBehaviour behaviour in behaviours)
                    {
                        if (behaviour.GetType().Equals(typeof(FGEnemy)))
                        {
                            behaviour.enabled = true;
                        }
                        else
                        {
                            behaviour.enabled = false;
                        }
                    }
                }

                player = GameObject.Find("Player");

                player.GetComponent<Rigidbody2D>().gravityScale = 1;
                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    if (mover.GetType().Equals(typeof(FGController)))
                    {
                        mover.enabled = true;
                    }
                    else
                    {
                        mover.enabled = false;
                    }
                }

                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                Camera.main.projectionMatrix = Matrix4x4.Ortho(-5.3f * aspect, 5.3f * aspect, -5.3f, 5.3f, 0.3f, 1000.0f);
                Camera.main.GetComponent<FPSController>().enabled = false;
                Camera.main.GetComponent<CameraScroll>().enabled = true;

                FindHints();

                SetHintDisp(0, false);
                SetHintDisp(1, false);
                SetHintDisp(2, shouldDisp[2]);
                SetHintDisp(3, false);
                SetHintDisp(4, false);
                SetHintDisp(5, false);
                break;

            default:
                Debug.Log("ERROR: INVALID GAME MODE");
                break;
        }

        if (switchMenu.activeInHierarchy)
        {
            ToggleSwitchMenu();
        }
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
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            paused = true;
            Time.timeScale = 0;

            switchMenu.SetActive(true);
            foreach (Mode mode in modes)
            {
                if (mode.unlocked)
                {
                    GameObject button = Instantiate(modeButton, switchMenu.transform);
                    button.GetComponent<Button>().onClick.AddListener(() => SwitchMode(mode.name));
                    button.GetComponentInChildren<Text>().text = mode.name;
                    if (EventSystem.current.currentSelectedGameObject == null)
                    {
                        EventSystem.current.SetSelectedGameObject(button);
                    }
                }
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;

            if (shouldDisp[2])
            {
                SetHintDisp(2, false);
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
        yield return new WaitForEndOfFrame();
        SwitchMode(GameMode.platformer);
        StartCoroutine(LevelFade(true));
        paused = false;
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

    public void SetPaused(bool val)
    {
        paused = val;
    }

    public void Hit(int damage)
    {
        currHP -= damage;
        Debug.Log(currHP);
        StartCoroutine(SpriteDamageFlash());
        if (currHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator SpriteDamageFlash()
    {
        SpriteRenderer renderer = GameObject.Find("Player").GetComponentInChildren<SpriteRenderer>();
        renderer.color = Color.red;
        Color temp = renderer.color;
        while (renderer.color.g < 1 || renderer.color.b < 1)
        {
            temp.g += inverseDamageFadeTime * Time.deltaTime;
            temp.b = temp.g;
            renderer.color = temp;
            yield return new WaitForEndOfFrame();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public bool IsUnlocked(string modeName)
    {
        for(int i = 0; i < modes.Length; i++)
        {
            if (modes[i].name.Equals(modeName))
            {
                return modes[i].unlocked;
            }
        }

        Debug.Log("ERROR: MODE NAME PASSED DOES NOT MATCH ANY MODE IN MODES");
        return false;
    }

    public void SetHintDisp(int hint, bool val)
    {
        shouldDisp[hint] = val;
        hints[hint].SetDisplay(val);
    }

    private void FindHints()
    {
        while (hints[0] == null)
        {
            GameObject hintParent = GameObject.Find("Hints");
            hints = hintParent.GetComponentsInChildren<HintDisp>(true);

        }

        for (int i = 0; i < hints.Length - 1; i++)
        {
            for (int j = i + 1; j < hints.Length; j++)
            {
                if (int.Parse(hints[i].gameObject.name.Substring(5)) > int.Parse(hints[j].gameObject.name.Substring(5)))
                {
                    HintDisp temp = hints[i];
                    hints[i] = hints[j];
                    hints[j] = temp;
                }
            }
        }
    }

    public int GetHP()
    {
        return currHP;
    }

    public int GetMP()
    {
        return currMP;
    }

    public IEnumerator Battle()
    {
        StartCoroutine(LevelFade(false));
        yield return new WaitForSeconds(levelFadeTime);
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        yield return new WaitForEndOfFrame();
        GameObject attackBtn = null;
        while (attackBtn == null)
        {
            attackBtn = GameObject.Find("AttackButton");
            yield return new WaitForEndOfFrame();
        }
        EventSystem.current.SetSelectedGameObject(attackBtn);
        StartCoroutine(LevelFade(true));
    }

    public void ErrDisp(string err)
    {
        errText.alpha = 1;
        errText.text = err;
        StartCoroutine(ErrFade());
    }

    private IEnumerator ErrFade()
    {
        yield return new WaitForSeconds(errDispTime);
        errText.CrossFadeAlpha(0, errFadeTime, true);
    }
}