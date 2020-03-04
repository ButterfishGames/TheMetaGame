using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager singleton;

    private GameObject rpgDialogueBox;
    private GameObject datingDialogueBox;

    public GameObject branchButtonPrefab;

    private Image charImg;
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI nameText;

    private Queue<string> sentences;
    private Queue<string> names;
    private Queue<Sprite> sprites;

    private bool displaying;
    private Dialogue currentDialogue;

    private bool primed;
    private bool isBranched;

    private Controls controls;

    private int relationship;

    private void OnEnable()
    {
        controls = new Controls();

        controls.UI.Submit.started += SubmitStartHandle;
        controls.UI.Submit.canceled += SubmitCancHandle;

        controls.UI.Submit.Enable();
    }

    private void OnDisable()
    {
        controls.UI.Submit.started -= SubmitStartHandle;
        controls.UI.Submit.canceled -= SubmitCancHandle;

        controls.UI.Submit.Disable();
    }

    private void SubmitStartHandle(InputAction.CallbackContext context)
    {
        if (displaying)
        {
            primed = true;
        }
    }

    private void SubmitCancHandle(InputAction.CallbackContext context)
    {
        if (primed)
        {
            DisplayNextLine();
            primed = false;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        relationship = 0;

        if (singleton == null)
        {
            DontDestroyOnLoad(gameObject);
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }

        sentences = new Queue<string>();
        names = new Queue<string>();
        sprites = new Queue<Sprite>();

        RectTransform[] rects = GetComponentsInChildren<RectTransform>(true);
        foreach (RectTransform rect in rects)
        {
            if (rect.name.Equals("RPG DBox"))
            {
                rpgDialogueBox = rect.gameObject;
            }
            else if (rect.name.Equals("Dating DBox"))
            {
                datingDialogueBox = rect.gameObject;
            }
        }
        
        currentDialogue = null;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        StartDialogue(dialogue, false);
    }

    public void StartDialogue(Dialogue dialogue, int relationshipChange)
    {
        relationship += relationshipChange;

        StartDialogue(dialogue, true);
    }

    public void StartDialogue(Dialogue dialogue, bool datingSim)
    {
        isBranched = false;
        sentences.Clear();
        names.Clear();
        sprites.Clear();

        GameObject branchPanel = GameObject.Find("BranchPanel");
        Button[] buttons = branchPanel.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            Destroy(button.gameObject);
        }

        foreach (DialogueLine line in dialogue.lines)
        {
            names.Enqueue(line.name);
            sentences.Enqueue(line.line);
            sprites.Enqueue(line.sprite);
        }

        currentDialogue = dialogue;

        TextMeshProUGUI[] texts;
        Image[] imgs;

        if (datingSim)
        {
            datingDialogueBox.SetActive(true);
            texts = datingDialogueBox.GetComponentsInChildren<TextMeshProUGUI>();
            imgs = datingDialogueBox.GetComponentsInChildren<Image>();
        }
        else
        {
            rpgDialogueBox.SetActive(true);
            texts = rpgDialogueBox.GetComponentsInChildren<TextMeshProUGUI>();
            imgs = rpgDialogueBox.GetComponentsInChildren<Image>();
        }

        foreach(TextMeshProUGUI text in texts)
        {
            if (text.gameObject.name.Equals("DialogueText"))
            {
                dialogueText = text;
            }
            else if (text.gameObject.name.Equals("NameText"))
            {
                nameText = text;
            }
        }

        foreach(Image img in imgs)
        {
            if (img.gameObject.name.Equals("CharImg"))
            {
                charImg = img;
            }
        }

        displaying = true;
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentDialogue == null)
        {
            return;
        }

        else if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string name = names.Dequeue();
        string line = sentences.Dequeue();
        Sprite spr = sprites.Dequeue();

        nameText.text = name;
        dialogueText.text = line;
        charImg.sprite = spr;

        if (currentDialogue.type == DialogueType.branch && sentences.Count == 0)
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        switch (currentDialogue.type)
        {
            case DialogueType.end:
                if (relationship >= 100)
                {
                    // TODO: Initiate fast travel
                    return;
                }
                else
                {
                    currentDialogue = null;
                    rpgDialogueBox.SetActive(false);
                    datingDialogueBox.SetActive(false);
                }
                break;

            case DialogueType.branch:
                isBranched = true;
                GameObject branchPanel = GameObject.Find("BranchPanel");
                for (int i = 0; i < currentDialogue.branches.Length; i++)
                {
                    GameObject branchButton = Instantiate(branchButtonPrefab, branchPanel.transform);
                    branchButton.GetComponentInChildren<Text>().text = currentDialogue.branches[i];
                    Dialogue nextDialogue = currentDialogue.branchDialogues[i];
                    int relChange = currentDialogue.relationshipChanges[i];
                    branchButton.GetComponentInChildren<Button>().onClick.AddListener(() => StartDialogue(nextDialogue, relChange));
                    if (i == 0)
                    {
                        EventSystem.current.SetSelectedGameObject(branchButton);
                    }
                }
                break;

            default:
                Debug.Log("ERROR: How did you break an enum?");
                break;
        }

        displaying = false;
    }

    public bool GetDisplaying()
    {
        return displaying;
    }
}
