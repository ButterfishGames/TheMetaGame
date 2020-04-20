using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteController : MonoBehaviour
{
    public Sprite[] arrowSprites, xboxSprites, ps4Sprites, switchSprites;

    private int dir;

    private RhythmController rCon;
    private StaffController sCon;

    private void Start()
    {
        rCon = FindObjectOfType<RhythmController>();
        sCon = GetComponentInParent<StaffController>();
    }

    public void OnControlsChanged(PlayerInput pIn)
    {
        if (SettingsController.singleton.dInput)
        {
            GetComponent<SpriteRenderer>().sprite = arrowSprites[dir];
        }
        else
        {
            switch (pIn.currentControlScheme)
            {
                case "KeyboardAndMouse":
                    GetComponent<SpriteRenderer>().sprite = arrowSprites[dir];
                    break;

                case "DualShock":
                    GetComponent<SpriteRenderer>().sprite = ps4Sprites[dir];
                    break;

                case "Switch":
                    GetComponent<SpriteRenderer>().sprite = switchSprites[dir];
                    break;

                default:
                    GetComponent<SpriteRenderer>().sprite = xboxSprites[dir];
                    break;
            }
        }
    }

    private void LateUpdate()
    {
        if (rCon.transform.position.x > transform.position.x + 1)
        {
            rCon.Miss(this);
            Destroy(gameObject);
        }
    }

    public void SetDir(int d)
    {
        if (d < 0 || d > 3)
        {
            return;
        }

        GetComponent<SpriteRenderer>().sprite = arrowSprites[d];
        dir = d;
        
        OnControlsChanged(FindObjectOfType<PlayerInput>());
    }

    public int GetDir()
    {
        return dir;
    }
}
