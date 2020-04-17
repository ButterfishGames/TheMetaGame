using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SwitchPanelUpdater : MonoBehaviour
{
    public GameObject keyboardButtons, xboxButtons, dualshockButtons, switchButtons;
    public Image lbImg, rbImg, l1Img, r1Img, lImg, rImg;
    public TextMeshProUGUI qText, eText;

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitUntil(() => GameObject.Find("Player") != null);
        OnControlsChanged(GameObject.Find("Player").GetComponent<PlayerInput>());
    }

    public void OnControlsChanged(PlayerInput pIn)
    {
        StartCoroutine(Updater(pIn));
    }

    private IEnumerator Updater(PlayerInput pIn)
    {
        yield return new WaitForEndOfFrame();
        switch (pIn.currentControlScheme)
        {
            case "KeyboardAndMouse":
                keyboardButtons.SetActive(true);
                xboxButtons.SetActive(false);
                dualshockButtons.SetActive(false);
                switchButtons.SetActive(false);

                qText.text = ButtonsLib.singleton.DialogueAction("SwitchModeNeg");
                eText.text = ButtonsLib.singleton.DialogueAction("SwitchModePos");
                break;

            case "DualShock":
                dualshockButtons.SetActive(true);
                keyboardButtons.SetActive(false);
                xboxButtons.SetActive(false);
                switchButtons.SetActive(false);

                l1Img.sprite = ButtonsLib.singleton.GetSprite("SwitchModeNeg", "DualShock");
                r1Img.sprite = ButtonsLib.singleton.GetSprite("SwitchModePos", "DualShock");
                break;

            case "Switch":
                switchButtons.SetActive(true);
                keyboardButtons.SetActive(false);
                xboxButtons.SetActive(false);
                dualshockButtons.SetActive(false);

                lImg.sprite = ButtonsLib.singleton.GetSprite("SwitchModeNeg", "Switch");
                rImg.sprite = ButtonsLib.singleton.GetSprite("SwitchModePos", "Switch");
                break;

            default:
                xboxButtons.SetActive(true);
                keyboardButtons.SetActive(false);
                dualshockButtons.SetActive(false);
                switchButtons.SetActive(false);

                lbImg.sprite = ButtonsLib.singleton.GetSprite("SwitchModeNeg", "Xbox");
                rbImg.sprite = ButtonsLib.singleton.GetSprite("SwitchModePos", "Xbox");
                break;
        }
    }
}
