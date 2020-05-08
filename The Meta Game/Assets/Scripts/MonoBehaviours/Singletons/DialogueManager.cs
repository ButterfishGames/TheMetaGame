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

    public TMP_SpriteAsset xbox, dualshock, switchPro, keyboardAndMouse;
    private TMP_SpriteAsset current;

    public Dialogue[] failDialogues;
    public Dialogue[] successDialogues;

    private Image stillImg;
    private Image charImg;
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI nameText;

    private Queue<string> sentences;
    private Queue<string> names;
    private Queue<Sprite> sprites;

    private bool displaying;
    private bool branching;
    private Dialogue currentDialogue;

    private bool primed;

    private Controls controls;

    private int relationship;

    private bool dating;

    private int pInd;

    private InputAction submit;

    private string line;

    private void OnEnable()
    {
        OnControlsChange(GameObject.Find("Player").GetComponent<PlayerInput>());
    }

    public void OnControlsChange(PlayerInput pIn)
    {
        switch(pIn.currentControlScheme)
        {
            case "KeyboardAndMouse":
                current = keyboardAndMouse;
                break;

            case "DualShock":
                current = dualshock;
                break;

            case "Switch":
                current = switchPro;
                break;

            default:
                current = xbox;
                break;
        }

        submit = pIn.actions["submit"];

        submit.started += SubmitStartHandle;
        submit.canceled += SubmitCancHandle;
    }

    private void SubmitStartHandle(InputAction.CallbackContext context)
    {
        if (GameController.singleton.GetPaused())
        {
            return;
        }

        if (displaying)
        {
            primed = true;
        }
    }

    private void SubmitCancHandle(InputAction.CallbackContext context)
    {
        if (GameController.singleton.GetPaused())
        {
            primed = false;
        }

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
        StartDialogue(dialogue, false, -1);
    }

    public void StartDialogue(Dialogue dialogue, int relationshipChange, int partnerInd)
    {
        relationship += relationshipChange;

        StartDialogue(dialogue, true, partnerInd);
    }

    public void StartDialogue(Dialogue dialogue, bool datingSim, int partnerInd)
    {
        dating = datingSim;
        sentences.Clear();
        names.Clear();
        sprites.Clear();

        pInd = partnerInd;

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

        dialogueText.spriteAsset = current;

        displaying = true;
        branching = false;
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
            EndDialogue(true);
            return;
        }

        string name = names.Dequeue();
        line = sentences.Dequeue();
        Sprite spr = sprites.Dequeue();

        line = DialogueParser(line);

        nameText.text = name;
        dialogueText.text = line;
        charImg.sprite = spr;

        if (currentDialogue.type == DialogueType.branch && sentences.Count == 0)
        {
            EndDialogue(true);
        }
    }

    public void EndDialogue(bool normal)
    {
        Debug.Log(normal);
        if (normal)
        {
            switch (currentDialogue.type)
            {
                case DialogueType.end:
                    if (dating)
                    {
                        Debug.Log(relationship);
                        if (relationship >= 100)
                        {
                            GameController.singleton.StartCoroutine(GameController.singleton.DateMap());

                            displaying = false;
                            return;
                        }
                        else
                        {
                            StartDialogue(failDialogues[pInd], true, pInd);
                        }
                    }
                    else
                    {
                        currentDialogue = null;
                        rpgDialogueBox.SetActive(false);
                        datingDialogueBox.SetActive(false);

                        displaying = false;
                    }
                    break;

                case DialogueType.branch:
                    GameObject branchPanel = GameObject.Find("BranchPanel");
                    for (int i = 0; i < currentDialogue.branches.Length; i++)
                    {
                        GameObject branchButton = Instantiate(branchButtonPrefab, branchPanel.transform);
                        branchButton.GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.branches[i];
                        Dialogue nextDialogue = currentDialogue.branchDialogues[i];
                        int relChange = currentDialogue.relationshipChanges[i];
                        branchButton.GetComponentInChildren<Button>().onClick.AddListener(() => StartDialogue(nextDialogue, relChange, pInd));
                        if (i == 0)
                        {
                            EventSystem.current.SetSelectedGameObject(branchButton);
                        }
                    }

                    displaying = false;
                    branching = true;
                    break;

                default:
                    Debug.Log("ERROR: How did you break an enum?");
                    break;
            }
        }
        else
        {
            sentences = new Queue<string>();
            names = new Queue<string>();
            sprites = new Queue<Sprite>();

            GameObject branchPanel = GameObject.Find("BranchPanel");
            Button[] branchButtons = branchPanel.GetComponentsInChildren<Button>();
            foreach (Button button in branchButtons)
            {
                Destroy(button.gameObject);
            }

            currentDialogue = null;
            rpgDialogueBox.SetActive(false);
            datingDialogueBox.SetActive(false);
            
            displaying = false;
        }
    }

    public bool GetDisplaying()
    {
        return displaying;
    }

    public bool GetBranching()
    {
        return branching;
    }

    public string DialogueParser(string input)
    {
        string output = input;

        if (output.Contains("|*PFJUMP*|"))
        {
            if (SettingsController.singleton.pfUpJump)
            {
                output = output.Replace("|*PFJUMP*|", ButtonsLib.singleton.DialogueAction("Jump") + " or " + ButtonsLib.singleton.DialogueExpl("DPadUp"));
            }
            else
            {
                output = output.Replace("|*PFJUMP*|", ButtonsLib.singleton.DialogueAction("Jump"));
            }
        }

        if (output.Contains("|*FGJUMP*|"))
        {
            if (SettingsController.singleton.fgUpJump)
            {
                output = output.Replace("|*FGJUMP*|", ButtonsLib.singleton.DialogueAction("Jump") + " or " + ButtonsLib.singleton.DialogueExpl("DPadUp"));
            }
            else
            {
                output = output.Replace("|*FGJUMP*|", ButtonsLib.singleton.DialogueAction("Jump"));
            }
        }

        if (output.Contains("|*JUMP*|")) output = output.Replace("|*JUMP*|", ButtonsLib.singleton.DialogueAction("Jump"));
        if (output.Contains("|*PAUSE*|")) output = output.Replace("|*PAUSE*|", ButtonsLib.singleton.DialogueAction("Pause"));
        if (output.Contains("|*MENU*|")) output = output.Replace("|*MENU*|", ButtonsLib.singleton.DialogueAction("Menu"));
        if (output.Contains("|*SWITCH_L*|")) output = output.Replace("|*SWITCH_L*|", ButtonsLib.singleton.DialogueAction("SwitchModeNeg"));
        if (output.Contains("|*SWITCH_R*|")) output = output.Replace("|*SWITCH_R*|", ButtonsLib.singleton.DialogueAction("SwitchModePos"));
        if (output.Contains("|*SUBMIT*|")) output = output.Replace("|*SUBMIT*|", ButtonsLib.singleton.DialogueAction("Submit"));
        if (output.Contains("|*CANCEL*|")) output = output.Replace("|*CANCEL*|", ButtonsLib.singleton.DialogueAction("Cancel"));
        if (output.Contains("|*LIGHT*|")) output = output.Replace("|*LIGHT*|", ButtonsLib.singleton.DialogueAction("Light"));
        if (output.Contains("|*MEDIUM*|")) output = output.Replace("|*MEDIUM*|", ButtonsLib.singleton.DialogueAction("Medium"));
        if (output.Contains("|*HEAVY*|")) output = output.Replace("|*HEAVY*|", ButtonsLib.singleton.DialogueAction("Heavy"));
        if (output.Contains("|*ZOOM*|")) output = output.Replace("|*ZOOM*|", ButtonsLib.singleton.DialogueAction("Zoom"));
        if (output.Contains("|*FIRE*|")) output = output.Replace("|*FIRE*|", ButtonsLib.singleton.DialogueAction("Fire"));

        if (output.Contains("|*MOVE*|")) output = output.Replace("|*MOVE*|", ButtonsLib.singleton.DialogueExpl("LStick"));
        if (output.Contains("|*LOOK*|")) output = output.Replace("|*LOOK*|", ButtonsLib.singleton.DialogueExpl("RStick"));
        if (output.Contains("|*UP*|")) output = output.Replace("|*UP*|", ButtonsLib.singleton.DialogueExpl("DPadUp"));
        if (output.Contains("|*RIGHT*|")) output = output.Replace("|*RIGHT*|", ButtonsLib.singleton.DialogueExpl("DPadRight"));
        if (output.Contains("|*LEFT*|")) output = output.Replace("|*LEFT*|", ButtonsLib.singleton.DialogueExpl("DPadLeft"));
        if (output.Contains("|*DOWN*|")) output = output.Replace("|*DOWN*|", ButtonsLib.singleton.DialogueExpl("DPadDown"));

        if (output.Contains("|*NOTEDOWN*|")) output = output.Replace("|*NOTEDOWN*|", ButtonsLib.singleton.DialogueNote("Down"));
        if (output.Contains("|*NOTERIGHT*|")) output = output.Replace("|*NOTERIGHT*|", ButtonsLib.singleton.DialogueNote("Right"));
        if (output.Contains("|*NOTELEFT*|")) output = output.Replace("|*NOTELEFT*|", ButtonsLib.singleton.DialogueNote("Left"));
        if (output.Contains("|*NOTEUP*|")) output = output.Replace("|*NOTEUP*|", ButtonsLib.singleton.DialogueNote("Up"));

        if (output.Contains("|*ATTACK*|")) output = output.Replace("|*ATTACK*|", ButtonsLib.singleton.DialogueAttack());

        return output;
    }
}
