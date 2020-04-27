﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class GameController : MonoBehaviour
{
    /// <summary>
    /// A static variable storing a reference to the GameController singleton
    /// </summary>
    public static GameController singleton;

    public AudioClip rpgCombatBGM, datingSimBGM;

    private AudioClip currentBGM;

    public RenderTexture rTex;

    public GameObject gCanv;

    public RawImage rTarg;

    public bool glitching = false;

    public bool demoBuild;

    public enum GameMode
    {
        platformer,
        rpg,
        fps,
        fighting,
        rhythm,
        dating,
        racing
    };

    [Tooltip("Currently equipped gamemode. Should default to platformer.")]
    public GameMode equipped;

    private string equippedStr;

    [Tooltip("Resets equipped gamemode to platformer at start if enabled.")]
    public bool resetMode;

    [System.Serializable]
    public struct Mode
    {
        public string name;
        public bool unlocked;
        public Sprite sprite;
    }

    [Tooltip("Modes Unlocked. Should default to only platformer.")]
    public Mode[] modes;

    private int modeInt;

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

    public float unlockWaitTime;

    public float unlockFadeTime;

    private float inverseUnlockFadeTime;

    [Tooltip("The maximum HP for the player (only used in certain gamemodes)")]
    public int maxHP;

    [Tooltip("The maximum MP for the player (only used in certain gamemodes)")]
    public int maxMP;

    public int maxSP;

    [Tooltip("The amount of damage the player takes each step on a damage floor while in RPG mode")]
    public int floorDamage;

    [Tooltip("The amount of time in seconds for which the error text will be displayed before it begins to fade")]
    public float errDispTime;

    [Tooltip("The amount of time in seconds over which the error text will fade")]
    public float errFadeTime;

    public bool onMenu;

    public TextMeshProUGUI codeText;

    public GameObject unlockPanel;

    public GameObject pauseMenu;

    public GameObject settingsPanel;

    public GameObject quitPanel;

    public GameObject rhythmUnpausePanel;

    public TextMeshProUGUI unlockText;

    public GameObject staffPrefab;

    public Dialogue[] datingDialogues;

    public bool dating;

    private bool battling;

    [System.Serializable]
    public struct SpellStr
    {
        public Spell spell;
        public bool unlocked;
    }

    [System.Serializable]
    public struct SkillStr
    {
        public Skill skill;
        public bool unlocked;
    }

    [System.Serializable]
    public struct ArtStr
    {
        public string name;
        public Sprite img;
        public bool unlocked;
        [TextArea(3,10)]
        public string desc;
    }

    [System.Serializable]
    public struct SongStr
    {
        public string name;
        public Sprite img;
        public AudioClip song;
        public bool unlocked;
    }

    [Header("Unlockables")]
    public SpellStr[] spellList;
    public SkillStr[] skillList;
    public ArtStr[] artList;
    public SongStr[] songList;

    [Header("Code Highlight Colors")]
    public Color keyword, type, comment, literal, stringLiteral, other;

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

    private int currSP;

    private int gold;

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

    private float camSize;

    private bool unpausing;

    private bool dated;

    private bool loading;

    private bool waitAGoddamnSecond = false;

    private GameMode prev;

    private bool dying;

    /*private void OnEnable()
    {
        controls = new Controls();

        controls.Player.Menu.performed += MenuHandle;
        controls.UI.Cancel.performed += CancelHandle;
        controls.Player.SwitchMode.started += SwitchHandle;
        controls.Player.Pause.started += PauseHandle;

        controls.Player.Menu.Enable();
        controls.UI.Cancel.Enable();
        controls.Player.SwitchMode.Enable();
        controls.Player.Pause.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Menu.performed -= MenuHandle;
        controls.UI.Cancel.performed -= CancelHandle;
        controls.Player.SwitchMode.started -= SwitchHandle;
        controls.Player.Pause.started -= PauseHandle;

        controls.Player.Menu.Disable();
        controls.UI.Cancel.Disable();
        controls.Player.SwitchMode.Disable();
        controls.Player.Pause.Disable();
    }*/

    public void OnSwitch (InputValue value)
    {
        if (paused || battling || dying || FindObjectOfType<RPGController>().shopping)
        {
            return;
        }

        if (DialogueManager.singleton.GetDisplaying() && !dating)
        {
            return;
        }

        if (CutsceneManager.singleton.scening)
        {
            return;
        }

        if (numUnlocked <= 1)
        {
            return;
        }

        float dir = value.Get<float>();

        List<Mode> unlocked = new List<Mode>();
        int i = 0;

        foreach (Mode mode in modes)
        {
            if (mode.unlocked)
            {
                unlocked.Add(mode);
                
                if (mode.name == equippedStr)
                {
                    modeInt = i;
                }

                i++;
            }
        }

        if (dir > 0)
        {
            if (modeInt == unlocked.ToArray().Length-1)
            {
                modeInt = 0;
            }
            else
            {
                modeInt++;
            }

            SwitchMode(unlocked[modeInt].name);
        }

        if (dir < 0)
        {
            if (modeInt == 0)
            {
                modeInt = unlocked.ToArray().Length-1;
            }
            else
            {
                modeInt--;
            }

            SwitchMode(unlocked[modeInt].name);
        }
    }

    public void OnPause(InputValue value)
    {
        if (unpausing || battling || FindObjectOfType<RPGController>().shopping)
        {
            return;
        }

        if (switchMenu.activeInHierarchy)
        {
            ToggleSwitchMenu();
        }
        else if (pauseMenu.activeInHierarchy && !(settingsPanel.activeInHierarchy || quitPanel.activeInHierarchy))
        {
            /*if (waitAGoddamnSecond)
            {
                waitAGoddamnSecond = false;
            }
            else
            {*/
                Unpause();
            //}
        }
        else
        {
            Pause();
        }
    }

    public void OnMenu (InputValue value)
    {
        if (numUnlocked < 2 || (paused && !switchMenu.activeInHierarchy) || FindObjectOfType<RPGController>().shopping)
        {
            return;
        }

        ToggleSwitchMenu();
    }

    public void OnCancel (InputValue value)
    {
        AkSoundEngine.PostEvent("sfx_ui_back", gameObject);

        if (switchMenu.activeInHierarchy)
        {
            ToggleSwitchMenu();
        }

        if (settingsPanel.activeInHierarchy)
        {
            CloseSettingsMenu();
            waitAGoddamnSecond = true;
        }

        if (quitPanel.activeInHierarchy)
        {
            CloseQuitMenu();
            waitAGoddamnSecond = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (singleton == null)
        {
            DontDestroyOnLoad(gameObject);
            singleton = this;
            SettingsController.singleton = GetComponentInChildren<SettingsController>();
            settingsPanel.SetActive(false);
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        dated = false;

        inverseDamageFadeTime = 1.0f / damageFadeTime;

        inverseLevelFadeTime = 1.0f / levelFadeTime;

        inverseUnlockFadeTime = 1.0f / unlockFadeTime;

        currHP = maxHP;
        currMP = maxMP;
        currSP = maxSP;
        strength = 10;
        magic = 10;
        gold = 0;

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
        camSize = Camera.main.orthographicSize;

        errText = GetComponentInChildren<TextMeshProUGUI>();
        
        SaveManager.singleton.Init();

        StartCoroutine(LevelFade(true));

        if (resetUnlocks)
        {
            ResetUnlocks();
        }

        numUnlocked = 0;
        foreach (Mode mode in modes)
        {
            if (mode.unlocked)
            {
                numUnlocked++;
            }
        }
        
        if (resetMode)
        {
            equipped = GameMode.platformer;
        }

        currentBGM = GameObject.Find("Song").GetComponent<AudioSource>().clip;

        StartCoroutine(Switch());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        AkSoundEngine.PostEvent("start_game", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked && !Cursor.visible && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {
            Cursor.visible = true;
        }
        
        if (glitching)
        {
            GameObject gCam = GameObject.Find("GCam");
            if (gCam != null)
            {
                gCam.SetActive(true);
            }

            Camera.main.GetComponent<Camera>().targetTexture = rTex;
            rTarg.texture = rTex;
            Resources.UnloadUnusedAssets();
        }
        else
        {
            GameObject gCam = GameObject.Find("GCam");
            if (gCam != null)
            {
                gCam.SetActive(false);
            }
        }

        if (switchMenu.activeInHierarchy)
        {
            string selected;

            if (EventSystem.current.currentSelectedGameObject != null)
            {
                selected = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;
            }
            else
            {
                selected = "";
            }
            
            switch (selected)
            {
                case "Platformer":
                    codeText.text = "// Platformer Game\n\n".HexEmbed(comment);
                    codeText.text += "float ".HexEmbed(keyword) + "moveSpeed = ".HexEmbed(other) + "7.5f".HexEmbed(literal) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "canJump = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "float ".HexEmbed(keyword) + "jumpHeight = ".HexEmbed(other) + "4.5f".HexEmbed(literal) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "canGrabWall = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "\nenemy.oneHitKill = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "enemy.chargeOnSight = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    break;

                case "Fighting":
                    codeText.text = "// Fighting Game\n\n".HexEmbed(comment);
                    codeText.text += "float ".HexEmbed(keyword) + "moveSpeed = ".HexEmbed(other) + "6.0f".HexEmbed(literal) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "canJump = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "float ".HexEmbed(keyword) + "jumpHeight = ".HexEmbed(other) + "1.5f".HexEmbed(literal) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "canGrabWall = ".HexEmbed(other) + "false".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "canAttack = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "\nif ".HexEmbed(keyword) + "(inputString.Equals(".HexEmbed(other) + "\"↓🡖🡒A\"".HexEmbed(stringLiteral) + ")) {\n".HexEmbed(other);
                    codeText.text += "    SpawnProjectile();\n".HexEmbed(other);
                    codeText.text += "}\n".HexEmbed(other);
                    codeText.text += "\nenemy.oneHitKill = ".HexEmbed(other) + "false".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "enemy.ActivateAI();\n".HexEmbed(other);
                    break;

                case "RPG":
                    codeText.text = "// Role Playing Game\n\n".HexEmbed(comment);
                    codeText.text += "float ".HexEmbed(keyword) + "moveSpeed = ".HexEmbed(other) + "3.0f".HexEmbed(literal) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "canJump = ".HexEmbed(other) + "false".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "\nMovement".HexEmbed(type) + ".SetMode(".HexEmbed(other) + "Movement".HexEmbed(type) + ".gridBased);\n".HexEmbed(other);
                    codeText.text += "\nif ".HexEmbed(keyword) + "(player.isWalking) {\n".HexEmbed(other);
                    codeText.text += "    int ".HexEmbed(keyword) + "num = Random.RNG(".HexEmbed(other) + "15".HexEmbed(literal) + ");\n".HexEmbed(other);
                    codeText.text += "    if ".HexEmbed(keyword) + "(num == ".HexEmbed(other) + "0".HexEmbed(literal) + ") {\n".HexEmbed(other);
                    codeText.text += "        StartBattle();\n".HexEmbed(other);
                    codeText.text += "    }\n".HexEmbed(other);
                    codeText.text += "}\n".HexEmbed(other);
                    codeText.text += "\nReplaceAll(enemy, npc);\n".HexEmbed(other);
                    codeText.text += "npc.canChat = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    break;

                case "Rhythm":
                    codeText.text = "// Rhythm Game\n\n".HexEmbed(comment);
                    codeText.text += "float ".HexEmbed(keyword) + "moveSpeed = ".HexEmbed(other) + "slow".HexEmbed(literal) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "collides = ".HexEmbed(other) + "false".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "dancing = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "int ".HexEmbed(keyword) + "numMisses = ".HexEmbed(other) + "3".HexEmbed(literal) + ";\n".HexEmbed(other);
                    codeText.text += "\nMovement".HexEmbed(type) + ".SetMode(".HexEmbed(other) + "Movement".HexEmbed(type) + ".autoScroll);\n".HexEmbed(other);
                    codeText.text += "Unpause".HexEmbed(type) + ".SetCountdown(".HexEmbed(other) + "true".HexEmbed(keyword) + ");\n".HexEmbed(other);
                    codeText.text += "\nChangeSong(rhythmSong[currentLevel]);".HexEmbed(other);
                    break;

                case "Dating":
                    codeText.text = "// Dating Sim\n\n".HexEmbed(comment);
                    codeText.text += "bool ".HexEmbed(keyword) + "canJump = ".HexEmbed(other) + "false".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "canMove = ".HexEmbed(other) + "false".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "canDoAnythingButTalk = ".HexEmbed(other) + "false".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "\nMovement".HexEmbed(type) + ".SetMode(".HexEmbed(other) + "Movement".HexEmbed(type) + ".none);\n".HexEmbed(other);
                    codeText.text += "\nReplace(enemy, lover);\n".HexEmbed(other);
                    codeText.text += "lover.relationship = ".HexEmbed(other) + "0".HexEmbed(literal) + ";\n".HexEmbed(other);
                    codeText.text += "\nif ".HexEmbed(keyword) + "(lover.relationship >= ".HexEmbed(other) + "100".HexEmbed(literal) + ") {\n".HexEmbed(other);
                    codeText.text += "    OpenDateMap();\n".HexEmbed(other);
                    codeText.text += "}\n".HexEmbed(other);
                    break;

                case "Racing":
                    codeText.text = "// Racing Game\n\n".HexEmbed(comment);
                    codeText.text += "bool ".HexEmbed(keyword) + "canJump = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "canTilt = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "\nif ".HexEmbed(keyword) + "(onGround && tilt > 45) {\n".HexEmbed(other);
                    codeText.text += "    Crash();\n".HexEmbed(other);
                    codeText.text += "}\n".HexEmbed(other);
                    codeText.text += "\nif ".HexEmbed(keyword) + "(player.collidingWithWall) {\n".HexEmbed(other);
                    codeText.text += "    Crash();\n".HexEmbed(other);
                    codeText.text += "}\n".HexEmbed(other);
                    codeText.text += "\nif ".HexEmbed(keyword) + "(player.collidingWithEnemy) {\n".HexEmbed(other);
                    codeText.text += "    Crash();\n".HexEmbed(other);
                    codeText.text += "}\n".HexEmbed(other);
                    codeText.text += "\nSpawnRacers(".HexEmbed(other) + "3".HexEmbed(literal) + ");".HexEmbed(other);
                    break;

                case "FPS":
                    codeText.text = "// First Person Shooter\n\n".HexEmbed(comment);
                    codeText.text += "bool ".HexEmbed(keyword) + "canJump = ".HexEmbed(other) + "false".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "canMove = ".HexEmbed(other) + "false".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "cameraControl = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "bool ".HexEmbed(keyword) + "canZoom = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);

                    codeText.text += "bool ".HexEmbed(keyword) + "invertY = ".HexEmbed(other);
                    codeText.text += SettingsController.singleton.invertY ? "true".HexEmbed(keyword) + ";".HexEmbed(other) + " // You heathen\n".HexEmbed(comment)
                        : "false".HexEmbed(keyword) + ";\n".HexEmbed(other);

                    codeText.text += "\nenemy.oneHitKill = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    codeText.text += "enemy.chargeOnSight = ".HexEmbed(other) + "true".HexEmbed(keyword) + ";\n".HexEmbed(other);
                    break;
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

    public void SwitchMode(GameMode newMode)
    {
        SwitchMode(newMode, false, false);
    }

    public void SwitchMode(GameMode newMode, bool transitioned, bool reload)
    {
        if (!reload)
        {
            switch (equipped)
            {
                case GameMode.fps:
                case GameMode.platformer:
                    if (GameObject.Find("Player").GetComponent<PFController>().GetDying())
                    {
                        return;
                    }
                    break;

                case GameMode.fighting:
                    if (GameObject.Find("Player").GetComponent<FGController>().GetDying())
                    {
                        return;
                    }
                    break;

                case GameMode.rhythm:
                    if (GameObject.Find("Player").GetComponent<RhythmController>().GetDying())
                    {
                        return;
                    }
                    break;

                case GameMode.racing:
                    if (GameObject.Find("Player").GetComponent<RaceController>().GetDying())
                    {
                        return;
                    }
                    break;
            }
        }

        prev = equipped;
        equipped = newMode;

        if (prev == GameMode.dating && !transitioned)
        {
            StartCoroutine(DatingUntransition());
        }

        BoxCollider2D[] cameraWalls;
        GameObject player;
        Mover[] movers;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float aspect = (float)Screen.width / (float)Screen.height;
        RPGController rpgCon;
        RhythmController rCon;
        GameObject staff;
        AudioSource source;

       //This effect is triggering on start for some reason 
        //AkSoundEngine.PostEvent("sfx_game_mode_switch", gameObject);

        switch (equipped)
        {
            #region platformer
            case GameMode.platformer:
                modeInt = 0;
                equippedStr = "Platformer";

                if (DialogueManager.singleton.GetDisplaying() && prev != GameMode.dating)
                {
                    DialogueManager.singleton.EndDialogue(false);
                }

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
                            behaviour.GetAnimator().SetBool("platformer", true);
                            behaviour.GetAnimator().SetBool("fighter", false);
                        }
                        else
                        {
                            behaviour.enabled = false;
                        }
                    }
                    if (enemy.GetComponent<Rigidbody2D>() != null)
                    {
                        enemy.GetComponent<Rigidbody2D>().gravityScale = 1;
                    }
                }

                player = GameObject.Find("Player");

                rpgCon = player.GetComponent<RPGController>();
                if (rpgCon.mvmtCoroutine != null)
                {
                    rpgCon.StopCoroutine(rpgCon.mvmtCoroutine);
                    rpgCon.SetMoving(false);
                    rpgCon.mvmtCoroutine = null;
                }

                rCon = player.GetComponent<RhythmController>();
                if (rCon.enabled)
                {
                    player.transform.position = rCon.inGround ? rCon.startPos : player.transform.position;
                    Camera.main.transform.position = new Vector3(
                        player.transform.position.x,
                        Camera.main.transform.position.y,
                        Camera.main.transform.position.z
                        );
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
                        mover.GetAnimator().SetBool("platformer", true);
                        mover.GetAnimator().SetBool("fighter", false);
                        mover.GetAnimator().SetBool("rpg", false);
                        mover.GetAnimator().SetBool("rhythm", false);
                    }
                    else
                    {
                        mover.hor = 0;
                        mover.ver = 0;
                        mover.hRaw = 0;
                        mover.vRaw = 0;
                        mover.enabled = false;
                    }
                }

                player.GetComponent<CapsuleCollider2D>().enabled = true;
                player.transform.Find("RacerCol").gameObject.SetActive(false);

                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                Camera.main.projectionMatrix = Matrix4x4.Ortho(-camSize * aspect, camSize * aspect, -camSize, camSize, 0.3f, 1000.0f);
                Camera.main.GetComponent<FPSController>().enabled = false;
                Camera.main.GetComponent<CameraScroll>().enabled = true;
                if (onMenu)
                {
                    Camera.main.GetComponent<CameraScroll>().hScroll = false;
                }
                else
                {
                    Camera.main.GetComponent<CameraScroll>().hScroll = true;
                }

                staff = GameObject.Find("MusicalStaff(Clone)");
                if (staff != null)
                {
                    Destroy(staff);
                }

                source = GameObject.Find("Song").GetComponent<AudioSource>();
                if (!source.isPlaying)
                {
                    source.Play();
                }
                break;
            #endregion
            #region rpg
            case GameMode.rpg:
                modeInt = 2;
                equippedStr = "RPG";

                if (DialogueManager.singleton.GetDisplaying() && prev != GameMode.dating)
                {
                    DialogueManager.singleton.EndDialogue(false);
                }

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
                RaycastHit2D hit;

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

                    if (enemy.GetComponent<Rigidbody2D>() != null)
                    {
                        enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        enemy.GetComponent<Rigidbody2D>().gravityScale = 0;
                    }

                    enemy.transform.position = new Vector3(GridLocker(enemy.transform.position.x), GridLocker(enemy.transform.position.y), 0);
                    
                    hit = Physics2D.BoxCast(enemy.transform.position, Vector2.one * 0.975f, 0, Vector2.zero, 0, ~((1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("Enemy2")) + (1 << LayerMask.NameToLayer("Floor"))));
                    if (hit.collider != null)
                    {
                        enemy.transform.position += Vector3.up;
                        enemy.transform.position = new Vector3(GridLocker(enemy.transform.position.x), GridLocker(enemy.transform.position.y), 0);
                    }
                }

                player = GameObject.Find("Player");

                player.GetComponent<Rigidbody2D>().gravityScale = 0;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                
                player.GetComponent<CapsuleCollider2D>().enabled = true;
                player.transform.Find("RacerCol").gameObject.SetActive(false);

                rCon = player.GetComponent<RhythmController>();
                if (rCon.enabled)
                {
                    player.transform.position = rCon.inGround ? rCon.startPos : player.transform.position;
                    Camera.main.transform.position = new Vector3(
                        player.transform.position.x,
                        Camera.main.transform.position.y,
                        Camera.main.transform.position.z
                        );
                }

                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    mover.transform.Find("GroundTrigger").GetComponent<BoxCollider2D>().size = new Vector2(0.71f, 0.69f);
                    mover.transform.Find("Hitbox").gameObject.SetActive(false);
                    if (mover.GetType().Equals(typeof(RPGController)))
                    {
                        mover.enabled = true;
                        mover.GetAnimator().SetBool("moving", false);
                        mover.GetAnimator().SetBool("rpg", true);
                        mover.GetAnimator().SetInteger("dir", 0);
                        mover.GetAnimator().SetBool("fighter", false);
                        mover.GetAnimator().SetBool("platformer", false);
                        mover.GetAnimator().SetBool("rhythm", false);
                    }
                    else
                    {
                        mover.hor = 0;
                        mover.ver = 0;
                        mover.hRaw = 0;
                        mover.vRaw = 0;
                        mover.enabled = false;
                    }
                }

                player.transform.position = new Vector3(GridLocker(player.transform.position.x), GridLocker(player.transform.position.y), 0);
                
                hit = Physics2D.BoxCast(player.transform.position, Vector2.one * 0.975f, 0, Vector2.zero, 0, ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));
                if (hit.collider != null)
                {
                    player.transform.position += Vector3.up;
                    player.transform.position = new Vector3(GridLocker(player.transform.position.x), GridLocker(player.transform.position.y), 0);
                }

                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                Camera.main.projectionMatrix = Matrix4x4.Ortho(-camSize * aspect, camSize * aspect, -camSize, camSize, 0.3f, 1000.0f);
                Camera.main.GetComponent<FPSController>().enabled = false;
                Camera.main.GetComponent<CameraScroll>().enabled = true;

                staff = GameObject.Find("MusicalStaff(Clone)");
                if (staff != null)
                {
                    Destroy(staff);
                }

                source = GameObject.Find("Song").GetComponent<AudioSource>();
                if (!source.isPlaying)
                {
                    source.Play();
                }
                break;
            #endregion
            #region fps
            case GameMode.fps:
                modeInt = 6;
                equippedStr = "FPS";

                if (DialogueManager.singleton.GetDisplaying() && prev != GameMode.dating)
                {
                    DialogueManager.singleton.EndDialogue(false);
                }

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
                        behaviour.GetAnimator().SetBool("platformer", true);
                        behaviour.GetAnimator().SetBool("fighter", false);
                        if (behaviour.GetType().Equals(typeof(PFEnemy)))
                        {
                            behaviour.enabled = true;
                        }
                        else
                        {
                            behaviour.enabled = false;
                        }
                    }
                    if (enemy.GetComponent<Rigidbody2D>() != null)
                    {
                        enemy.GetComponent<Rigidbody2D>().gravityScale = 1;
                    }
                }

                player = GameObject.Find("Player");

                player.GetComponent<Rigidbody2D>().gravityScale = gScale;
                
                player.GetComponent<CapsuleCollider2D>().enabled = true;
                player.transform.Find("RacerCol").gameObject.SetActive(false);

                rpgCon = player.GetComponent<RPGController>();
                if (rpgCon.mvmtCoroutine != null)
                {
                    rpgCon.StopCoroutine(rpgCon.mvmtCoroutine);
                    rpgCon.SetMoving(false);
                    rpgCon.mvmtCoroutine = null;
                }

                rCon = player.GetComponent<RhythmController>();
                if (rCon.enabled)
                {
                    player.transform.position = rCon.inGround ? rCon.startPos : player.transform.position;
                    Camera.main.transform.position = new Vector3(
                        player.transform.position.x,
                        Camera.main.transform.position.y,
                        Camera.main.transform.position.z
                        );
                }

                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    if (!mover.GetAnimator().GetBool("platformer"))
                    {
                        mover.GetAnimator().SetBool("platformer", true);
                        mover.GetAnimator().SetBool("fighter", false);
                        mover.GetAnimator().SetBool("rpg", false);
                        mover.GetAnimator().SetBool("rhythm", false);
                    }
                    mover.transform.Find("GroundTrigger").GetComponent<BoxCollider2D>().size = new Vector2(0.71f, 0.69f);
                    mover.transform.Find("Hitbox").gameObject.SetActive(false);
                    mover.hor = 0;
                    mover.ver = 0;
                    mover.hRaw = 0;
                    mover.vRaw = 0;
                    mover.enabled = false;
                }

                Camera.main.transform.position = new Vector3(
                    Mathf.Clamp(player.transform.position.x, Camera.main.GetComponent<CameraScroll>().min.x, Camera.main.GetComponent<CameraScroll>().max.x),
                    Mathf.Clamp(player.transform.position.y, Camera.main.GetComponent<CameraScroll>().min.y, Camera.main.GetComponent<CameraScroll>().max.y) + Camera.main.GetComponent<CameraScroll>().yOffset,
                    Camera.main.transform.position.z);
                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                Camera.main.projectionMatrix = Matrix4x4.Perspective(95.52f, aspect, 0.3f, 1000.0f);
                Camera.main.GetComponent<CameraScroll>().enabled = false;
                Camera.main.GetComponent<FPSController>().enabled = true;
                staff = GameObject.Find("MusicalStaff(Clone)");

                if (staff != null)
                {
                    Destroy(staff);
                }

                source = GameObject.Find("Song").GetComponent<AudioSource>();
                if (!source.isPlaying)
                {
                    source.Play();
                }
                break;
            #endregion
            #region fighting
            case GameMode.fighting:
                modeInt = 1;
                equippedStr = "Fighting";

                if (DialogueManager.singleton.GetDisplaying() && prev != GameMode.dating)
                {
                    DialogueManager.singleton.EndDialogue(false);
                }

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
                    if (enemy.GetComponent<Rigidbody2D>() != null)
                    {
                        enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
                    }
                    foreach (EnemyBehaviour behaviour in behaviours)
                    {
                        if (behaviour.GetType().Equals(typeof(FGEnemy)))
                        {
                            behaviour.enabled = true;
                            behaviour.GetAnimator().SetBool("fighter", true);
                            behaviour.GetAnimator().SetBool("platformer", false);
                        }
                        else
                        {
                            behaviour.enabled = false;
                        }
                    }
                    if (enemy.GetComponent<Rigidbody2D>() != null)
                    {
                        enemy.GetComponent<Rigidbody2D>().gravityScale = 1;
                    }
                }

                player = GameObject.Find("Player");
                
                rpgCon = player.GetComponent<RPGController>();
                if (rpgCon.mvmtCoroutine != null)
                {
                    rpgCon.StopCoroutine(rpgCon.mvmtCoroutine);
                    rpgCon.SetMoving(false);
                    rpgCon.mvmtCoroutine = null;
                }

                rCon = player.GetComponent<RhythmController>();
                if (rCon.enabled)
                {
                    player.transform.position = rCon.inGround ? rCon.startPos : player.transform.position;
                    Camera.main.transform.position = new Vector3(
                        player.transform.position.x,
                        Camera.main.transform.position.y,
                        Camera.main.transform.position.z
                        );
                }

                player.GetComponent<Rigidbody2D>().gravityScale = gScale;

                player.GetComponent<CapsuleCollider2D>().enabled = true;
                player.transform.Find("RacerCol").gameObject.SetActive(false);

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
                        Debug.Log("Changing to fighter");
                        mover.GetAnimator().SetBool("fighter", true);
                        mover.GetAnimator().SetBool("platformer", false);
                        mover.GetAnimator().SetBool("rpg", false);
                        mover.GetAnimator().SetBool("rhythm", false);
                    }
                    else
                    {
                        mover.hor = 0;
                        mover.ver = 0;
                        mover.hRaw = 0;
                        mover.vRaw = 0;
                        mover.enabled = false;
                    }
                }

                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                Camera.main.projectionMatrix = Matrix4x4.Ortho(-camSize * aspect, camSize * aspect, -camSize, camSize, 0.3f, 1000.0f);
                Camera.main.GetComponent<FPSController>().enabled = false;
                Camera.main.GetComponent<CameraScroll>().enabled = true;

                staff = GameObject.Find("MusicalStaff(Clone)");
                if (staff != null)
                {
                    Destroy(staff);
                }

                source = GameObject.Find("Song").GetComponent<AudioSource>();
                if (!source.isPlaying)
                {
                    source.Play();
                }
                break;
            #endregion
            #region rhythm
            case GameMode.rhythm:
                modeInt = 3;
                equippedStr = "Rhythm";

                if (DialogueManager.singleton.GetDisplaying() && prev != GameMode.dating)
                {
                    DialogueManager.singleton.EndDialogue(false);
                }

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

                    if (enemy.GetComponent<Rigidbody2D>() != null)
                    {
                        enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        enemy.GetComponent<Rigidbody2D>().gravityScale = 0;
                    }
                }

                player = GameObject.Find("Player");

                rpgCon = player.GetComponent<RPGController>();
                if (rpgCon.mvmtCoroutine != null)
                {
                    rpgCon.StopCoroutine(rpgCon.mvmtCoroutine);
                    rpgCon.SetMoving(false);
                    rpgCon.mvmtCoroutine = null;
                }

                player.GetComponent<Rigidbody2D>().gravityScale = 0;
                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    if (mover.GetType().Equals(typeof(RhythmController)))
                    {
                        mover.enabled = true;
                        mover.GetAnimator().SetBool("rhythm", true);
                        mover.GetAnimator().SetBool("platformer", false);
                        mover.GetAnimator().SetBool("fighter", false);
                        mover.GetAnimator().SetBool("rpg", false);
                    }
                    else
                    {
                        mover.hor = 0;
                        mover.ver = 0;
                        mover.hRaw = 0;
                        mover.vRaw = 0;
                        mover.enabled = false;
                    }
                }

                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<CapsuleCollider2D>().enabled = false;
                player.transform.Find("RacerCol").gameObject.SetActive(false);

                player.transform.rotation = Quaternion.identity;

                Vector3 staffSpawn = player.transform.position;
                staffSpawn += new Vector3(0.7f, -0.05f, 0);
                Instantiate(staffPrefab, staffSpawn, Quaternion.identity);
                
                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                Camera.main.projectionMatrix = Matrix4x4.Ortho(-camSize * aspect, camSize * aspect, -camSize, camSize, 0.3f, 1000.0f);
                Camera.main.GetComponent<FPSController>().enabled = false;
                Camera.main.GetComponent<CameraScroll>().enabled = true;
                if (onMenu)
                {
                    Camera.main.GetComponent<CameraScroll>().hScroll = false;
                }
                else
                {
                    Camera.main.GetComponent<CameraScroll>().hScroll = true;
                }

                GameObject.Find("Song").GetComponent<AudioSource>().Stop();

                rCon = player.GetComponent<RhythmController>();
                rCon.StartCoroutine(rCon.StartRhythm());
                break;
            #endregion
            #region dating
            case GameMode.dating:
                modeInt = 4;
                equippedStr = "Dating";

                staff = GameObject.Find("MusicalStaff(Clone)");
                if (staff != null)
                {
                    Destroy(staff);
                }

                StartCoroutine(DatingTransition());
                break;
            #endregion
            #region racing
            case GameMode.racing:
                modeInt = 5;
                equippedStr = "Racing";

                if (DialogueManager.singleton.GetDisplaying() && prev != GameMode.dating)
                {
                    DialogueManager.singleton.EndDialogue(false);
                }

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

                foreach (GameObject enemy in enemies)
                {
                    enemy.transform.Find("EnemyHitbox").gameObject.SetActive(false);
                    EnemyBehaviour[] behaviours = enemy.GetComponents<EnemyBehaviour>();

                    foreach (EnemyBehaviour behaviour in behaviours)
                    {
                        if (behaviour.GetType().Equals(typeof(RaceObst)))
                        {
                            behaviour.enabled = true;
                        }
                        else
                        {
                            behaviour.enabled = false;
                        }
                    }
                    if (enemy.GetComponent<Rigidbody2D>() != null)
                    {
                        enemy.GetComponent<Rigidbody2D>().gravityScale = 1;
                    }
                }

                player = GameObject.Find("Player");

                rpgCon = player.GetComponent<RPGController>();
                if (rpgCon.mvmtCoroutine != null)
                {
                    rpgCon.StopCoroutine(rpgCon.mvmtCoroutine);
                    rpgCon.SetMoving(false);
                    rpgCon.mvmtCoroutine = null;
                }

                rCon = player.GetComponent<RhythmController>();
                if (rCon.enabled)
                {
                    player.transform.position = rCon.inGround ? rCon.startPos : player.transform.position;
                    Camera.main.transform.position = new Vector3(
                        player.transform.position.x,
                        Camera.main.transform.position.y,
                        Camera.main.transform.position.z
                        );
                }

                player.GetComponent<Rigidbody2D>().gravityScale = gScale;
                movers = player.GetComponents<Mover>();
                foreach (Mover mover in movers)
                {
                    mover.transform.Find("Hitbox").gameObject.SetActive(false);
                    mover.transform.Find("GroundTrigger").gameObject.SetActive(true);
                    mover.transform.Find("GroundTrigger").GetComponent<BoxCollider2D>().size = new Vector2(0.71f, 0.69f);
                    if (mover.GetType().Equals(typeof(RaceController)))
                    {
                        mover.enabled = true;
                    }
                    else
                    {
                        mover.hor = 0;
                        mover.ver = 0;
                        mover.hRaw = 0;
                        mover.vRaw = 0;
                        mover.enabled = false;
                    }
                }

                player.GetComponent<CapsuleCollider2D>().enabled = false;
                player.transform.Find("RacerCol").gameObject.SetActive(true);

                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                Camera.main.projectionMatrix = Matrix4x4.Ortho(-camSize * aspect, camSize * aspect, -camSize, camSize, 0.3f, 1000.0f);
                Camera.main.GetComponent<FPSController>().enabled = false;
                Camera.main.GetComponent<CameraScroll>().enabled = true;
                if (onMenu)
                {
                    Camera.main.GetComponent<CameraScroll>().hScroll = false;
                }
                else
                {
                    Camera.main.GetComponent<CameraScroll>().hScroll = true;
                }

                staff = GameObject.Find("MusicalStaff(Clone)");
                if (staff != null)
                {
                    Destroy(staff);
                }

                source = GameObject.Find("Song").GetComponent<AudioSource>();
                if (!source.isPlaying)
                {
                    source.Play();
                }
                break;
            #endregion

            default:
                Debug.Log("ERROR: INVALID GAME MODE");
                break;
        }

        UpdateSwitchPanel();

        if (switchMenu.activeInHierarchy)
        {
            ToggleSwitchMenu();
        }
    }

    private IEnumerator DatingTransition()
    {
        dating = true;
        StartCoroutine(LevelFade(false));
        AkSoundEngine.PostEvent("Dating_Sim", gameObject);
        yield return new WaitForSeconds(levelFadeTime);
        SetGlitching(false);

        // TOOD: Switch music to Dating Sim music

        ToggleSwitchPanel(false);
        if (datingDialogues.Length > 1)
        {
            if (dated)
            {
                int i = Random.Range(1, datingDialogues.Length);
                DialogueManager.singleton.StartDialogue(datingDialogues[i], true, i);
            }
            else
            {
                DialogueManager.singleton.StartDialogue(datingDialogues[0], true, 0);
            }
        }
        else
        {
            DialogueManager.singleton.StartDialogue(datingDialogues[0], true, 0);
        }
        StartCoroutine(LevelFade(true));
    }
    
    private IEnumerator DatingUntransition()
    {
        StartCoroutine(LevelFade(false));
        AkSoundEngine.PostEvent((uint)GameObject.Find("AudioPlayer").GetComponent<AkAmbient>().eventID, gameObject);
        yield return new WaitForSeconds(levelFadeTime);
        if (DialogueManager.singleton.GetDisplaying())
        {
            DialogueManager.singleton.EndDialogue(false);
        }

        if (equipped != GameMode.rhythm)
        {
            AudioSource source = GameObject.Find("Song").GetComponent<AudioSource>();
            source.clip = currentBGM;
            source.Play();
        }

        ToggleSwitchPanel(true);
        StartCoroutine(LevelFade(true));
        yield return new WaitForSeconds(0.1f);
        dating = false;
    }

    public static float GridLocker(float pos)
    {
        pos = Mathf.Floor(pos * 2) / 2;

        if (pos - Mathf.Floor(pos) == 0)
        {
            pos += 0.5f;
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
            Cursor.visible = false;
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

                    Image img = null;
                    Image[] images = button.GetComponentsInChildren<Image>();
                    foreach (Image image in images)
                    {
                        if (image.name.Equals("Icon"))
                        {
                            img = image;
                        }
                    }

                    if (img != null)
                    {
                        img.sprite = mode.sprite;
                    }

                    if (EventSystem.current.currentSelectedGameObject == null)
                    {
                        EventSystem.current.SetSelectedGameObject(button);
                    }
                }
            }

            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Unlock(GameMode mode)
    {
        string modeStr = "";
        switch (mode)
        {
            case GameMode.platformer:
                modeStr = "Platformer";
                break;

            case GameMode.fighting:
                modeStr = "Fighting";
                break;

            case GameMode.rpg:
                modeStr = "RPG";
                break;

            case GameMode.fps:
                modeStr = "FPS";
                break;

            case GameMode.rhythm:
                modeStr = "Rhythm";
                break;

            case GameMode.dating:
                modeStr = "Dating";
                break;
        }

        Unlock(modeStr);
    }

    public void Unlock(string mode)
    {
        bool found = false;
        for (int i = 0; i < modes.Length && !found; i++)
        {
            if (modes[i].name.Equals(mode))
            {
                modes[i].unlocked = true;
                unlockPanel.SetActive(true);
                if (mode.Equals("Fighting") || mode.Equals("Rhythm") || mode.Equals("Racing"))
                {
                    mode += " Game";
                }
                else if (mode.Equals("Dating"))
                {
                    mode += " Sim";
                    dated = false;
                }
                unlockText.text = "You unlocked \n" + mode + " mode!";
                found = true;
                // TODO: Add Unlock Mode SFX Event
                StartCoroutine(UnlockFade());
            }
        }

        numUnlocked++;

        if (!found)
        {
            Debug.Log("ERROR: MODE STRING DID NOT MATCH ANY NAMES IN MODES ARRAY");
        }
    }

    public void Lock(GameMode mode)
    {
        string modeStr = "";
        switch (mode)
        {
            case GameMode.platformer:
                modeStr = "Platformer";
                break;

            case GameMode.fighting:
                modeStr = "Fighting";
                break;

            case GameMode.rpg:
                modeStr = "RPG";
                break;

            case GameMode.fps:
                modeStr = "FPS";
                break;

            case GameMode.rhythm:
                modeStr = "Rhythm";
                break;

            case GameMode.dating:
                modeStr = "Dating";
                break;
        }

        Lock(modeStr);
    }

    public void Lock(string mode)
    {
        bool found = false;
        for (int i = 0; i < modes.Length && !found; i++)
        {
            if (modes[i].name.Equals(mode))
            {
                modes[i].unlocked = false;
                unlockPanel.SetActive(true);
                if (mode.Equals("Fighting") || mode.Equals("Rhythm") || mode.Equals("Racing"))
                {
                    mode += " Game";
                }
                else if (mode.Equals("Dating"))
                {
                    mode += " Sim";
                    dated = false;
                }
                unlockText.text = "You lost \n" + mode + " mode!";
                found = true;
                AkSoundEngine.PostEvent("sfx_game_mode_unlocked", gameObject);
                StartCoroutine(UnlockFade());
            }
        }

        numUnlocked--;

        if (numUnlocked == 1)
        {
            ToggleSwitchPanel(false);
        }

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
        if (dying)
        {
            return;
        }

        // Uncomment the below lines when the death jingle isn't deafening
        //AkSoundEngine.PostEvent("Death_Jingle_MuteMusic", gameObject);
        //AkSoundEngine.PostEvent("Death_Jingle", gameObject);

        dying = true;
        paused = true;
        StartCoroutine(ReloadLevel(false));
        currHP = maxHP;
        currMP = maxMP;
        currSP = maxSP;
    }

    public void Die(bool fall)
    {
        if (dying)
        {
            return;
        }

        // TODO: Post Death jingle event
        dying = true;
        paused = true;
        StartCoroutine(ReloadLevel(fall));
        currHP = maxHP;
        currMP = maxMP;
        currSP = maxSP;
    }

    private IEnumerator ReloadLevel(bool fall)
    {
        if (loading)
        {
            yield return null;
        }
        else
        {
            loading = true;
            if (fall)
            {
                levelFadeTime = 2;
            }
            StartCoroutine(LevelFade(false));
            SaveManager.singleton.InitGameController();
            yield return new WaitForSeconds(levelFadeTime);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            yield return new WaitForEndOfFrame();
            SwitchMode(GameMode.platformer, true, true);
            if (numUnlocked > 1)
            {
                ToggleSwitchPanel(true);
            }
            
            dying = false;
            StartCoroutine(LevelFade(true));
            paused = false;
            battling = false;
            loading = false;
        }
    }

    private IEnumerator UnlockFade()
    {
        Image img = unlockPanel.GetComponent<Image>();
        Color temp = img.color;
        Color temp2 = unlockText.color;

        temp.a = temp2.a = 1;
        img.color = temp;
        unlockText.color = temp2;

        yield return new WaitForSeconds(unlockWaitTime);

        while (img.color.a > 0)
        {
            temp.a -= inverseUnlockFadeTime * Time.deltaTime;
            temp2.a = temp.a;
            img.color = temp;
            unlockText.color = temp2;
            yield return new WaitForEndOfFrame();
        }

        unlockPanel.SetActive(false);
        temp.a = temp2.a = 1;
        img.color = temp;
        unlockText.color = temp;
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
            paused = true;
            PFController con = GameObject.Find("Player").GetComponent<PFController>();
            con.StartCoroutine(con.Die(true));
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

    public void UseSkill(int cost)
    {
        currSP = Mathf.Clamp(currSP - cost, 0, maxSP);
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

    public int GetHP()
    {
        return currHP;
    }

    public int GetMP()
    {
        return currMP;
    }

    public int GetSP()
    {
        return currSP;
    }

    public IEnumerator FadeAndLoad(int buildIndex)
    {
        if (loading)
        {
            yield return null;
        }
        else
        {
            loading = true;
            StartCoroutine(LevelFade(false));
            yield return new WaitForSeconds(levelFadeTime);
            SceneManager.LoadScene(buildIndex);
            yield return new WaitForEndOfFrame();
            SwitchMode(GameMode.platformer, true, true);

            if (numUnlocked > 1)
            {
                ToggleSwitchPanel(true);
            }

            StartCoroutine(LevelFade(true));
            paused = false;
            loading = false;

            if (numUnlocked > 1)
            {
                ToggleSwitchPanel(true);
            }

            yield return new WaitForSeconds(0.1f);
            AudioClip clip = GameObject.Find("Song").GetComponent<AudioSource>().clip;
            if (clip != rpgCombatBGM && clip != datingSimBGM)
            {
                currentBGM = clip;
            }
        }
    }

    public IEnumerator Battle()
    {
        battling = true;
        ToggleSwitchPanel(false);
        StartCoroutine(LevelFade(false));
        AkSoundEngine.PostEvent("RPG_Battle", gameObject);
        yield return new WaitForSeconds(levelFadeTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1, LoadSceneMode.Additive);
        DialogueManager.singleton.EndDialogue(false);
        yield return new WaitForEndOfFrame();

        AudioSource source = GameObject.Find("Song").GetComponent<AudioSource>();
        source.clip = rpgCombatBGM;
        source.Play();

        GameObject attackBtn = null;
        while (attackBtn == null)
        {
            attackBtn = GameObject.Find("AttackButton");
            yield return new WaitForEndOfFrame();
        }
        EventSystem.current.SetSelectedGameObject(attackBtn);
        StartCoroutine(LevelFade(true));
    }

    public IEnumerator DateMap()
    {
        StartCoroutine(LevelFade(false));
        yield return new WaitForSeconds(levelFadeTime);
        DialogueManager.singleton.EndDialogue(false);
        SceneManager.LoadScene(12, LoadSceneMode.Additive);
        yield return new WaitForEndOfFrame();
        GameObject caButton = null;
        while (caButton == null)
        {
            caButton = GameObject.Find("CA Button");
            yield return new WaitForEndOfFrame();
        }
        EventSystem.current.SetSelectedGameObject(caButton);
        StartCoroutine(LevelFade(true));
    }

    public IEnumerator UnloadBattle()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(LevelFade(false));
        AkSoundEngine.PostEvent((uint)GameObject.Find("AudioPlayer").GetComponent<AkAmbient>().eventID, gameObject);
        yield return new WaitForSeconds(levelFadeTime);

        int temp = SceneManager.GetActiveScene().buildIndex;

        if (temp % 2 == 0)
        {
            SceneManager.UnloadSceneAsync(temp);
        }
        else
        {
            SceneManager.UnloadSceneAsync(temp+1);
        }

        AudioSource source = GameObject.Find("Song").GetComponent<AudioSource>();
        source.clip = currentBGM;
        source.Play();

        yield return new WaitForEndOfFrame();
        StartCoroutine(LevelFade(true));
        GameObject.Find("Player").GetComponent<RPGController>().SetEncountering(false);
        ToggleSwitchPanel(true);
        paused = false;
        battling = false;
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

    public void SetStrength(int str)
    {
        strength = str;
    }

    public int GetMagic()
    {
        return magic;
    }
    
    public void SetMagic(int mag)
    {
        magic = mag;
    }

    public int GetNumUnlocked()
    {
        return numUnlocked;
    }

    public void SetNumUnlocked(int num)
    {
        numUnlocked = num;
    }

    public float GetGScale()
    {
        return gScale;
    }

    public void ReturnToMenu()
    {
        paused = false;
        Time.timeScale = 1;
        StartCoroutine(FadeAndLoad(0));
        onMenu = true;
        ResetUnlocks();
        ToggleSwitchPanel(false);
        Unpause();
    }

    private IEnumerator Switch()
    {
        bool success = false;
        while (!success)
        {
            try
            {
                SwitchMode(equipped);
                success = true;
            }
            catch
            {
                success = false;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void ToggleSwitchPanel (bool value)
    {
        Image[] images = GetComponentsInChildren<Image>(true);

        foreach (Image image in images)
        {
            if (image.name.Equals("SwitchPanel"))
            {
                image.gameObject.SetActive(value);
            }
        }

        if (value)
        {
            UpdateSwitchPanel();
        }
    }

    private void ResetUnlocks()
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

        numUnlocked = 1;
    }

    private void Pause()
    {
        if (equipped == GameMode.rhythm)
        {
            FindObjectOfType<StaffController>().source.Pause();
        }
        paused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        Button[] buttons = pauseMenu.GetComponentsInChildren<Button>();
        bool found = false;
        for (int i = 0; i < buttons.Length && !found; i++)
        {
            if (buttons[i].name.Equals("ResumeButton"))
            {
                EventSystem.current.SetSelectedGameObject(buttons[i].gameObject);
                found = true;
            }
        }

        Cursor.lockState = CursorLockMode.None;
    }

    public void Unpause()
    {
        if (unpausing)
        {
            return;
        }

        if (equipped == GameMode.rhythm)
        {
            StartCoroutine(RhythmUnpause());
        }
        else
        {
            paused = false;
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            CloseQuitMenu();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettingsMenu()
    {
        settingsPanel.SetActive(true);

        Button[] buttons = pauseMenu.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        SettingsController.singleton.ShowGameplaySettings();
    }

    public void CloseSettingsMenu()
    {
        settingsPanel.SetActive(false);

        SettingsController.singleton.Revert();

        Button[] buttons = pauseMenu.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.interactable = true;
            if (button.name.Equals("ResumeButton"))
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
        }
    }

    public void OpenQuitMenu()
    {
        quitPanel.SetActive(true);

        Button[] buttons = pauseMenu.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        buttons = quitPanel.GetComponentsInChildren<Button>();
        bool found = false;
        for (int i = 0; i < buttons.Length && !found; i++)
        {
            if (buttons[i].name.Equals("MenuButton"))
            {
                EventSystem.current.SetSelectedGameObject(buttons[i].gameObject);
                found = true;
            }
        }
    }

    public void CloseQuitMenu()
    {
        quitPanel.SetActive(false);

        Button[] buttons = pauseMenu.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.interactable = true;
            if (button.name.Equals("ResumeButton"))
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
        }
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    private IEnumerator RhythmUnpause()
    {
        unpausing = true;
        pauseMenu.SetActive(false);
        rhythmUnpausePanel.SetActive(true);
        TextMeshProUGUI text = rhythmUnpausePanel.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "3";
        yield return new WaitForSecondsRealtime(1);
        text.text = "2";
        yield return new WaitForSecondsRealtime(1);
        text.text = "1";
        yield return new WaitForSecondsRealtime(1);

        FindObjectOfType<StaffController>().source.UnPause();
        Time.timeScale = 1;
        paused = false;
        rhythmUnpausePanel.SetActive(false);
        unpausing = false;
    }

    private void UpdateSwitchPanel()
    {
        Image[] images = GetComponentsInChildren<Image>();

        foreach (Image image in images)
        {
            if (image.name.Equals("Main"))
            {
                image.sprite = modes[modeInt].sprite;
            }

            if (image.name.Equals("LB"))
            {
                bool found = false;

                if (modeInt > 0)
                {
                    for (int i = modeInt - 1; i >= 0 && !found; i--)
                    {
                        if (modes[i].unlocked)
                        {
                            image.sprite = modes[i].sprite;
                            found = true;
                        }
                    }
                }

                for (int i = modes.Length - 1; i > modeInt && !found; i--)
                {
                    if (modes[i].unlocked)
                    {
                        image.sprite = modes[i].sprite;
                        found = true;
                    }
                }
            }

            if (image.name.Equals("RB"))
            {
                bool found = false;

                if (modeInt < modes.Length - 1)
                {
                    for (int i = modeInt + 1; i < modes.Length && !found; i++)
                    {
                        if (modes[i].unlocked)
                        {
                            image.sprite = modes[i].sprite;
                            found = true;
                        }
                    }
                }

                for (int i = 0; i < modeInt && !found; i++)
                {
                    if (modes[i].unlocked)
                    {
                        image.sprite = modes[i].sprite;
                        found = true;
                    }
                }
            }
        }
    }

    public void SetGlitching (bool val)
    {
        glitching = val;
        gCanv.SetActive(val);
    }

    public void AddGold(int amt)
    {
        gold += amt;
    }

    public int GetGold()
    {
        return gold;
    }

    public void SetGold(int val)
    {
        gold = val;
    }
}