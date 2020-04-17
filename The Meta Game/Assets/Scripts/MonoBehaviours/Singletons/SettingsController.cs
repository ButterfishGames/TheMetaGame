using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private enum Panel
    {
        gameplay,
        display,
        audio,
        controls
    }

    private Panel currentPanel;

    private Image panelImg;

    private void Awake()
    {
        panelImg = GetComponent<Image>();

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
        //controlPanel.SetActive(false);

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
        //controlPanel.SetActive(false);

        audioPanel.SetActive(true);
        currentPanel = Panel.audio;
        panelImg.sprite = audioSprite;
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
                musicVolumeSlider.value = musicVolume;
                sfxVolumeSlider.value = sfxVolume;

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
        }
    }
}
