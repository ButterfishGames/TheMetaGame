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

    [System.Serializable]
    public struct SpellStr
    {
        public Spell spell;
        public bool unlocked;
    }

    [Tooltip("Spells unlocked. DO NOT DEFAULT TO ANY HEALING SPELLS")]
    public SpellStr[] spellList;

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

    public bool ignoreHints;

    public bool onMenu;

    public TextMeshProUGUI codeText;

    [Header("Code Highlight Colors")]
    public Color keyword, type, comment, literal, stringLiteral, other;

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

    private int strength, magic;

    private float gScale;

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
        strength = 10;
        magic = 10;

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

        gScale = GameObject.Find("Player").GetComponent<Rigidbody2D>().gravityScale;

        errText = GetComponentInChildren<TextMeshProUGUI>();

        StartCoroutine(LevelFade(true));

        if (!ignoreHints)
        {
            hints = new HintDisp[] { null };
            while (hints[0] == null)
            {
                StartCoroutine(FindHints());
            }
        }

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

        if (switchMenu.activeInHierarchy)
        {
            string selected = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;

            codeText.text = "";
            codeText.text += "public class ".HexEmbed(keyword) + "GameController".HexEmbed(type) + " : ".HexEmbed(other) + "MonoBehaviour\n".HexEmbed(type);
            codeText.text += "{\n".HexEmbed(other);
            codeText.text += "    private void ".HexEmbed(keyword) + "SwitchMode(".HexEmbed(other) + "string ".HexEmbed(keyword) + "newMode)\n".HexEmbed(other);
            codeText.text += "    {\n".HexEmbed(other);
            codeText.text += "        equipped = newMode;\n".HexEmbed(other);
            codeText.text += "        GameObject ".HexEmbed(type) + "player;\n".HexEmbed(other);
            codeText.text += "        Mover".HexEmbed(type) + "[] movers;\n\n".HexEmbed(other);
            codeText.text += "        switch ".HexEmbed(type) + "(selected)\n".HexEmbed(other);
            codeText.text += "        {\n".HexEmbed(other);

            switch (selected)
            {
                case "Platformer":
                    codeText.text += "            case ".HexEmbed(keyword) + "GameMode".HexEmbed(literal) + ".platformer:\n".HexEmbed(other);
                    codeText.text += "                player = ".HexEmbed(other) + "GameObject".HexEmbed(type) + ".Find(".HexEmbed(other) + "\"Player\"".HexEmbed(stringLiteral) + ");\n".HexEmbed(other);
                    codeText.text += "                movers = player.GetComponent<".HexEmbed(other) + "Mover".HexEmbed(type) + ">();\n\n".HexEmbed(other);
                    codeText.text += "                foreach ".HexEmbed(keyword) + "(".HexEmbed(other) + "Mover ".HexEmbed(type) + "mover ".HexEmbed(other) + "in ".HexEmbed(keyword) + "movers)\n".HexEmbed(other);
                    codeText.text += "                {\n".HexEmbed(other);
                    codeText.text += "                    if ".HexEmbed(keyword) + "(mover.GetType() == \n".HexEmbed(other);
                    codeText.text += "                        typeof".HexEmbed(keyword) + "(".HexEmbed(other) + "PFController".HexEmbed(type) + "))\n".HexEmbed(other);
                    codeText.text += "                    {\n".HexEmbed(other);
                    codeText.text += "                        mover.enabled = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "                    } ".HexEmbed(other) + "else ".HexEmbed(keyword) + "{\n".HexEmbed(other);
                    codeText.text += "                        mover.enabled = ".HexEmbed(other) + "false".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "                    }\n".HexEmbed(other);
                    codeText.text += "                }\n".HexEmbed(other);
                    break;

                case "RPG":
                    codeText.text += "            case ".HexEmbed(keyword) + "GameMode".HexEmbed(literal) + ".rpg:\n".HexEmbed(other);

                    codeText.text += "                player.transform.position = \n".HexEmbed(other);
                    codeText.text += "                    new ".HexEmbed(keyword) + "Vector3".HexEmbed(type) + "(\n".HexEmbed(other);
                    codeText.text += "                        GridLocker(\n".HexEmbed(other);
                    codeText.text += "                            player.transform.position.x\n".HexEmbed(other);
                    codeText.text += "                        ),\n".HexEmbed(other);
                    codeText.text += "                        GridLocker(\n".HexEmbed(other);
                    codeText.text += "                            player.transform.position.y\n".HexEmbed(other);
                    codeText.text += "                        ),\n".HexEmbed(other);
                    codeText.text += "                        1\n".HexEmbed(other);
                    codeText.text += "                    );\n".HexEmbed(other);
                    break;
            }

            codeText.text += "                break".HexEmbed(keyword) + ";\n".HexEmbed(other);
            codeText.text += "        }\n".HexEmbed(other);
            codeText.text += "    }\n".HexEmbed(other);
            codeText.text += "}".HexEmbed(other);
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
                if (GameObject.Find("Killbox") != null)
                {
                    GameObject.Find("Killbox").tag = "Killbox";
                }
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

                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Bounds"), true);

                foreach (GameObject enemy in enemies)
                {
                    enemy.transform.Find("EnemyHitbox").gameObject.SetActive(false);
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

                player.GetComponent<Rigidbody2D>().gravityScale = gScale;
                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    mover.transform.Find("Hitbox").gameObject.SetActive(false);
                    mover.transform.Find("GroundTrigger").gameObject.SetActive(true);
                    mover.transform.Find("GroundTrigger").GetComponent<BoxCollider2D>().size = new Vector2(0.71f, 0.69f);
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
                if (onMenu)
                {
                    Camera.main.GetComponent<CameraScroll>().enabled = false;
                }
                else
                {
                    Camera.main.GetComponent<CameraScroll>().enabled = true;
                }

                if (!ignoreHints)
                {
                    while (hints[0] == null)
                    {
                        StartCoroutine(FindHints());
                    }

                    SetHintDisp(0, true);
                    SetHintDisp(1, true);
                    SetHintDisp(2, shouldDisp[2]);
                    SetHintDisp(3, false);
                    SetHintDisp(4, false);
                    SetHintDisp(5, false);
                }
                break;

            case GameMode.rpg:
                if (GameObject.Find("Killbox") != null)
                {
                    GameObject.Find("Killbox").tag = "Killbox";
                }
                cameraWalls = Camera.main.GetComponentsInChildren<BoxCollider2D>(true);
                foreach (BoxCollider2D col in cameraWalls)
                {
                    col.gameObject.SetActive(true);
                }

                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Bounds"), true);

                foreach (GameObject enemy in enemies)
                {
                    enemy.transform.Find("EnemyHitbox").gameObject.SetActive(false);
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
                    enemy.transform.position = new Vector3(GridLocker(enemy.transform.position.x), GridLocker(enemy.transform.position.y), 0);
                }

                player = GameObject.Find("Player");

                player.GetComponent<Rigidbody2D>().gravityScale = 0;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    mover.transform.Find("GroundTrigger").GetComponent<BoxCollider2D>().size = new Vector2(0.71f, 0.69f);
                    mover.transform.Find("Hitbox").gameObject.SetActive(false);
                    if (mover.GetType().Equals(typeof(RPGController)))
                    {
                        mover.enabled = true;
                    }
                    else
                    {
                        mover.enabled = false;
                    }
                }

                player.transform.position = new Vector3(GridLocker(player.transform.position.x), GridLocker(player.transform.position.y), 0);

                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                Camera.main.projectionMatrix = Matrix4x4.Ortho(-5.3f * aspect, 5.3f * aspect, -5.3f, 5.3f, 0.3f, 1000.0f);
                Camera.main.GetComponent<FPSController>().enabled = false;
                Camera.main.GetComponent<CameraScroll>().enabled = true;

                if (!ignoreHints)
                {
                    while (hints[0] == null)
                    {
                        StartCoroutine(FindHints());
                    }

                    SetHintDisp(0, false);
                    SetHintDisp(1, false);
                    SetHintDisp(2, shouldDisp[2]);
                    SetHintDisp(3, true);
                    SetHintDisp(4, true);
                    SetHintDisp(5, false);
                }
                break;

            case GameMode.fps:
                if (GameObject.Find("Killbox") != null)
                {
                    GameObject.Find("Killbox").tag = "Killbox";
                }
                cameraWalls = Camera.main.GetComponentsInChildren<BoxCollider2D>(true);
                foreach (BoxCollider2D col in cameraWalls)
                {
                    col.gameObject.SetActive(false);
                }

                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Bounds"), true);

                foreach (GameObject enemy in enemies)
                {
                    enemy.transform.Find("EnemyHitbox").gameObject.SetActive(false);
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

                player.GetComponent<Rigidbody2D>().gravityScale = gScale;

                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    mover.transform.Find("GroundTrigger").GetComponent<BoxCollider2D>().size = new Vector2(0.71f, 0.69f);
                    mover.transform.Find("Hitbox").gameObject.SetActive(false);
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

                if (!ignoreHints)
                {
                    while (hints[0] == null)
                    {
                        StartCoroutine(FindHints());
                    }

                    SetHintDisp(0, false);
                    SetHintDisp(1, false);
                    SetHintDisp(2, shouldDisp[2]);
                    SetHintDisp(3, false);
                    SetHintDisp(4, false);
                    SetHintDisp(5, true);
                }
                break;

            case GameMode.fighting:
                if (GameObject.Find("Killbox") != null)
                {
                    GameObject.Find("Killbox").tag = "Untagged";
                }
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

                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Bounds"), false);

                foreach (GameObject enemy in enemies)
                {
                    enemy.transform.Find("EnemyHitbox").gameObject.SetActive(true);
                    enemy.GetComponent<FGEnemy>().hitstun = 0;

                    Camera cam = FindObjectOfType<Camera>();
                    Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

                    

                    if (GeometryUtility.TestPlanesAABB(planes, enemy.GetComponent<Collider2D>().bounds))
                    {
                        enemy.GetComponent<FGEnemy>().changedInView = true;
                    }
                    else
                    {
                        enemy.GetComponent<FGEnemy>().changedInView = false;
                    }

                    EnemyBehaviour[] behaviours = enemy.GetComponents<EnemyBehaviour>();

                    enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
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

                player.GetComponent<Rigidbody2D>().gravityScale = gScale;
                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    mover.GetComponent<FGController>().hitstun = 0;
                    mover.transform.Find("GroundTrigger").GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.69f);
                    mover.transform.Find("Hitbox").gameObject.SetActive(true);
                    mover.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
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

                if (!ignoreHints)
                {
                    while (hints[0] == null)
                    {
                        StartCoroutine(FindHints());
                    }

                    SetHintDisp(0, false);
                    SetHintDisp(1, false);
                    SetHintDisp(2, shouldDisp[2]);
                    SetHintDisp(3, false);
                    SetHintDisp(4, false);
                    SetHintDisp(5, false);
                }
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
        if (onMenu)
        {
            return;
        }

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
            Transform contentPanel = switchMenu.GetComponentInChildren<GridLayoutGroup>().transform;
            foreach (Mode mode in modes)
            {
                if (mode.unlocked)
                {
                    GameObject button = Instantiate(modeButton, contentPanel);
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

            if (!ignoreHints)
            {
                if (hints[2] != null && shouldDisp[2])
                {
                    SetHintDisp(2, false);
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
        currMP = maxMP;
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
        StartCoroutine(SpriteDamageFlash());
        if (currHP <= 0)
        {
            Die();
        }
    }

    public void Damage(int damage)
    {
        currHP = Mathf.Clamp(currHP - damage, 0, maxHP);
    }

    public void Cast(int mana)
    {
        currMP = Mathf.Clamp(currMP - mana, 0, maxMP);
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

    private IEnumerator FindHints()
    {
        while (hints[0] == null)
        {
            GameObject hintParent = GameObject.Find("Hints");
            if (hintParent != null)
            {
                hints = hintParent.GetComponentsInChildren<HintDisp>(true);
            }
            yield return new WaitForEndOfFrame();
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

    public IEnumerator FadeAndLoad(int buildIndex)
    {
        StartCoroutine(LevelFade(false));
        yield return new WaitForSeconds(levelFadeTime);
        SceneManager.LoadScene(buildIndex);
        yield return new WaitForEndOfFrame();
        if (!ignoreHints)
        {
            hints = new HintDisp[] { null };
            while (hints[0] == null)
            {
                StartCoroutine(FindHints());
                yield return new WaitForEndOfFrame();
            }
        }
        SwitchMode(GameMode.platformer);
        StartCoroutine(LevelFade(true));
        paused = false;
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

    public IEnumerator UnloadBattle()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(LevelFade(false));
        yield return new WaitForSeconds(levelFadeTime);
        SceneManager.UnloadSceneAsync(2);
        yield return new WaitForEndOfFrame();
        StartCoroutine(LevelFade(true));
        GameObject.Find("Player").GetComponent<RPGController>().SetEncountering(false);
        paused = false;
    }

    public void ErrDisp(string err)
    {
        errText.CrossFadeAlpha(1, 0, true);
        errText.text = err;
        StartCoroutine(ErrFade());
    }

    private IEnumerator ErrFade()
    {
        yield return new WaitForSeconds(errDispTime);
        errText.CrossFadeAlpha(0, errFadeTime, true);
    }

    public int GetStrength()
    {
        return strength;
    }

    public int GetMagic()
    {
        return magic;
    }
}