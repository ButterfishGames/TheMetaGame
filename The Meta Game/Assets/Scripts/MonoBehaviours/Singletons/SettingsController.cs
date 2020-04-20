using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    public static SettingsController singleton;

    [Header("Settings Panels")]
    public GameObject gameplayPanel;
    public GameObject displayPanel, audioPanel, controlPanel;

    [Header("Settings Panel Sprites")]
    public Sprite gameplaySprite;
    public Sprite displaySprite, audioSprite, controlSprite;

    // Gameplay Setting Vars
    [HideInInspector] public bool pfUpJump = true;
    [HideInInspector] public bool fgUpJump = true;
    [HideInInspector] public bool dInput = false;
    [HideInInspector] public bool invertX = false;
    [HideInInspector] public bool invertY = false;
    [HideInInspector] public float sensitivity = 20;

    [Header("Gameplay Settings UI")]
    public Toggle pfujToggle;
    public Toggle fgujToggle;
    public Toggle dInputToggle;
    public Toggle invertXToggle;
    public Toggle invertYToggle;
    public Slider sensitivitySlider;

    [Header("Gameplay Content Holders")]
    public GameObject pfLabel;
    public GameObject pfUpJumpHolder, fgLabel, fgUpJumpHolder, rhythmLabel, dInputHolder, fpsLabel,
        invertXHolder, invertYHolder, sensitivityHolder;

    // Audio Setting Vars
    [HideInInspector] public float musicVolume = 0.5f;
    [HideInInspector] public float sfxVolume = 0.6f;

    [Header("Audio Settings UI")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    // Control Settings Vars
    private List<string> tempKeys;
    private List<string> tempActions;
    private List<string> tempPaths;
    private List<int> bindingInds;

    [Header("Control Settings UI")]
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI pauseText, menuText, switchModeLText, switchModeRText, lightText, light2Text, mediumText,
        medium2Text, heavyText, heavy2Text, fireText, zoomText;
    
    [Header("Control Content Holders")]
    public GameObject menuHolder;
    public GameObject switchLHolder, switchRHolder, fgControlLabel, fgLightHolder, fgMediumHolder, fgHeavyHolder, 
        fpsControlLabel, fpsFireHolder, fpsZoomHolder;

    [Header("Context-Dependent Buttons")]
    public GameObject lightButton2;
    public GameObject mediumButton2, heavyButton2;

    private string[] actions = { "Jump", "Pause", "Menu", "SwitchModeNeg", "SwitchModePos",
        "Light", "Medium", "Heavy", "Fire", "Zoom" };

    private enum Panel
    {
        gameplay,
        display,
        audio,
        controls
    }

    private Panel currentPanel;

    private Image panelImg;

    private PlayerInput pInput;
    private TMP_SpriteAsset current;

    private void Awake()
    {
        pInput = FindObjectOfType<PlayerInput>();

        panelImg = GetComponent<Image>();

        #region Gameplay Settings
        // Gameplay Settings setup
        if (PlayerPrefs.HasKey("pfUpJump"))
        {
            pfujToggle.isOn = bool.Parse(PlayerPrefs.GetString("pfUpJump"));
            pfUpJump = pfujToggle.isOn;
        }
        else
        {
            pfujToggle.isOn = true;
            PlayerPrefs.SetString("pfUpJump", "true");
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey("fgUpJump"))
        {
            fgujToggle.isOn = bool.Parse(PlayerPrefs.GetString("fgUpJump"));
            fgUpJump = fgujToggle.isOn;
        }
        else
        {
            fgujToggle.isOn = true;
            PlayerPrefs.SetString("fgUpJump", "true");
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey("dInput"))
        {
            dInputToggle.isOn = bool.Parse(PlayerPrefs.GetString("dInput"));
            dInput = dInputToggle.isOn;
        }
        else
        {
            dInputToggle.isOn = false;
            PlayerPrefs.SetString("dInput", "false");
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey("invertX"))
        {
            invertXToggle.isOn = bool.Parse(PlayerPrefs.GetString("invertX"));
            invertX = invertXToggle.isOn;
        }
        else
        {
            invertXToggle.isOn = false;
            PlayerPrefs.SetString("invertX", "false");
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey("invertY"))
        {
            invertYToggle.isOn = bool.Parse(PlayerPrefs.GetString("invertY"));
            invertY = invertYToggle.isOn;
        }
        else
        {
            invertYToggle.isOn = false;
            PlayerPrefs.SetString("invertY", "false");
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey("sensitivity"))
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity");
            sensitivity = sensitivitySlider.value;
        }
        else
        {
            sensitivitySlider.value = 20;
            PlayerPrefs.SetFloat("sensitivity", 20);
            PlayerPrefs.Save();
        }
        #endregion
        #region Audio Settings
        // Audio Settings setup
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
            musicVolume = musicVolumeSlider.value;
        }
        else
        {
            musicVolumeSlider.value = 0.5f;
            PlayerPrefs.SetFloat("musicVolume", 0.5f);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume");
            sfxVolume = sfxVolumeSlider.value;
        }
        else
        {
            sfxVolumeSlider.value = 0.6f;
            PlayerPrefs.SetFloat("sfxVolume", 0.6f);
            PlayerPrefs.Save();
        }

        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (source.gameObject.name.Equals("Player") 
                || source.gameObject.name.Equals("Song") 
                || source.gameObject.name.Equals("MusicalStaff(Clone)"))
            {
                source.volume = musicVolume;
            }
            else
            {
                source.volume = sfxVolume;
            }
        }
        #endregion
        #region Control Settings
        foreach (string action in actions)
        {
            Setup(action);
        }

        tempActions = new List<string>();
        tempPaths = new List<string>();
        tempKeys = new List<string>();
        bindingInds = new List<int>();
        #endregion

        OnControlsChanged(pInput);
    }

    public void OnControlsChanged(PlayerInput pIn)
    {
        if (DialogueManager.singleton != null)
        {
            switch (pIn.currentControlScheme)
            {
                case "KeyboardAndMouse":
                    current = DialogueManager.singleton.keyboardAndMouse;
                    break;

                case "DualShock":
                    current = DialogueManager.singleton.dualshock;
                    break;

                case "Switch":
                    current = DialogueManager.singleton.switchPro;
                    break;

                default:
                    current = DialogueManager.singleton.xbox;
                    break;
            }
        }
        else
        {
            current = zoomText.spriteAsset;
        }

        if (controlPanel.activeInHierarchy)
        {
            InitControlText(pIn);
        }
    }

    private void Update()
    {
        TempApply();
    }

    public void ShowGameplaySettings()
    {
        if (gameplayPanel.activeInHierarchy)
        {
            return;
        }

        Apply();

        //displayPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlPanel.SetActive(false);

        gameplayPanel.SetActive(true);
        currentPanel = Panel.gameplay;
        panelImg.sprite = gameplaySprite;

        if (GameController.singleton.modes[0].unlocked)
        {
            pfLabel.SetActive(true);
            pfUpJumpHolder.SetActive(true);
            if (PlayerPrefs.HasKey("pfUpJump"))
            {
                pfujToggle.isOn = bool.Parse(PlayerPrefs.GetString("pfUpJump"));
                pfUpJump = pfujToggle.isOn;
            }
            else
            {
                pfujToggle.isOn = true;
                PlayerPrefs.SetString("pfUpJump", "true");
                PlayerPrefs.Save();
            }
        }
        else
        {
            pfLabel.SetActive(false);
            pfUpJumpHolder.SetActive(false);
        }

        if (GameController.singleton.modes[1].unlocked)
        {
            fgLabel.SetActive(true);
            fgUpJumpHolder.SetActive(true);
            if (PlayerPrefs.HasKey("fgUpJump"))
            {
                fgujToggle.isOn = bool.Parse(PlayerPrefs.GetString("fgUpJump"));
                fgUpJump = fgujToggle.isOn;
            }
            else
            {
                fgujToggle.isOn = true;
                PlayerPrefs.SetString("fgUpJump", "true");
                PlayerPrefs.Save();
            }
        }
        else
        {
            fgLabel.SetActive(false);
            fgUpJumpHolder.SetActive(false);
        }

        if (GameController.singleton.modes[3].unlocked)
        {
            rhythmLabel.SetActive(true);
            dInputHolder.SetActive(true);
            if (PlayerPrefs.HasKey("dInput"))
            {
                dInputToggle.isOn = bool.Parse(PlayerPrefs.GetString("dInput"));
                dInput = dInputToggle.isOn;
            }
            else
            {
                dInputToggle.isOn = false;
                PlayerPrefs.SetString("dInput", "false");
                PlayerPrefs.Save();
            }
        }
        else
        {
            rhythmLabel.SetActive(false);
            dInputHolder.SetActive(false);
        }

        if (GameController.singleton.modes[6].unlocked)
        {
            fpsLabel.SetActive(true);
            invertXHolder.SetActive(true);
            if (PlayerPrefs.HasKey("invertX"))
            {
                invertXToggle.isOn = bool.Parse(PlayerPrefs.GetString("invertX"));
                invertX = invertXToggle.isOn;
            }
            else
            {
                invertXToggle.isOn = false;
                PlayerPrefs.SetString("invertX", "false");
                PlayerPrefs.Save();
            }

            invertYHolder.SetActive(true);
            if (PlayerPrefs.HasKey("invertY"))
            {
                invertYToggle.isOn = bool.Parse(PlayerPrefs.GetString("invertY"));
                invertY = invertYToggle.isOn;
            }
            else
            {
                invertYToggle.isOn = false;
                PlayerPrefs.SetString("invertY", "false");
                PlayerPrefs.Save();
            }

            sensitivityHolder.SetActive(true);
            if (PlayerPrefs.HasKey("sensitivity"))
            {
                sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity");
                sensitivity = sensitivitySlider.value;
            }
            else
            {
                sensitivitySlider.value = 20;
                PlayerPrefs.SetFloat("sensitivity", 20);
                PlayerPrefs.Save();
            }
        }
        else
        {
            fpsLabel.SetActive(false);
            invertXHolder.SetActive(false);
            invertYHolder.SetActive(false);
            sensitivityHolder.SetActive(false);
        }
    }

    public void ShowAudioSettings()
    {
        if (audioPanel.activeInHierarchy)
        {
            return;
        }

        Apply();

        gameplayPanel.SetActive(false);
        //displayPanel.SetActive(false);
        controlPanel.SetActive(false);

        audioPanel.SetActive(true);
        currentPanel = Panel.audio;
        panelImg.sprite = audioSprite;
    }

    public void ShowControlSettings()
    {
        if (controlPanel.activeInHierarchy)
        {
            return;
        }

        Apply();

        gameplayPanel.SetActive(false);
        //displayPanel.SetActive(false);
        audioPanel.SetActive(false);

        controlPanel.SetActive(true);
        currentPanel = Panel.controls;
        panelImg.sprite = controlSprite;

        if (GameController.singleton.GetNumUnlocked() > 1)
        {
            menuHolder.SetActive(true);
            switchLHolder.SetActive(true);
            switchRHolder.SetActive(true);
        }
        else
        {
            menuHolder.SetActive(false);
            switchLHolder.SetActive(false);
            switchRHolder.SetActive(false);
        }

        if (GameController.singleton.modes[1].unlocked)
        {
            fgControlLabel.SetActive(true);
            fgLightHolder.SetActive(true);
            fgMediumHolder.SetActive(true);
            fgHeavyHolder.SetActive(true);
        }
        else
        {
            fgControlLabel.SetActive(false);
            fgLightHolder.SetActive(false);
            fgMediumHolder.SetActive(false);
            fgHeavyHolder.SetActive(false);
        }

        if (GameController.singleton.modes[6].unlocked)
        {
            fpsControlLabel.SetActive(true);
            fpsZoomHolder.SetActive(true);
            fpsFireHolder.SetActive(true);
        }
        else
        {
            fpsControlLabel.SetActive(false);
            fpsZoomHolder.SetActive(false);
            fpsFireHolder.SetActive(false);
        }

        InitControlText(pInput);
    }

    public void Apply()
    {
        switch (currentPanel)
        {
            case Panel.gameplay:
                pfUpJump = pfujToggle.isOn;
                fgUpJump = fgujToggle.isOn;
                dInput = dInputToggle.isOn;
                invertX = invertXToggle.isOn;
                invertY = invertYToggle.isOn;
                sensitivity = sensitivitySlider.value;

                PlayerPrefs.SetString("pfUpJump", pfUpJump.ToString());
                PlayerPrefs.SetString("fgUpJump", fgUpJump.ToString());
                PlayerPrefs.SetString("dInput", dInput.ToString());
                PlayerPrefs.SetString("invertX", invertX.ToString());
                PlayerPrefs.SetString("invertY", invertY.ToString());
                PlayerPrefs.SetFloat("sensitivity", sensitivity);

                PlayerPrefs.Save();
                break;

            case Panel.audio:
                musicVolume = musicVolumeSlider.value;
                sfxVolume = sfxVolumeSlider.value;

                PlayerPrefs.SetFloat("musicVolume", musicVolume);
                PlayerPrefs.SetFloat("sfxVolume", sfxVolume);

                PlayerPrefs.Save();

                AudioSource[] sources = FindObjectsOfType<AudioSource>();
                foreach (AudioSource source in sources)
                {
                    if (source.gameObject.name.Equals("Player")
                        || source.gameObject.name.Equals("Song")
                        || source.gameObject.name.Equals("MusicalStaff(Clone)"))
                    {
                        source.volume = musicVolume;
                    }
                    else
                    {
                        source.volume = sfxVolume;
                    }
                }
                break;

            case Panel.controls:
                for (int i = 0; i < tempPaths.Count; i++)
                {
                    pInput.actions[tempActions[i]].ApplyBindingOverride(bindingInds[i], tempPaths[i]);
                    PlayerPrefs.SetString(tempKeys[i], tempPaths[i]);
                }
                PlayerPrefs.Save();

                tempActions = new List<string>();
                tempKeys = new List<string>();
                tempPaths = new List<string>();
                bindingInds = new List<int>();
                break;
        }
    }

    public void TempApply()
    {
        switch (currentPanel)
        {
            case Panel.audio:
                musicVolume = musicVolumeSlider.value;
                sfxVolume = sfxVolumeSlider.value;

                AudioSource[] sources = FindObjectsOfType<AudioSource>();
                foreach (AudioSource source in sources)
                {
                    if (source.gameObject.name.Equals("Player")
                        || source.gameObject.name.Equals("Song")
                        || source.gameObject.name.Equals("MusicalStaff(Clone)"))
                    {
                        source.volume = musicVolume;
                    }
                    else
                    {
                        source.volume = sfxVolume;
                    }
                }
                break;
        }
    }

    public void Revert()
    {
        switch (currentPanel)
        {
            case Panel.gameplay:
                pfujToggle.isOn = pfUpJump;
                fgujToggle.isOn = fgUpJump;
                dInputToggle.isOn = dInput;
                invertXToggle.isOn = invertX;
                invertYToggle.isOn = invertY;
                sensitivitySlider.value = sensitivity;

                PlayerPrefs.SetString("pfUpJump", pfUpJump.ToString());
                PlayerPrefs.SetString("fgUpJump", fgUpJump.ToString());
                PlayerPrefs.SetString("dInput", dInput.ToString());
                PlayerPrefs.SetString("invertX", invertX.ToString());
                PlayerPrefs.SetString("invertY", invertY.ToString());
                PlayerPrefs.SetFloat("sensitivity", sensitivity);

                PlayerPrefs.Save();
                break;

            case Panel.audio:
                musicVolume = PlayerPrefs.GetFloat("musicVolume");
                sfxVolume = PlayerPrefs.GetFloat("sfxVolume");

                musicVolumeSlider.value = musicVolume;
                sfxVolumeSlider.value = sfxVolume;

                AudioSource[] sources = FindObjectsOfType<AudioSource>();
                foreach (AudioSource source in sources)
                {
                    if (source.gameObject.name.Equals("Player")
                        || source.gameObject.name.Equals("Song")
                        || source.gameObject.name.Equals("MusicalStaff(Clone)"))
                    {
                        source.volume = musicVolume;
                    }
                    else
                    {
                        source.volume = sfxVolume;
                    }
                }
                break;

            case Panel.controls:
                tempActions = new List<string>();
                tempKeys = new List<string>();
                tempPaths = new List<string>();
                bindingInds = new List<int>();

                foreach (string action in actions)
                {
                    Setup(action);
                }
                InitControlText(pInput);
                break;
        }
    }

    public void Default()
    {
        switch (currentPanel)
        {
            case Panel.gameplay:
                pfUpJump = true;
                fgUpJump = true;
                dInput = false;
                invertX = false;
                invertY = false;
                sensitivity = 20;

                pfujToggle.isOn = true;
                fgujToggle.isOn = true;
                dInputToggle.isOn = false;
                invertXToggle.isOn = false;
                invertYToggle.isOn = false;
                sensitivitySlider.value = 20;

                PlayerPrefs.SetString("pfUpJump", "true");
                PlayerPrefs.SetString("fgUpJump", "true");
                PlayerPrefs.SetString("dInput", "false");
                PlayerPrefs.SetString("invertX", "false");
                PlayerPrefs.SetString("invertY", "false");
                PlayerPrefs.SetFloat("sensitivity", 20);

                PlayerPrefs.Save();
                break;

            case Panel.audio:
                musicVolume = 0.5f;
                sfxVolume = 0.6f;

                musicVolumeSlider.value = 0.5f;
                sfxVolumeSlider.value = 0.6f;

                PlayerPrefs.SetFloat("musicVolume", 0.5f);
                PlayerPrefs.SetFloat("sfxVolume", 0.6f);

                PlayerPrefs.Save();
                
                AudioSource[] sources = FindObjectsOfType<AudioSource>();
                foreach (AudioSource source in sources)
                {
                    if (source.gameObject.name.Equals("Player")
                        || source.gameObject.name.Equals("Song")
                        || source.gameObject.name.Equals("MusicalStaff(Clone)"))
                    {
                        source.volume = musicVolume;
                    }
                    else
                    {
                        source.volume = sfxVolume;
                    }
                }
                break;

            case Panel.controls:
                foreach (string action in actions)
                {
                    ControlDefault(action);
                }
                PlayerPrefs.Save();
                break;
        }
    }

    public void StartMap(string action)
    {
        string key;
        PlayerInput pIn = FindObjectOfType<PlayerInput>();

        switch(pIn.currentControlScheme)
        {
            case "KeyboardAndMouse":
                key = "kam" + action;
                break;

            case "DualShock":
                key = "ds" + action;
                break;

            case "Switch":
                key = "switch" + action;
                break;

            default:
                key = "xbox" + action;
                break;
        }

        string modAction = action;

        if (action.Contains("SwitchMode"))
        {
            action = "SwitchMode";
        }
        else if (action.Contains("Light"))
        {
            action = "Light";
        }
        else if (action.Contains("Medium"))
        {
            action = "Medium";
        }
        else if (action.Contains("Heavy"))
        {
            action = "Heavy";
        }

        bool curr = pIn.actions[action].enabled;

        if (curr)
        {
            pIn.actions[action].Disable();
        }

        int ind = GetBindingIndex(pIn.actions[action], key);

        InputActionRebindingExtensions.RebindingOperation op = pIn.actions[action].PerformInteractiveRebinding(ind);
        op.WithBindingGroup(pIn.currentControlScheme);
        op.OnApplyBinding((operation, path) =>
        {
            tempPaths.Add(path);
            tempActions.Add(action);
            tempKeys.Add(key);
            bindingInds.Add(ind);

            if (curr)
            {
                pIn.actions[action].Enable();
            }

            switch (modAction)
            {
                case "Jump":
                    jumpText.spriteAsset = current;
                    jumpText.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "Pause":
                    pauseText.spriteAsset = current;
                    pauseText.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "Menu":
                    menuText.spriteAsset = current;
                    menuText.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "SwitchModeNeg":
                    switchModeLText.spriteAsset = current;
                    switchModeLText.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "SwitchModePos":
                    switchModeRText.spriteAsset = current;
                    switchModeRText.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "Light":
                    lightText.spriteAsset = current;
                    lightText.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "Light2":
                    light2Text.spriteAsset = current;
                    light2Text.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "Medium":
                    mediumText.spriteAsset = current;
                    mediumText.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "Medium2":
                    medium2Text.spriteAsset = current;
                    medium2Text.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "Heavy":
                    heavyText.spriteAsset = current;
                    heavyText.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "Heavy2":
                    heavy2Text.spriteAsset = current;
                    heavy2Text.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "Fire":
                    fireText.spriteAsset = current;
                    fireText.text = ButtonsLib.singleton.PathParse(path);
                    break;

                case "Zoom":
                    zoomText.spriteAsset = current;
                    zoomText.text = ButtonsLib.singleton.PathParse(path);
                    break;
            }
        });

        op.Start();
    }

    private void Setup(string action)
    {
        string modAction = action;
        if (action.Contains("SwitchMode"))
        {
            action = "SwitchMode";
        }

        if (PlayerPrefs.HasKey("kam" + modAction))
        {
            int index = -1;
            for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
            {
                if (pInput.actions[action].bindings[i].groups.Contains("KeyboardAndMouse")
                    && BindingApproved(modAction, pInput.actions[action].bindings[i]))
                {
                    index = i;
                }
            }
            pInput.actions[action].ApplyBindingOverride(index, PlayerPrefs.GetString("kam" + modAction));
        }
        else
        {
            int index = -1;
            for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
            {
                if (pInput.actions[action].bindings[i].groups.Contains("KeyboardAndMouse")
                    && BindingApproved(modAction, pInput.actions[action].bindings[i]))
                {
                    index = i;
                }
            }
            PlayerPrefs.SetString("kam" + modAction, pInput.actions[action].bindings[index].path);
            PlayerPrefs.Save();
        }

        if (action.Equals("Light") || action.Equals("Medium") || action.Equals("Heavy"))
        {
            modAction += 2;

            if (PlayerPrefs.HasKey("kam" + modAction))
            {
                int index = -1;
                for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
                {
                    if (pInput.actions[action].bindings[i].groups.Contains("KeyboardAndMouse")
                        && BindingApproved(modAction, pInput.actions[action].bindings[i]))
                    {
                        index = i;
                    }
                }
                pInput.actions[action].ApplyBindingOverride(index, PlayerPrefs.GetString("kam" + modAction));
            }
            else
            {
                int index = -1;
                for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
                {
                    if (pInput.actions[action].bindings[i].groups.Contains("KeyboardAndMouse")
                        && BindingApproved(modAction, pInput.actions[action].bindings[i]))
                    {
                        index = i;
                    }
                }
                PlayerPrefs.SetString("kam" + modAction, pInput.actions[action].bindings[index].path);
                PlayerPrefs.Save();
            }
        }

        if (PlayerPrefs.HasKey("ds" + modAction))
        {
            int index = -1;
            for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
            {
                if (pInput.actions[action].bindings[i].groups.Contains("DualShock"))
                {
                    index = i;
                }
            }
            pInput.actions[action].ApplyBindingOverride(index, PlayerPrefs.GetString("ds" + modAction));
        }
        else
        {
            int index = -1;
            for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
            {
                if (pInput.actions[action].bindings[i].groups.Contains("DualShock"))
                {
                    index = i;
                }
            }
            PlayerPrefs.SetString("ds" + modAction, pInput.actions[action].bindings[index].path);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey("switch" + modAction))
        {
            int index = -1;
            for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
            {
                if (pInput.actions[action].bindings[i].groups.Contains("Switch"))
                {
                    index = i;
                }
            }
            pInput.actions[action].ApplyBindingOverride(index, PlayerPrefs.GetString("switch" + modAction));
        }
        else
        {
            int index = -1;
            for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
            {
                if (pInput.actions[action].bindings[i].groups.Contains("Switch"))
                {
                    index = i;
                }
            }
            PlayerPrefs.SetString("switch" + modAction, pInput.actions[action].bindings[index].path);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey("xbox" + modAction))
        {
            int index = -1;
            for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
            {
                if (pInput.actions[action].bindings[i].groups.Contains("Xbox"))
                {
                    index = i;
                }
            }
            pInput.actions[action].ApplyBindingOverride(index, PlayerPrefs.GetString("xbox" + modAction));
        }
        else
        {
            int index = -1;
            for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
            {
                if (pInput.actions[action].bindings[i].groups.Contains("Xbox"))
                {
                    index = i;
                }
            }
            PlayerPrefs.SetString("xbox" + modAction, pInput.actions[action].bindings[index].path);
            PlayerPrefs.Save();
        }
    }

    private bool BindingApproved (string action, InputBinding binding)
    {
        if (action.Contains("SwitchMode"))
        {
            if (action.Equals("SwitchModePos") && binding.name.Equals("positive"))
            {
                return true;
            }
            else if (action.Equals("SwitchModeNeg") && binding.name.Equals("negative"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (action.Contains("Light"))
        {
            if (action.Equals("Light") && binding.path.Contains("/z"))
            {
                return true;
            }
            else if (action.Equals("Light2") && binding.path.Contains("/i"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (action.Contains("Medium"))
        {
            if (action.Equals("Medium") && binding.path.Contains("/x"))
            {
                return true;
            }
            else if (action.Equals("Medium2") && binding.path.Contains("/o"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (action.Contains("Heavy"))
        {
            if (action.Equals("Heavy") && binding.path.Contains("/c"))
            {
                return true;
            }
            else if (action.Equals("Heavy2") && binding.path.Contains("/p"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    private void InitControlText(PlayerInput pIn)
    {
        jumpText.spriteAsset = current;
        jumpText.text = ButtonsLib.singleton.DialogueSingleAction("Jump");

        pauseText.spriteAsset = current;
        pauseText.text = ButtonsLib.singleton.DialogueSingleAction("Pause");

        menuText.spriteAsset = current;
        menuText.text = ButtonsLib.singleton.DialogueSingleAction("Menu");

        switchModeLText.spriteAsset = current;
        switchModeLText.text = ButtonsLib.singleton.DialogueSingleAction("SwitchModeNeg");

        switchModeRText.spriteAsset = current;
        switchModeRText.text = ButtonsLib.singleton.DialogueSingleAction("SwitchModePos");

        if (GameController.singleton.modes[1].unlocked)
        {
            lightText.spriteAsset = current;
            lightText.text = ButtonsLib.singleton.DialogueSingleAction("Light");

            mediumText.spriteAsset = current;
            mediumText.text = ButtonsLib.singleton.DialogueSingleAction("Medium");

            heavyText.spriteAsset = current;
            heavyText.text = ButtonsLib.singleton.DialogueSingleAction("Heavy");

            if (pIn.currentControlScheme.Equals("KeyboardAndMouse"))
            {
                lightButton2.SetActive(true);
                light2Text.spriteAsset = current;
                light2Text.text = ButtonsLib.singleton.DialogueSingleAction("Light", 1);

                mediumButton2.SetActive(true);
                medium2Text.spriteAsset = current;
                medium2Text.text = ButtonsLib.singleton.DialogueSingleAction("Medium", 1);

                heavyButton2.SetActive(true);
                heavy2Text.spriteAsset = current;
                heavy2Text.text = ButtonsLib.singleton.DialogueSingleAction("Heavy", 1);
            }
            else
            {
                lightButton2.SetActive(false);
                mediumButton2.SetActive(false);
                heavyButton2.SetActive(false);
            }
        }

        if (GameController.singleton.modes[6].unlocked)
        {
            zoomText.spriteAsset = current;
            zoomText.text = ButtonsLib.singleton.DialogueSingleAction("Zoom");

            fireText.spriteAsset = current;
            fireText.text = ButtonsLib.singleton.DialogueSingleAction("Fire");
        }
    }

    private int GetBindingIndex(InputAction action, string key)
    {
        string scheme = "";
        int match = 1;

        if (key.Contains("kam"))
        {
            scheme = "KeyboardAndMouse";

            if (key.Contains("2"))
            {
                match = 2;
            }
        }
        else if (key.Contains("ds"))
        {
            scheme = "DualShock";
        }
        else if (key.Contains("switch"))
        {
            scheme = "Switch";
        }
        else if (key.Contains("xbox"))
        {
            scheme = "Xbox";
        }
        
        int count = 0;

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].groups.Contains(scheme) && 
                ((!key.Contains("Pos") && !key.Contains("Neg"))
                || (key.Contains("Pos") && action.bindings[i].name.Equals("positive"))
                || (key.Contains("Neg") && action.bindings[i].name.Equals("negative"))))
            {
                count++;

                if (count == match)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    private void ControlDefault(string action)
    {
        string modAction = action;
        if (action.Contains("SwitchMode"))
        {
            modAction = "SwitchMode";
        }

        pInput.actions[modAction].RemoveAllBindingOverrides();

        PlayerPrefs.DeleteKey("kam" + action);
        PlayerPrefs.DeleteKey("ds" + action);
        PlayerPrefs.DeleteKey("switch" + action);
        PlayerPrefs.DeleteKey("xbox" + action);

        if (action.Contains("Light") || action.Contains("Medium") || action.Contains("Heavy"))
        {
            action += 2;
            PlayerPrefs.DeleteKey("kam" + action);
        }
    }
}
