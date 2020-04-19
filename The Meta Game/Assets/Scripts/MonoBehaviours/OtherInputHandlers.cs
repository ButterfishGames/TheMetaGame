using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OtherInputHandlers : MonoBehaviour
{
    public void OnControlsChangedHandle(PlayerInput pIn)
    {
        if (DialogueManager.singleton != null)
        {
            DialogueManager.singleton.OnControlsChange(pIn);
        }
        
        if (SettingsController.singleton != null)
        {
            SettingsController.singleton.OnControlsChanged(pIn);
        }

        FPSController fpsCon = FindObjectOfType<FPSController>();
        if (fpsCon != null)
        {
            fpsCon.OnControlsChange(pIn);
        }

        SwitchPanelUpdater spu = FindObjectOfType<SwitchPanelUpdater>();
        if (spu != null)
        {
            spu.OnControlsChanged(pIn);
        }
    }

    private void OnMenu(InputValue value)
    {
        if (GameController.singleton != null)
        {
            GameController.singleton.OnMenu(value);
        }
    }

    private void OnCancel(InputValue value)
    {
        BattleController bCon = FindObjectOfType<BattleController>();
        if (bCon != null)
        {
            bCon.OnCancel(value);
        }

        if (GameController.singleton != null)
        {
            GameController.singleton.OnCancel(value);
        }
    }

    private void OnPause(InputValue value)
    {
        if (GameController.singleton != null)
        {
            GameController.singleton.OnPause(value);
        }
    }

    private void OnSwitchMode(InputValue value)
    {
        if (GameController.singleton != null)
        {
            GameController.singleton.OnSwitch(value);
        }
    }
}
