using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC : EnemyBehaviour
{
    public static NPC shopkeeper;

    public Dialogue firstDialogue, otherDialogue;
    public bool interacted;

    public enum NPCType
    {
        standard,
        restoreHP,
        restoreMP,
        learnSpell,
        learnSkill,
        boostHP,
        boostMP,
        boostStrength,
        boostMagic,
        shop
    };
    public NPCType npcType;

    public int amt;
    public Spell spell;
    public Skill skill;

    [Header("Shop Vars")]
    public GameObject shopPanel;
    public GameObject shopContent;
    public GameObject shopButton;

    public Sprite coinSprite, powerUpSprite, conceptArtSprite, songSprite;

    private readonly string[] boostTypeStrs = { "HP", "MP", "Strength", "Magic" };

    public bool containsSpell = false;

    public int[] boostTypes;
    public int[] boostAmts;
    public bool[] boostsPurchased;
    public int[] boostCosts;

    public int[] artInds;
    public int[] artCosts;

    public int[] songInds;
    public int[] songCosts;

    public void Start()
    {
        if (npcType == NPCType.shop)
        {
            Debug.Log("Why not work?");
            shopkeeper = this;
        }

        interacted = false;

        switch(npcType)
        {
            case NPCType.shop:
            case NPCType.standard:
                animator.SetInteger("npcType", 0);
                break;

            case NPCType.restoreHP:
                animator.SetInteger("npcType", 1);
                break;

            case NPCType.restoreMP:
                animator.SetInteger("npcType", 2);
                break;

            case NPCType.learnSpell:
                animator.SetInteger("npcType", 3);
                break;

            case NPCType.learnSkill:
                animator.SetInteger("npcType", 4);
                break;

            case NPCType.boostHP:
                animator.SetInteger("npcType", 5);
                break;

            case NPCType.boostMP:
                animator.SetInteger("npcType", 6);
                break;

            case NPCType.boostStrength:
                animator.SetInteger("npcType", 7);
                break;

            case NPCType.boostMagic:
                animator.SetInteger("npcType", 8);
                break;
        }
    }

    private void OnEnable()
    {
        animator.SetBool("platformer", false);
        animator.SetBool("fighter", false);
        animator.SetBool("racing", false);
        animator.SetBool("rpg", true);
    }

    private void OnDisable()
    {
        animator.SetBool("rpg", false);
    }

    public void Interact()
    {
        StartCoroutine(InteractRtn());
    }

    private IEnumerator InteractRtn()
    {
        if (interacted)
        {
            DialogueManager.singleton.StartDialogue(otherDialogue);
        }
        else
        {
            DialogueManager.singleton.StartDialogue(firstDialogue);
        }

        yield return new WaitUntil(() => !DialogueManager.singleton.GetDisplaying());

        bool found = false;

        switch (npcType)
        {
            case NPCType.restoreHP:
                GameController.singleton.Damage(-GameController.singleton.maxHP);
                SaveManager.singleton.UpdatePlayerData();
                break;

            case NPCType.restoreMP:
                GameController.singleton.Cast(-GameController.singleton.maxMP);
                SaveManager.singleton.UpdatePlayerData();
                break;

            case NPCType.boostHP:
                if (!interacted)
                {
                    GameController.singleton.maxHP += amt;
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.boostMP:
                if (!interacted)
                {
                    GameController.singleton.maxMP += amt;
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.boostStrength:
                if (!interacted)
                {
                    GameController.singleton.SetStrength(GameController.singleton.GetStrength() + amt);
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.boostMagic:
                if (!interacted)
                {
                    GameController.singleton.SetMagic(GameController.singleton.GetMagic() + amt);
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.learnSpell:
                if (!interacted)
                {
                    for (int i = 0; i < GameController.singleton.spellList.Length && !found; i++)
                    {
                        if (GameController.singleton.spellList[i].spell.spellName.Equals(spell.spellName))
                        {
                            GameController.singleton.spellList[i].unlocked = true;
                            SaveManager.singleton.UpdatePlayerData();
                            found = true;
                        }
                    }
                }
                break;

            case NPCType.shop:
                OpenShop();
                break;
        }

        if (!interacted)
        {
            interacted = true;
            SaveManager.singleton.UpdateSceneData();
        }
    }

    private void OpenShop()
    {
        FindObjectOfType<RPGController>().shopping = true;
        GameController.singleton.ToggleSwitchPanel(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        shopPanel.SetActive(true);
        GameObject button;

        if (containsSpell)
        {
            button = Instantiate(shopButton, shopContent.transform);
            button.transform.Find("Icon").GetComponent<Image>().sprite = powerUpSprite;
            button.transform.Find("ItemText").GetComponent<TextMeshProUGUI>().text = "Heal Spell";
            button.transform.Find("CostText").GetComponent<TextMeshProUGUI>().text = "<sprite=0>" + 100;
            Button iButton = button.GetComponent<Button>();
            if (GameController.singleton.spellList[3].unlocked)
            {
                iButton.interactable = false;
                iButton.transform.Find("ItemText").GetComponent<TextMeshProUGUI>().text = "SOLD OUT";
            }
            else
            {
                iButton.onClick.AddListener(() => Buy("Spell", 3, 100, iButton));
            }
        }

        for (int i = 0; i < boostsPurchased.Length; i++)
        {
            button = Instantiate(shopButton, shopContent.transform);
            button.transform.Find("Icon").GetComponent<Image>().sprite = powerUpSprite;
            button.transform.Find("ItemText").GetComponent<TextMeshProUGUI>().text = boostTypeStrs[boostTypes[i]] + " +" + boostAmts[i];
            button.transform.Find("CostText").GetComponent<TextMeshProUGUI>().text = "<sprite=0>" + boostCosts[i];
            Button iButton = button.GetComponent<Button>();
            int temp = i;
            if (boostsPurchased[i])
            {
                iButton.interactable = false;
                iButton.transform.Find("ItemText").GetComponent<TextMeshProUGUI>().text = "SOLD OUT";
            }
            else
            {
                iButton.onClick.AddListener(() => Buy("PowerUp", temp, boostCosts[temp], iButton));
            }
        }

        for (int i = 0; i < artInds.Length; i++)
        {
            button = Instantiate(shopButton, shopContent.transform);
            button.transform.Find("Icon").GetComponent<Image>().sprite = conceptArtSprite;
            button.transform.Find("ItemText").GetComponent<TextMeshProUGUI>().text = GameController.singleton.artList[artInds[i]].name;
            button.transform.Find("CostText").GetComponent<TextMeshProUGUI>().text = "<sprite=0>" + artCosts[i];
            Button iButton = button.GetComponent<Button>();
            int temp = i;
            if (GameController.singleton.artList[artInds[i]].unlocked)
            {
                iButton.interactable = false;
                iButton.transform.Find("ItemText").GetComponent<TextMeshProUGUI>().text = "SOLD OUT";
            }
            else
            {
                iButton.onClick.AddListener(() => Buy("ConceptArt", artInds[temp], artCosts[temp], iButton));
            }
        }

        for (int i = 0; i < songInds.Length; i++)
        {
            button = Instantiate(shopButton, shopContent.transform);
            button.transform.Find("Icon").GetComponent<Image>().sprite = songSprite;
            button.transform.Find("ItemText").GetComponent<TextMeshProUGUI>().text = GameController.singleton.songList[songInds[i]].name;
            button.transform.Find("CostText").GetComponent<TextMeshProUGUI>().text = "<sprite=0>" + songCosts[i];
            Button iButton = button.GetComponent<Button>();
            int temp = i;
            if (GameController.singleton.songList[songInds[i]].unlocked)
            {
                iButton.interactable = false;
                button.transform.Find("ItemText").GetComponent<TextMeshProUGUI>().text = "SOLD OUT";
            }
            else
            {
                iButton.onClick.AddListener(() => Buy("Music", songInds[temp], songCosts[temp], iButton));
            }
        }

        button = Instantiate(shopButton, shopContent.transform);
        button.GetComponent<Image>().enabled = false;
        Transform[] children = button.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.gameObject != button)
            {
                child.gameObject.SetActive(false);
            }
        }
        shopPanel.transform.Find("GoldPanel").Find("GoldText").GetComponent<TextMeshProUGUI>().text = GameController.singleton.GetGold().ToString();
    }

    public void Buy(string type, int ind, int cost, Button button)
    {
        if (cost > GameController.singleton.GetGold())
        {
            return;
        }
        else
        {
            GameController.singleton.AddGold(-cost);
        }

        switch (type)
        {
            case "Spell":
                GameController.singleton.spellList[ind].unlocked = true;
                break;

            case "PowerUp":
                switch (boostTypes[ind])
                {
                    case 0:
                        GameController.singleton.maxHP += boostAmts[ind];
                        break;

                    case 1:
                        GameController.singleton.maxMP += boostAmts[ind];
                        break;

                    case 2:
                        GameController.singleton.SetStrength(GameController.singleton.GetStrength() + boostAmts[ind]);
                        break;

                    case 3:
                        GameController.singleton.SetMagic(GameController.singleton.GetMagic() + boostAmts[ind]);
                        break;
                }
                break;

            case "ConceptArt":
                GameController.singleton.artList[ind].unlocked = true;
                break;

            case "Music":
                GameController.singleton.songList[ind].unlocked = true;
                break;
        }

        button.interactable = false;
        button.transform.Find("ItemText").GetComponent<TextMeshProUGUI>().text = "SOLD OUT";
        shopPanel.transform.Find("GoldPanel").Find("GoldText").GetComponent<TextMeshProUGUI>().text = GameController.singleton.GetGold().ToString();
    }
}
