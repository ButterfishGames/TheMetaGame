using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    /// <summary>
    /// A static variable storing a reference to the DialogueManager singleton
    /// </summary>
    public static DialogueManager singleton;

    /// <summary>
    /// Object reference to the dialogue box game object
    /// </summary>
    private GameObject dialogueBox;

    /// <summary>
    /// Object reference to the text component which will contain dialogue
    /// </summary>
    private TextMeshProUGUI dialogueText;

    /// <summary>
    /// Queue storing the lines of the currently active dialogue
    /// </summary>
    private Queue<string> lines;

    /// <summary>
    /// Bool representing whether a dialogue is currently being displayed or not
    /// </summary>
    private bool displaying;

    /// <summary>
    /// Bool used to ensure proper execution order when displaying dialogue
    /// </summary>
    private bool primed;

    // Start is called before the first frame update
    void Start()
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

        RectTransform[] rects = GetComponentsInChildren<RectTransform>(true);

        foreach (RectTransform rect in rects)
        {
            if (rect.name.Equals("DialogueBox"))
            {
                dialogueBox = rect.gameObject;
            }
        }

        dialogueText = dialogueBox.GetComponentInChildren<TextMeshProUGUI>(true);

        lines = new Queue<string>();
        displaying = false;
        primed = false;
    }

    // LateUpdate is called once per frame after all Update methods have executed
    void LateUpdate()
    {
        if (displaying && Input.GetButtonDown("Submit"))
        {
            primed = true;
        }

        if (primed && Input.GetButtonUp("Submit"))
        {
            DisplayNextLine();
            primed = false;
        }
    }

    public void StartDialogue(string[] lns)
    {
        lines.Clear();

        foreach(string line in lns)
        {
            lines.Enqueue(line);
        }

        dialogueBox.SetActive(true);

        displaying = true;
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (!displaying)
        {
            return;
        }
        else if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        string line = lines.Dequeue();

        dialogueText.text = line;
    }

    private void EndDialogue()
    {
        dialogueBox.SetActive(false);
        displaying = false;
    }

    public bool GetDisplaying()
    {
        return displaying;
    }
}
