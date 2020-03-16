using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Goal : MonoBehaviour
{
    /// <summary>
    /// A static variable storing a reference to the Goal singleton
    /// </summary>
    public static Goal singleton;

    [Tooltip("Object reference for the game object which contains the win screen UI")]
    public GameObject winPanel;

    [Tooltip("Object reference for text which will display the total playtime")]
    public TextMeshProUGUI timeText;

    [Tooltip("Object reference for quit button")]
    public GameObject quitButton;

    /// <summary>
    /// Holds total playtime
    /// </summary>
    private float timer;

    // Start is called before the first frame update
    private void Start()
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

        timer = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Win();
        }
    }

    private void Win()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        string timeStr = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        winPanel.SetActive(true);
        timeText.text = timeStr;
        EventSystem.current.SetSelectedGameObject(quitButton);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }
}
