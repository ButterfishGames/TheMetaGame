using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SwitchPanelUpdater : MonoBehaviour
{
    public GameObject keyboardButtons, xboxButtons, dualshockButtons, switchButtons;
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
                break;

            case "Switch":
                switchButtons.SetActive(true);
                keyboardButtons.SetActive(false);
                xboxButtons.SetActive(false);
                dualshockButtons.SetActive(false);
                break;

            default:
                xboxButtons.SetActive(true);
                keyboardButtons.SetActive(false);
                dualshockButtons.SetActive(false);
                switchButtons.SetActive(false);
                break;
        }
    }
}
