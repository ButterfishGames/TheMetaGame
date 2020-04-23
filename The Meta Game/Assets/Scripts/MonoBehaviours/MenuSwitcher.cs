using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    public static MenuSwitcher active;
    public MenuSwitcher other;
    public Cutscene transition;
    public bool startActive;

    private void Start()
    {
        if (startActive)
        {
            active = this;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active != this)
        {
            return;
        }

        CutsceneManager.singleton.StartScene(transition);
        SaveManager.singleton.LoadGame(false);
        active = other;
    }
}
