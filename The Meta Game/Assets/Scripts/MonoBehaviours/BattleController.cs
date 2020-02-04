using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BattleController : MonoBehaviour
{
    [Range(0, 1)]
    public float fleeChance;

    public TextMeshProUGUI playerStats;

    public GameObject enemyImgPrefab;

    public GameObject enemyStatPrefab;

    public GameObject spellButtonPrefab;

    public GameObject messagePanel;

    public GameObject magicPanel;

    public GameObject attackButton;

    public Transform enemyCharSpace;

    public Transform enemyStatPanel;

    public ScrollRect magicScroll;

    public Troop[] troops;

    private Button[] mainButtons;

    private Button[] enemyButtons;

    private Button[] spellButtons;

    public Troop currTroop;

    private GameObject enemy1;

    private int scrollOffset;

    private int currTurn;

    private bool onMain;

    private bool onMagic;

    private enum Command
    {
        attack,
        magic,
        none
    };

    private Command currCommand;

    private Spell currSpell;

    // Start is called before the first frame update
    void Start()
    {
        currCommand = Command.none;

        mainButtons = FindObjectsOfType<Button>();
        spellButtons = new Button[] { null };

        messagePanel.SetActive(false);

        playerStats.text = "Player\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + "\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP;

        currTroop = troops[Random.Range(0, troops.Length)];
        List<Button> buttonList = new List<Button>();

        for(int i = 0; i < currTroop.enemies.Length; i++)
        {
            GameObject img = Instantiate(enemyImgPrefab, enemyCharSpace);
            img.GetComponent<Image>().sprite = currTroop.enemies[i].source.sprite;
            currTroop.enemies[i].img = img;

            GameObject stats = Instantiate(enemyStatPrefab, enemyStatPanel);
            currTroop.enemies[i].stats = stats;

            int enemyIndex = i;
            stats.GetComponent<Button>().onClick.AddListener(() => Target(enemyIndex));
            buttonList.Add(stats.GetComponent<Button>());

            if (enemy1 == null)
            {
                enemy1 = stats;
            }

            TextMeshProUGUI[] texts = stats.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (TextMeshProUGUI text in texts)
            {
                if (text.name.Equals("NameText"))
                {
                    if (currTroop.enemies[i].name.Equals(""))
                    {
                        currTroop.enemies[i].name = currTroop.enemies[i].source.name;
                    }
                    text.text = currTroop.enemies[i].name;
                }
                else if (text.name.Equals("HPText"))
                {
                    currTroop.enemies[i].currHP = currTroop.enemies[i].source.maxHP;
                    text.text = currTroop.enemies[i].currHP + "/" + currTroop.enemies[i].source.maxHP;
                }
            }

            stats.GetComponent<Button>().interactable = false;
        }
        enemyButtons = buttonList.ToArray();

        onMain = true;
        currTurn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !onMain)
        {
            ReturnToMain();
        }

        if (onMagic && EventSystem.current.currentSelectedGameObject != null)
        {
            RectTransform selected = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();
            int posOffset = scrollOffset * 90;
            if (selected.localPosition.y > (-45 - posOffset))
            {
                magicScroll.content.localPosition = new Vector3(magicScroll.content.localPosition.x,
                    magicScroll.content.localPosition.y - (selected.localPosition.y + 45 + posOffset), 0);
                scrollOffset--;
            }
            else if (selected.localPosition.y < (-315 - posOffset))
            {
                magicScroll.content.localPosition = new Vector3(magicScroll.content.localPosition.x,
                    magicScroll.content.localPosition.y - (selected.localPosition.y + 315 + posOffset), 0);
                scrollOffset++;
            }
        }
    }

    private void NextTurn()
    {
        currCommand = Command.none;

        spellButtons = new Button[] { null };

        currTurn++;
        if (currTurn > currTroop.enemies.Length)
        {
            currTurn = 0;
        }

        Button[] buttons = FindObjectsOfType<Button>();

        if (currTurn == 0)
        {
            ReturnToMain();
        }
        else
        {
            foreach (Button button in buttons)
            {
                button.interactable = false;
            }
            StartCoroutine(EnemyTurn(currTroop.enemies[currTurn - 1]));
        }
    }

    private IEnumerator EnemyTurn(Enemy enemy)
    {
        enemy.img.GetComponent<RectTransform>().anchoredPosition += new Vector2(30, 0);
        Attack attack = enemy.source.attacks[Random.Range(0, enemy.source.attacks.Length)];

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = attack.name;
        yield return new WaitForSeconds(0.75f);

        enemy.source.UseAttack(attack);

        if (GameController.singleton.GetHP() <= 0)
        {
            playerStats.text = "Player\n0/" + GameController.singleton.maxHP + "\n"
                + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP;
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You lose...";
            yield return new WaitForSeconds(1);
            GameController.singleton.Die();
        }
        else
        {
            playerStats.text = "Player\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + "\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP;
            yield return new WaitForSeconds(0.75f);
            messagePanel.SetActive(false);
            enemy.img.GetComponent<RectTransform>().anchoredPosition -= new Vector2(30, 0);
            yield return new WaitForSeconds(0.25f);
            NextTurn();
        }
    }

    public void Target(int enemyIndex)
    {
        switch (currCommand)
        {
            case Command.attack:
                StartCoroutine(AttackRtn(enemyIndex));
                break;

            case Command.magic:
                StartCoroutine(DmgSpellRtn(currSpell, enemyIndex));
                break;

            default:
                Debug.Log("ERROR: NO COMMAND REQUIRING TARGET FOUND");
                break;
        }

        if (spellButtons[0] != null)
        {
            foreach (Button button in spellButtons)
            {
                button.interactable = false;
            }
        }

        foreach (Button button in enemyButtons)
        {
            button.interactable = false;
        }

        foreach (Button button in mainButtons)
        {
            button.interactable = false;
        }
    }

    public void AttackCmd()
    {
        currCommand = Command.attack;
        EventSystem.current.SetSelectedGameObject(enemy1);

        foreach (Button button in enemyButtons)
        {
            button.interactable = true;
        }

        foreach (Button button in mainButtons)
        {
            button.interactable = false;
        }

        onMain = false;
    }

    private IEnumerator AttackRtn(int enemyIndex)
    {
        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Attack!";
        yield return new WaitForSeconds(0.75f);

        int str = GameController.singleton.GetStrength();

        bool won = false;
        bool crit = false;

        int dmg = Mathf.FloorToInt(str * Random.Range(0.75f, 1.25f));
        if (dmg == Mathf.FloorToInt(str * 1.25f))
        {
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Critical hit!";
            dmg += Mathf.FloorToInt(str * Random.Range(0.75f, 1.25f));
            crit = true;
        }

        currTroop.enemies[enemyIndex].currHP -= dmg;

        if (crit)
        {
            yield return new WaitForSeconds(0.75f);
        }

        if (currTroop.enemies[enemyIndex].currHP <= 0)
        {
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = currTroop.enemies[enemyIndex].name + " was defeated!";
            bool replaceE1 = false;

            if (currTroop.enemies[enemyIndex].stats == enemy1)
            {
                replaceE1 = true;
            }

            List<Enemy> temp = new List<Enemy>(currTroop.enemies);
            temp.Remove(currTroop.enemies[enemyIndex]);
            Destroy(currTroop.enemies[enemyIndex].stats);
            Destroy(currTroop.enemies[enemyIndex].img);
            currTroop.enemies = temp.ToArray();
            List<Button> buttonList = new List<Button>();
            foreach (Enemy foe in currTroop.enemies)
            {
                buttonList.Add(foe.stats.GetComponent<Button>());
            }

            enemyButtons = buttonList.ToArray();
            for (int i = 0; i < enemyButtons.Length; i++)
            {
                int ind = i;
                enemyButtons[i].onClick.RemoveAllListeners();
                enemyButtons[i].onClick.AddListener(() => Target(ind));
            }

            yield return new WaitForSeconds(0.75f);

            if (currTroop.enemies.Length < 1)
            {
                messagePanel.SetActive(true);
                messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You win!";
                GameController.singleton.StartCoroutine(GameController.singleton.UnloadBattle());
                won = true;
            }
            else if (replaceE1)
            {
                enemy1 = currTroop.enemies[0].stats;
            }
        }
        else
        {
            TextMeshProUGUI[] texts = currTroop.enemies[enemyIndex].stats.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                if (text.name.Equals("HPText"))
                {
                    text.text = currTroop.enemies[enemyIndex].currHP + "/" + currTroop.enemies[enemyIndex].source.maxHP;
                }
            }
        }

        if (!won)
        {
            messagePanel.SetActive(false);
            ReturnToMain();
            NextTurn();
        }
    }

    public void MagicCmd()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        scrollOffset = 0;
        magicPanel.SetActive(true);
        magicScroll.content.anchoredPosition = Vector2.zero;
        GameObject spell1 = null;
        int count = 0;

        List<Button> buttonList = new List<Button>();
        foreach (GameController.SpellStr spell in GameController.singleton.spellList)
        {
            if (spell.unlocked)
            {
                count++;
                GameObject spellButton = Instantiate(spellButtonPrefab, magicScroll.content);
                spellButton.GetComponentInChildren<TextMeshProUGUI>().text = spell.spell.spellName + " [" + spell.spell.manaCost + "]";
                spellButton.GetComponent<Button>().onClick.AddListener(() => Cast(spell.spell));
                buttonList.Add(spellButton.GetComponent<Button>());
                if (spell1 == null)
                {
                    spell1 = spellButton;
                }
            }
        }
        spellButtons = buttonList.ToArray();

        foreach (Button button in spellButtons)
        {
            button.interactable = true;
        }

        foreach (Button button in enemyButtons)
        {
            button.interactable = false;
        }

        foreach (Button button in mainButtons)
        {
            button.interactable = false;
        }

        EventSystem.current.SetSelectedGameObject(spell1);
        onMain = false;
        onMagic = true;
    }

    public void Cast(Spell spell)
    {
        if (spell.manaCost > GameController.singleton.GetMP())
        {
            return;
        }

        onMagic = false;
        foreach (Button button in spellButtons)
        {
            button.interactable = false;
        }

        foreach (Button button in mainButtons)
        {
            button.interactable = false;
        }

        switch (spell.spellType)
        {
            case Spell.SpellType.damage:
                currCommand = Command.magic;
                currSpell = spell;
                foreach (Button button in enemyButtons)
                {
                    button.interactable = true;
                }

                EventSystem.current.SetSelectedGameObject(enemy1);
                break;

            case Spell.SpellType.damageAll:
                foreach (Button button in enemyButtons)
                {
                    button.interactable = false;
                }

                StartCoroutine(DmgAllSpellRtn(spell));
                break;

            case Spell.SpellType.heal:
                foreach (Button button in enemyButtons)
                {
                    button.interactable = false;
                }

                StartCoroutine(HealSpellRtn(spell));
                break;

            default:
                Debug.Log("ERROR: INVALID SPELL TYPE");
                break;
        }
    }

    private IEnumerator DmgSpellRtn(Spell spell, int enemyIndex)
    {
        GameController.singleton.Cast(spell.manaCost);
        playerStats.text = "Player\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + "\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP;

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = spell.name;
        yield return new WaitForSeconds(0.75f);

        int mag = GameController.singleton.GetMagic();

        bool won = false;

        int dmg = Mathf.FloorToInt((spell.baseAmt + mag) * Random.Range(0.75f, 1.25f));

        currTroop.enemies[enemyIndex].currHP -= dmg;

        if (currTroop.enemies[enemyIndex].currHP <= 0)
        {
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = currTroop.enemies[enemyIndex].name + " was defeated!";
            bool replaceE1 = false;

            if (currTroop.enemies[enemyIndex].stats == enemy1)
            {
                replaceE1 = true;
            }

            List<Enemy> temp = new List<Enemy>(currTroop.enemies);
            temp.Remove(currTroop.enemies[enemyIndex]);
            Destroy(currTroop.enemies[enemyIndex].stats);
            Destroy(currTroop.enemies[enemyIndex].img);
            currTroop.enemies = temp.ToArray();
            Debug.Log(currTroop.enemies.Length);
            List<Button> buttonList = new List<Button>();
            foreach (Enemy foe in currTroop.enemies)
            {
                buttonList.Add(foe.stats.GetComponent<Button>());
            }

            enemyButtons = buttonList.ToArray();
            for (int i = 0; i < enemyButtons.Length; i++)
            {
                int ind = i;
                enemyButtons[i].onClick.RemoveAllListeners();
                enemyButtons[i].onClick.AddListener(() => Target(ind));
            }

            yield return new WaitForSeconds(0.75f);

            if (currTroop.enemies.Length < 1)
            {
                messagePanel.SetActive(true);
                messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You win!";
                GameController.singleton.StartCoroutine(GameController.singleton.UnloadBattle());
                won = true;
            }
            else if (replaceE1)
            {
                enemy1 = currTroop.enemies[0].stats;
            }
        }
        else
        {
            TextMeshProUGUI[] texts = currTroop.enemies[enemyIndex].stats.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                if (text.name.Equals("HPText"))
                {
                    text.text = currTroop.enemies[enemyIndex].currHP + "/" + currTroop.enemies[enemyIndex].source.maxHP;
                }
            }
        }

        if (!won)
        {
            messagePanel.SetActive(false);
            ReturnToMain();
            NextTurn();
        }
    }

    private IEnumerator DmgAllSpellRtn(Spell spell)
    {
        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = spell.name;
        yield return new WaitForSeconds(0.75f);

        int mag = GameController.singleton.GetMagic();

        bool replaceE1 = false;

        List<Enemy> dead = new List<Enemy>();

        for (int i = 0; i < currTroop.enemies.Length; i++)
        {
            int dmg = Mathf.FloorToInt((spell.baseAmt + mag) * Random.Range(0.75f, 1.25f));

            currTroop.enemies[i].currHP -= dmg;

            TextMeshProUGUI[] texts = currTroop.enemies[i].stats.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                if (text.name.Equals("HPText"))
                {
                    text.text = currTroop.enemies[i].currHP + "/" + currTroop.enemies[i].source.maxHP;
                }
            }

            if (currTroop.enemies[i].currHP <= 0)
            {
                messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = currTroop.enemies[i].name + " was defeated!";

                if (currTroop.enemies[i].stats == enemy1)
                {
                    replaceE1 = true;
                }

                Destroy(currTroop.enemies[i].stats);
                Destroy(currTroop.enemies[i].img);
                dead.Add(currTroop.enemies[i]);
                yield return new WaitForSeconds(0.75f);
            }
        }

        if (dead.ToArray().Length == currTroop.enemies.Length)
        {
            messagePanel.SetActive(true);
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You win!";
            GameController.singleton.StartCoroutine(GameController.singleton.UnloadBattle());
        }
        else
        {
            List<Enemy> temp = new List<Enemy>(currTroop.enemies);
            foreach (Enemy deadEnemy in dead)
            {
                temp.Remove(deadEnemy);
            }
            currTroop.enemies = temp.ToArray();
            List<Button> buttonList = new List<Button>();
            foreach (Enemy foe in currTroop.enemies)
            {
                buttonList.Add(foe.stats.GetComponent<Button>());
            }
            enemyButtons = buttonList.ToArray();

            if (replaceE1)
            {
                enemy1 = currTroop.enemies[0].stats;
            }

            messagePanel.SetActive(false);
            ReturnToMain();
            NextTurn();
        }
    }

    private IEnumerator HealSpellRtn(Spell spell)
    {
        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = spell.name;
        yield return new WaitForSeconds(0.75f);

        int mag = GameController.singleton.GetMagic();

        int amt = Mathf.FloorToInt((spell.baseAmt + mag) * Random.Range(0.75f, 1.25f));

        GameController.singleton.Damage(-amt);

        playerStats.text = "Player\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + "\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP;

        messagePanel.SetActive(false);
        ReturnToMain();
        NextTurn();
    }

    public void ItemCmd()
    {
        GameController.singleton.ErrDisp("Error: BattleController.cs does not contain a definition for 'ItemCmd'");
        NextTurn();
    }

    public void FleeCmd()
    {
        float det = Random.Range(0.0f, 1.0f);

        messagePanel.SetActive(true);

        if (det < fleeChance)
        {
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You successfully escaped!";
            GameController.singleton.StartCoroutine(GameController.singleton.UnloadBattle());
        }
        else
        {
            StartCoroutine(FleeFail());   
        }
    }

    private IEnumerator FleeFail()
    {
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You failed to escape!";
        yield return new WaitForSeconds(0.75f);
        NextTurn();
    }

    private void ReturnToMain()
    {
        onMagic = false;

        if (spellButtons[0] != null)
        {
            foreach (Button button in spellButtons)
            {
                Destroy(button.gameObject);
            }
            magicPanel.SetActive(false);
        }

        foreach (Button button in enemyButtons)
        {
            button.interactable = false;
        }

        foreach (Button button in mainButtons)
        {
            button.interactable = true;
        }

        EventSystem.current.SetSelectedGameObject(attackButton);
        onMain = true;
    }
}

[System.Serializable]
public struct Troop
{
    public Enemy[] enemies;
}

[System.Serializable]
public struct Enemy
{
    public string name;
    public RPGEnemy source;
    public GameObject stats;
    public GameObject img;
    public int currHP;
}

[System.Serializable]
public struct Attack
{
    public string name;
    public int baseDmg;

    [Range(0, 1)]
    public float var;
}
