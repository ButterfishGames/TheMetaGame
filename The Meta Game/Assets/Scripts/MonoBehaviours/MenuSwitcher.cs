using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    public static MenuSwitcher active;
    public MenuSwitcher other;
    public Cutscene transition;
    public bool startActive;

    private Transform player;

    private void Start()
    {
        if (startActive)
        {
            active = this;
        }

        player = GameObject.Find("Player").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active != this)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            CutsceneManager.singleton.StartScene(transition, false);
            SaveManager.singleton.LoadGame(false);
            active = other;
        }
    }
}
