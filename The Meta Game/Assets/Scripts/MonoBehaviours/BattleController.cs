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

    private Troop currTroop;

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

        foreach (Enemy enemy in currTroop.enemies)
        {
            GameObject img = Instantiate(enemyImgPrefab, enemyCharSpace);
            img.GetComponent<Image>().sprite = enemy.sprite;
            enemy.SetImg(img);

            GameObject stats = Instantiate(enemyStatPrefab, enemyStatPanel);
            enemy.SetStats(stats);
            stats.GetComponent<Button>().onClick.AddListener(() => Target(enemy));
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
                    text.text = enemy.name;
                }
                else if (text.name.Equals("HPText"))
                {
                    enemy.SetHP(enemy.maxHP);
                    text.text = enemy.GetHP() + "/" + enemy.maxHP;
                }
            }
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
        enemy.GetImg().GetComponent<RectTransform>().anchoredPosition += new Vector2(30, 0);
        Attack attack = enemy.attacks[Random.Range(0, enemy.attacks.Length)];

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = attack.name;
        yield return new WaitForSeconds(0.75f);

        enemy.UseAttack(attack);

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
            enemy.GetImg().GetComponent<RectTransform>().anchoredPosition -= new Vector2(30, 0);
            yield return new WaitForSeconds(0.25f);
            NextTurn();
        }
    }

    public void Target(Enemy enemy)
    {
        switch (currCommand)
        {
            case Command.attack:
                StartCoroutine(AttackRtn(enemy));
                break;

            case Command.magic:
                StartCoroutine(DmgSpellRtn(currSpell, enemy));
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

    private IEnumerator AttackRtn(Enemy enemy)
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

        enemy.Damage(dmg);

        if (crit)
        {
            yield return new WaitForSeconds(0.75f);
        }

        if (enemy.GetHP() <= 0)
        {
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = enemy.name + " was defeated!";
            bool replaceE1 = false;

            if (enemy.GetStats() == enemy1)
            {
                replaceE1 = true;
            }

            List<Enemy> temp = new List<Enemy>(currTroop.enemies);
            temp.Remove(enemy);
            Destroy(enemy.GetStats());
            Destroy(enemy.GetImg());
            currTroop.enemies = temp.ToArray();
            List<Button> buttonList = new List<Button>();
            foreach (Enemy foe in currTroop.enemies)
            {
                buttonList.Add(foe.GetStats().GetComponent<Button>());
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
                enemy1 = currTroop.enemies[0].GetStats();
            }
        }
        else
        {
            TextMeshProUGUI[] texts = enemy.GetStats().GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                if (text.name.Equals("HPText"))
                {
                    text.text = enemy.GetHP() + "/" + enemy.maxHP;
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
                spellButton.GetComponentInChildren<TextMeshProUGUI>().text = spell.spell.spellName;
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

    private IEnumerator DmgSpellRtn(Spell spell, Enemy enemy)
    {
        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = spell.name;
        yield return new WaitForSeconds(0.75f);

        int mag = GameController.singleton.GetMagic();

        bool won = false;

        int dmg = Mathf.FloorToInt((spell.baseAmt + mag) * Random.Range(0.75f, 1.25f));

        enemy.Damage(dmg);

        if (enemy.GetHP() <= 0)
        {
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = enemy.name + " was defeated!";
            bool replaceE1 = false;

            if (enemy.GetStats() == enemy1)
            {
                replaceE1 = true;
            }

            List<Enemy> temp = new List<Enemy>(currTroop.enemies);
            temp.Remove(enemy);
            Destroy(enemy.GetStats());
            Destroy(enemy.GetImg());
            currTroop.enemies = temp.ToArray();
            List<Button> buttonList = new List<Button>();
            foreach (Enemy foe in currTroop.enemies)
            {
                buttonList.Add(foe.GetStats().GetComponent<Button>());
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
                enemy1 = currTroop.enemies[0].GetStats();
            }
        }
        else
        {
            TextMeshProUGUI[] texts = enemy.GetStats().GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                if (text.name.Equals("HPText"))
                {
                    text.text = enemy.GetHP() + "/" + enemy.maxHP;
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

        int dmg = Mathf.FloorToInt((spell.baseAmt + mag) * Random.Range(0.75f, 1.25f));

        List<Enemy> dead = new List<Enemy>();

        foreach (Enemy enemy in currTroop.enemies)
        {
            enemy.Damage(dmg);

            TextMeshProUGUI[] texts = enemy.GetStats().GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                if (text.name.Equals("HPText"))
                {
                    text.text = enemy.GetHP() + "/" + enemy.maxHP;
                }
            }

            if (enemy.GetHP() <= 0)
            {
                messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = enemy.name + " was defeated!";

                if (enemy.GetStats() == enemy1)
                {
                    replaceE1 = true;
                }

                Destroy(enemy.GetStats());
                Destroy(enemy.GetImg());
                dead.Add(enemy);
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
                buttonList.Add(foe.GetStats().GetComponent<Button>());
            }

            if (replaceE1)
            {
                enemy1 = currTroop.enemies[0].GetStats();
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
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You failed to escape!";
            NextTurn();
        }
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
public class Enemy
{
    public string name;
    public Sprite sprite;
    public int maxHP;
    public Attack[] attacks;

    private int currHP;
    private GameObject stats;
    private GameObject img;

    public Enemy()
    {
        currHP = maxHP;
    }

    public void UseAttack(Attack attack)
    {
        int damage = attack.baseDmg;
        damage = Mathf.FloorToInt(damage * Random.Range(1 - attack.var, 1 + attack.var));
        GameController.singleton.Damage(damage);
    }

    public int GetHP()
    {
        return currHP;
    }

    public void SetHP(int val)
    {
        currHP = val;
    }

    public void Damage(int amt)
    {
        currHP -= amt;
    }

    public void SetStats(GameObject statObj)
    {
        stats = statObj;
    }

    public GameObject GetStats()
    {
        return stats;
    }

    public void SetImg(GameObject imgObj)
    {
        img = imgObj;
    }

    public GameObject GetImg()
    {
        return img;
    }
}

[System.Serializable]
public struct Attack
{
    public string name;
    public int baseDmg;

    [Range(0, 1)]
    public float var;
}
