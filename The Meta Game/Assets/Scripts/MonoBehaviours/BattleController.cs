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

    public GameObject messagePanel;

    public GameObject magicPanel;

    public GameObject attackButton;

    public Transform enemyCharSpace;

    public Transform enemyStatPanel;

    public ScrollRect magicScroll;

    public Troop[] troops;

    private Troop currTroop;

    private GameObject enemy1;

    private int currTurn;

    private bool onMain;

    // Start is called before the first frame update
    void Start()
    {
        messagePanel.SetActive(false);

        playerStats.text = "Player\n" 
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + "\n" 
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP;

        currTroop = troops[Random.Range(0, troops.Length)];

        foreach(Enemy enemy in currTroop.enemies)
        {
            GameObject img = Instantiate(enemyImgPrefab, enemyCharSpace);
            img.GetComponent<Image>().sprite = enemy.sprite;
            enemy.SetImg(img);

            GameObject stats = Instantiate(enemyStatPrefab, enemyStatPanel);
            enemy.SetStats(stats);
            stats.GetComponent<Button>().onClick.AddListener(() => Attack(enemy));

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

        if (magicPanel.activeInHierarchy)
        {
            magicScroll.content.localPosition = magicScroll.GetSnapToPositionToBringChildIntoView(
                EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>());
        }
    }

    private void NextTurn()
    {
        currTurn++;
        if (currTurn > currTroop.enemies.Length)
        {
            currTurn = 0;
        }

        Button[] buttons = FindObjectsOfType<Button>();

        if (currTurn == 0)
        {
            foreach (Button button in buttons)
            {
                button.interactable = true;
                ReturnToMain();
            }
        }
        else
        {
            foreach (Button button in buttons)
            {
                button.interactable = false;
            }
            StartCoroutine(EnemyTurn(currTroop.enemies[currTurn-1]));
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

    public void AttackCmd()
    {
        EventSystem.current.SetSelectedGameObject(enemy1);
        onMain = false;
    }

    public void Attack(Enemy enemy)
    {
        StartCoroutine(AttackRtn(enemy));
    }

    public IEnumerator AttackRtn(Enemy enemy)
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
        magicPanel.SetActive(true);
    }

    public void ItemCmd()
    {
        GameController.singleton.ErrDisp("Error: BattleController.cs does not contain a definition for 'ItemCmd'");
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
    }

    private void ReturnToMain()
    {
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
