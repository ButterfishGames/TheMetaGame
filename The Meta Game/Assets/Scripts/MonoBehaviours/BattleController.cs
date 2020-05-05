using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class BattleController : MonoBehaviour
{
    #region variables
    [Range(0, 1)]
    public float fleeChance;

    public TextMeshProUGUI playerStats;

    public GameObject enemyImgPrefab;

    public GameObject enemyStatPrefab;

    public GameObject spellButtonPrefab;

    public GameObject messagePanel;

    public GameObject optionPanel;

    public GameObject attackButton;

    public Transform enemyCharSpace;

    public Transform enemyStatPanel;

    public ScrollRect optionScroll;

    public Troop[] troops;

    private Button[] mainButtons;

    private Button[] enemyButtons;

    private Button[] spellButtons;

    public Troop currTroop;

    private GameObject enemy1;

    private int scrollOffset;

    private int currTurn;

    private bool onMagic;

    private enum Command
    {
        attack,
        magic,
        skill,
        none
    };

    private Command currCommand;

    private Spell currSpell;

    private Skill currSkill;

    private bool guarding = false;
    private bool countering = false;
    private bool aiming = false;
    private int aimedInd = -1;

    private bool attacking = false;

    private int reward;
    #endregion

    public void OnCancel(InputValue value)
    {
        ReturnToMain();
    }

    // Start is called before the first frame update
    void Start()
    {
        reward = 0;
        currCommand = Command.none;

        mainButtons = FindObjectsOfType<Button>();
        spellButtons = new Button[] { null };

        messagePanel.SetActive(false);

        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

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

            reward += currTroop.enemies[i].source.goldReward;
            stats.GetComponent<Button>().interactable = false;
        }
        enemyButtons = buttonList.ToArray();
        currTurn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (onMagic && EventSystem.current.currentSelectedGameObject != null)
        {
            RectTransform selected = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();
            int posOffset = scrollOffset * 80;
            if (selected.localPosition.y > (-40 - posOffset))
            {
                optionScroll.content.localPosition = new Vector3(optionScroll.content.localPosition.x,
                    optionScroll.content.localPosition.y - (selected.localPosition.y + 40 + posOffset), 0);
                scrollOffset--;
            }
            else if (selected.localPosition.y < (-280 - posOffset))
            {
                optionScroll.content.localPosition = new Vector3(optionScroll.content.localPosition.x,
                    optionScroll.content.localPosition.y - (selected.localPosition.y + 280 + posOffset), 0);
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
            GameController.singleton.UseSkill(-1);
            playerStats.text = "Dextra\n"
                + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
                + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
                + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

            guarding = false;
            countering = false;
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

        int i = 0;
        float threshold = 0;
        Attack? temp = null;
        float det = Random.Range(0.0f, 1.0f);
        Debug.Log(det);
        while (temp == null)
        {
            threshold += enemy.source.attacks[i].chance;
            if (det <= threshold)
            {
                temp = enemy.source.attacks[i];
            }
            else
            {
                i++;
            }
        }
        Attack attack = (Attack)temp;

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = attack.name;
        yield return new WaitForSeconds(0.75f);

        switch (attack.target)
        {
            case Attack.Target.player:
                Animator effectAnim = null;
                Animator[] animators = GameObject.Find("PlayerImage").GetComponentsInChildren<Animator>();
                foreach (Animator animator in animators)
                {
                    if (animator.gameObject.name.Equals("PlayerImage"))
                    {
                        // TODO: trigger damage animation
                    }
                    else
                    {
                        effectAnim = animator;
                        animator.SetBool(attack.effect, true);
                    }
                }
                yield return new WaitUntil(() => !effectAnim.GetBool("slash"));
                yield return new WaitForSeconds(0.1f);
                enemy.source.UseAttack(attack, guarding);
                break;

        if (GameController.singleton.GetHP() <= 0)
        {
            GameObject.Find("Player").GetComponent<AudioSource>().Play();
            playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You lose...";
            yield return new WaitForSeconds(1);
            GameController.singleton.Die();
        }
        else
        {
            playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";
            yield return new WaitForSeconds(0.75f);
            enemy.img.GetComponent<RectTransform>().anchoredPosition -= new Vector2(30, 0);
            if (countering)
            {
                StartCoroutine(AttackRtn(currTurn - 1, true));
                yield return new WaitUntil(() => !attacking);
            }
            messagePanel.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            NextTurn();
        }
    }

    public void Target(int enemyIndex)
    {
        // TODO: Add Attack Menu Confirm SFX Event

        switch (currCommand)
        {
            case Command.attack:
                StartCoroutine(AttackRtn(enemyIndex, false));
                break;

            case Command.magic:
                if (currSpell.spellType == Spell.SpellType.damage)
                {
                    StartCoroutine(DmgSpellRtn(currSpell, enemyIndex));
                }
                else
                {
                    StartCoroutine(DrainSpellRtn(currSpell, enemyIndex));
                }
                break;

            case Command.skill:
                if (currSkill.skillType == Skill.SkillType.damage)
                {
                    StartCoroutine(DmgSkillRtn(currSkill, enemyIndex));
                }
                else if (currSkill.skillType == Skill.SkillType.tripleDamage)
                {
                    StartCoroutine(TrpDmgSkillRtn(currSkill, enemyIndex));
                }
                else
                {
                    StartCoroutine(AimSkillRtn(currSkill, enemyIndex));
                }
                break;

            default:
                Debug.Log("ERROR: NO COMMAND REQUIRING TARGET FOUND");
                break;
        }

        if (spellButtons[0] != null)
        {
            foreach (Button button in spellButtons)
            {
                Destroy(button.gameObject);
            }
            optionPanel.SetActive(false);
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
    }

    private IEnumerator AttackRtn(int enemyIndex, bool counter)
    {
        attacking = true;
        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = countering ? "Counterattack!" : "Attack!";
        yield return new WaitForSeconds(0.75f);

        int str = GameController.singleton.GetStrength();

        bool won = false;
        bool crit = false;

        Animator effectAnim = null;
        Animator[] animators = currTroop.enemies[enemyIndex].img.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            if (animator.gameObject == currTroop.enemies[enemyIndex].img)
            {
                // TODO: trigger damage animation
            }
            else
            {
                effectAnim = animator;
                animator.SetBool("slash", true);
            }
        }
        yield return new WaitUntil(() => !effectAnim.GetBool("slash"));
        yield return new WaitForSeconds(0.1f);

        int dmg;
        if (aiming && aimedInd == enemyIndex)
        {
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Critical hit!";
            dmg = Mathf.FloorToInt(str * 1.25f + str * Random.Range(0.75f, 1.25f));
            crit = true;
            aiming = false;
        }
        else if (aiming && aimedInd != enemyIndex)
        {
            dmg = Mathf.FloorToInt(str * 0.75f);
            aiming = false;
        }
        else
        {
            dmg = Mathf.FloorToInt(str * Random.Range(0.75f, 1.25f));
            if (dmg == Mathf.FloorToInt(str * 1.25f))
            {
                messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Critical hit!";
                dmg += Mathf.FloorToInt(str * Random.Range(0.75f, 1.25f));
                crit = true;
            }
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
                won = true;
                Win();
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
        
        if (!counter && !won)
        {
            aiming = false;
            aimedInd = -1;
            messagePanel.SetActive(false);
            ReturnToMain();
            NextTurn();
        }

        attacking = false;
    }

    public void MagicCmd()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        scrollOffset = 0;
        optionPanel.SetActive(true);
        optionScroll.content.anchoredPosition = Vector2.zero;
        GameObject spell1 = null;
        int count = 0;

        List<Button> buttonList = new List<Button>();
        foreach (GameController.SpellStr spell in GameController.singleton.spellList)
        {
            if (spell.unlocked)
            {
                count++;
                GameObject spellButton = Instantiate(spellButtonPrefab, optionScroll.content);
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
        onMagic = true;
    }

    public void Cast(Spell spell)
    {
        if (spell.manaCost > GameController.singleton.GetMP())
        {
            StartCoroutine(OutOfMana());
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

                foreach (Button button in spellButtons)
                {
                    Destroy(button.gameObject);
                }
                optionPanel.SetActive(false);

                StartCoroutine(DmgAllSpellRtn(spell));
                break;

            case Spell.SpellType.heal:
                foreach (Button button in enemyButtons)
                {
                    button.interactable = false;
                }

                foreach (Button button in spellButtons)
                {
                    Destroy(button.gameObject);
                }
                optionPanel.SetActive(false);

                StartCoroutine(HealSpellRtn(spell));
                break;

            case Spell.SpellType.drain:
                currCommand = Command.magic;
                currSpell = spell;
                foreach (Button button in enemyButtons)
                {
                    button.interactable = true;
                }

                EventSystem.current.SetSelectedGameObject(enemy1);
                break;

            default:
                Debug.Log("ERROR: INVALID SPELL TYPE");
                break;
        }
    }

    private IEnumerator DmgSpellRtn(Spell spell, int enemyIndex)
    {
        GameController.singleton.Cast(spell.manaCost);
        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = spell.name;

        Animator effectAnim = null;
        Animator[] animators = currTroop.enemies[enemyIndex].img.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            if (animator.gameObject == currTroop.enemies[enemyIndex].img)
            {
                // TODO: trigger damage animation
            }
            else
            {
                effectAnim = animator;
                animator.SetBool(spell.effect, true);
            }
        }
        yield return new WaitUntil(() => !effectAnim.GetBool(spell.effect));
        yield return new WaitForSeconds(0.1f);

        int mag = GameController.singleton.GetMagic();

        bool won = false;

        int dmg = Mathf.FloorToInt((spell.baseAmt + mag) * Random.Range(1-spell.var, 1+spell.var));

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
                won = true;
                Win();
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
            aiming = false;
            aimedInd = -1;
            messagePanel.SetActive(false);
            ReturnToMain();
            NextTurn();
        }
    }

    private IEnumerator DmgAllSpellRtn(Spell spell)
    {
        GameController.singleton.Cast(spell.manaCost);
        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = spell.name;
        yield return new WaitForSeconds(0.75f);

        int mag = GameController.singleton.GetMagic();

        bool replaceE1 = false;

        List<Enemy> dead = new List<Enemy>();

        for (int i = 0; i < currTroop.enemies.Length; i++)
        {
            int dmg = Mathf.FloorToInt((spell.baseAmt + mag) * Random.Range(1-spell.var, 1+spell.var));

            Animator effectAnim = null;
            Animator[] animators = currTroop.enemies[i].img.GetComponentsInChildren<Animator>();
            foreach (Animator animator in animators)
            {
                if (animator.gameObject == currTroop.enemies[i].img)
                {
                    // TODO: trigger damage animation
                }
                else
                {
                    effectAnim = animator;
                    animator.SetBool(spell.effect, true);
                }
            }
            
            yield return new WaitUntil(() => !effectAnim.GetBool(spell.effect));

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
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (dead.ToArray().Length == currTroop.enemies.Length)
        {
            Win();
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
            
            for (int i = 0; i < enemyButtons.Length; i++)
            {
                int ind = i;
                enemyButtons[i].onClick.RemoveAllListeners();
                enemyButtons[i].onClick.AddListener(() => Target(ind));
            }

            if (replaceE1)
            {
                enemy1 = currTroop.enemies[0].stats;
            }

            aiming = false;
            aimedInd = -1;
            messagePanel.SetActive(false);
            ReturnToMain();
            NextTurn();
        }
    }

    private IEnumerator HealSpellRtn(Spell spell)
    {
        GameController.singleton.Cast(spell.manaCost);
        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = spell.name;

        GameObject playerImg = GameObject.Find("PlayerImage");
        Animator effectAnim = null;
        Animator[] animators = playerImg.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            if (animator.gameObject.name.Equals("Effects"))
            {
                effectAnim = animator;
                animator.SetBool(spell.effect, true);
            }
        }
        yield return new WaitUntil(() => !effectAnim.GetBool(spell.effect));
        yield return new WaitForSeconds(0.1f);

        int mag = GameController.singleton.GetMagic();

        int amt = Mathf.FloorToInt((spell.baseAmt + mag) * Random.Range(0.75f, 1.25f));

        GameController.singleton.Damage(-amt);

        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        aiming = false;
        aimedInd = -1;
        messagePanel.SetActive(false);
        ReturnToMain();
        NextTurn();
    }

    private IEnumerator DrainSpellRtn(Spell spell, int enemyIndex)
    {
        GameController.singleton.Cast(spell.manaCost);
        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = spell.name;

        Animator effectAnim = null;
        Animator[] animators = currTroop.enemies[enemyIndex].img.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            if (animator.gameObject == currTroop.enemies[enemyIndex].img)
            {
                // TODO: trigger damage animation
            }
            else
            {
                effectAnim = animator;
                animator.SetBool(spell.effect, true);
            }
        }
        yield return new WaitUntil(() => !effectAnim.GetBool(spell.effect));
        yield return new WaitForSeconds(0.1f);

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

            GameObject playerImg = GameObject.Find("PlayerImage");
            effectAnim = null;
            animators = playerImg.GetComponentsInChildren<Animator>();
            foreach (Animator animator in animators)
            {
                if (animator.gameObject.name.Equals("Effects"))
                {
                    effectAnim = animator;
                    animator.SetBool("sparkle", true);
                }
            }
            yield return new WaitUntil(() => !effectAnim.GetBool("sparkle"));
            yield return new WaitForSeconds(0.1f);

            int amt = Mathf.FloorToInt(dmg/2);

            GameController.singleton.Damage(-amt);

            playerStats.text = "Dextra\n"
                + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
                + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
                + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

            if (currTroop.enemies.Length < 1)
            {
                won = true;
                Win();
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
            aiming = false;
            aimedInd = -1;
            messagePanel.SetActive(false);
            ReturnToMain();
            NextTurn();
        }
    }

    private IEnumerator OutOfMana()
    {
        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Not enough MP!";
        yield return new WaitForSeconds(0.75f);
        messagePanel.SetActive(false);
    }

    public void SkillCmd()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        scrollOffset = 0;
        optionPanel.SetActive(true);
        optionScroll.content.anchoredPosition = Vector2.zero;
        GameObject spell1 = null;
        int count = 0;

        List<Button> buttonList = new List<Button>();
        foreach (GameController.SkillStr skill in GameController.singleton.skillList)
        {
            if (skill.unlocked)
            {
                count++;
                GameObject spellButton = Instantiate(spellButtonPrefab, optionScroll.content);
                spellButton.GetComponentInChildren<TextMeshProUGUI>().text = skill.skill.skillName + " [" + skill.skill.spCost + "]";
                spellButton.GetComponent<Button>().onClick.AddListener(() => UseSkill(skill.skill));
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
        onMagic = true;
    }

    public void UseSkill(Skill skill)
    {
        if (skill.spCost > GameController.singleton.GetSP())
        {
            StartCoroutine(OutOfSP());
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

        switch (skill.skillType)
        {
            case Skill.SkillType.damage:
                currCommand = Command.skill;
                currSkill = skill;
                foreach (Button button in enemyButtons)
                {
                    button.interactable = true;
                }

                EventSystem.current.SetSelectedGameObject(enemy1);
                break;

            case Skill.SkillType.tripleDamage:
                currCommand = Command.skill;
                currSkill = skill;
                foreach (Button button in enemyButtons)
                {
                    button.interactable = true;
                }

                EventSystem.current.SetSelectedGameObject(enemy1);
                break;

            case Skill.SkillType.aim:
                currCommand = Command.skill;
                currSkill = skill;
                foreach (Button button in enemyButtons)
                {
                    button.interactable = true;
                }

                EventSystem.current.SetSelectedGameObject(enemy1);
                break;

            case Skill.SkillType.guard:
                foreach (Button button in enemyButtons)
                {
                    button.interactable = false;
                }

                foreach (Button button in spellButtons)
                {
                    Destroy(button.gameObject);
                }
                optionPanel.SetActive(false);

                StartCoroutine(GuardSkillRtn(skill));
                break;

            case Skill.SkillType.focus:
                foreach (Button button in enemyButtons)
                {
                    button.interactable = false;
                }

                foreach (Button button in spellButtons)
                {
                    Destroy(button.gameObject);
                }
                optionPanel.SetActive(false);

                StartCoroutine(FocusSkillRtn(skill));
                break;

            case Skill.SkillType.counter:
                foreach (Button button in enemyButtons)
                {
                    button.interactable = false;
                }

                foreach (Button button in spellButtons)
                {
                    Destroy(button.gameObject);
                }
                optionPanel.SetActive(false);

                StartCoroutine(CounterSkillRtn(skill));
                break;

            default:
                Debug.Log("ERROR: INVALID SKILL TYPE");
                break;
        }
    }

    private IEnumerator DmgSkillRtn(Skill skill, int enemyIndex)
    {
        GameController.singleton.UseSkill(skill.spCost);
        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName;

        Animator effectAnim = null;
        Animator[] animators = currTroop.enemies[enemyIndex].img.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            if (animator.gameObject == currTroop.enemies[enemyIndex].img)
            {
                // TODO: trigger damage animation
            }
            else
            {
                effectAnim = animator;
                animator.SetBool(skill.effect, true);
            }
        }
        yield return new WaitUntil(() => !effectAnim.GetBool(skill.effect));
        yield return new WaitForSeconds(0.1f);

        int str = GameController.singleton.GetStrength();

        bool won = false;

        int dmg = Mathf.FloorToInt((skill.baseAmt + str) * Random.Range(1-skill.var, 1+skill.var));

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
                won = true;
                Win();
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
            aiming = false;
            aimedInd = -1;
            messagePanel.SetActive(false);
            ReturnToMain();
            NextTurn();
        }
    }

    private IEnumerator TrpDmgSkillRtn(Skill skill, int enemyIndex)
    {
        GameController.singleton.UseSkill(skill.spCost);
        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Triple Hit";

        Animator effectAnim = null;
        Animator[] animators = currTroop.enemies[enemyIndex].img.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            if (animator.gameObject == currTroop.enemies[enemyIndex].img)
            {
                // TODO: trigger damage animation
            }
            else
            {
                effectAnim = animator;
                animator.SetBool(skill.effect, true);
            }
        }
        yield return new WaitUntil(() => !effectAnim.GetBool(skill.effect));
        yield return new WaitForSeconds(0.1f);

        int str = GameController.singleton.GetStrength();

        bool won = false;

        int dmg = 0;
        int crits = 0;

        for (int i = 0; i < 3; i++)
        {
            int hitDmg = Mathf.FloorToInt(str * Random.Range(0.75f, 1.25f));
            dmg += hitDmg;
            if (hitDmg == Mathf.FloorToInt(str * 1.25f) || (aiming && aimedInd == enemyIndex))
            {
                dmg += Mathf.FloorToInt(str * Random.Range(0.75f, 1.25f));
                crits++;
            }
        }
        
        if (crits > 0)
        {
            if (crits == 1)
            {
                messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Critical hit!";
            }
            else if (crits == 2)
            {
                messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Double critical!!";
            }
            else if (crits >= 3)
            {
                messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Triple critical!!!";
            }
            yield return new WaitForSeconds(0.75f);
        }

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
                won = true;
                Win();
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
            aiming = false;
            aimedInd = -1;
            messagePanel.SetActive(false);
            ReturnToMain();
            NextTurn();
        }
    }

    private IEnumerator AimSkillRtn(Skill skill, int enemyIndex)
    {
        GameController.singleton.UseSkill(skill.spCost);
        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName;

        Animator effectAnim = null;
        Animator[] animators = currTroop.enemies[enemyIndex].img.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            if (animator.gameObject == currTroop.enemies[enemyIndex].img)
            {
                // TODO: trigger damage animation
            }
            else
            {
                effectAnim = animator;
                animator.SetBool(skill.effect, true);
            }
        }
        yield return new WaitUntil(() => !effectAnim.GetBool(skill.effect));
        yield return new WaitForSeconds(0.1f);

        aiming = true;
        aimedInd = enemyIndex;
        
        messagePanel.SetActive(false);
        ReturnToMain();
        NextTurn();
    }

    private IEnumerator GuardSkillRtn(Skill skill)
    {
        GameController.singleton.UseSkill(skill.spCost);
        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName;

        GameObject playerImg = GameObject.Find("PlayerImage");
        Animator effectAnim = null;
        Animator[] animators = playerImg.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            if (animator.gameObject.name.Equals("Effects"))
            {
                effectAnim = animator;
                animator.SetBool(skill.effect, true);
            }
        }
        yield return new WaitUntil(() => !effectAnim.GetBool(skill.effect));
        yield return new WaitForSeconds(0.1f);

        guarding = true;

        aiming = false;
        aimedInd = -1;
        messagePanel.SetActive(false);
        ReturnToMain();
        NextTurn();
    }

    private IEnumerator FocusSkillRtn(Skill skill)
    {
        GameController.singleton.UseSkill(skill.spCost);
        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName;

        GameObject playerImg = GameObject.Find("PlayerImage");
        Animator effectAnim = null;
        Animator[] animators = playerImg.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            if (animator.gameObject.name.Equals("Effects"))
            {
                effectAnim = animator;
                animator.SetBool(skill.effect, true);
            }
        }
        yield return new WaitUntil(() => !effectAnim.GetBool(skill.effect));
        yield return new WaitForSeconds(0.1f);

        GameController.singleton.Cast(-skill.baseAmt);
        
        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        aiming = false;
        aimedInd = -1;
        messagePanel.SetActive(false);
        ReturnToMain();
        NextTurn();
    }

    private IEnumerator CounterSkillRtn(Skill skill)
    {
        GameController.singleton.UseSkill(skill.spCost);
        playerStats.text = "Dextra\n"
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + " HP\n"
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP + " MP\n"
            + GameController.singleton.GetSP() + "/" + GameController.singleton.maxSP + " SP";

        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName;

        GameObject playerImg = GameObject.Find("PlayerImage");
        Animator effectAnim = null;
        Animator[] animators = playerImg.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            if (animator.gameObject.name.Equals("Effects"))
            {
                effectAnim = animator;
                animator.SetBool(skill.effect, true);
            }
        }
        yield return new WaitUntil(() => !effectAnim.GetBool(skill.effect));
        yield return new WaitForSeconds(0.1f);

        guarding = true;
        countering = true;

        aiming = false;
        aimedInd = -1;
        messagePanel.SetActive(false);
        ReturnToMain();
        NextTurn();
    }

    private IEnumerator OutOfSP()
    {
        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Not enough SP!";
        yield return new WaitForSeconds(0.75f);
        messagePanel.SetActive(false);
    }

    public void FleeCmd()
    {
        currCommand = Command.none;

        spellButtons = new Button[] { null };

        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        float det = Random.Range(0.0f, 1.0f);

        messagePanel.SetActive(true);

        if (det < fleeChance)
        {
            // TODO: Add Flee SFX Event
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
        // TODO: Add Flee Fail SFX Event
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You failed to escape!";
        yield return new WaitForSeconds(0.75f);
        NextTurn();
    }

    private void ReturnToMain()
    {
        // TODO: Add UI Back SFX Event

        onMagic = false;

        if (spellButtons[0] != null)
        {
            foreach (Button button in spellButtons)
            {
                Destroy(button.gameObject);
            }
            optionPanel.SetActive(false);
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
    }

    private void Win()
    {
        StartCoroutine(WinRtn());
    }

    private IEnumerator WinRtn()
    {
        // TODO: Add RPG Win SFX Event
        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You win!";
        yield return new WaitForSeconds(1);
        GameController.singleton.AddGold(reward);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You received " + reward + "G!";
        yield return new WaitForSeconds(0.5f);
        GameController.singleton.StartCoroutine(GameController.singleton.UnloadBattle());
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
    
    [Range(0, 1)] public float var;
    [Range(0, 1)] public float chance;

    public string anim;
    public string effect;

    public enum Target
    {
        player,
        self,
        ally,
        ownParty
    }

    public Target target;
}
