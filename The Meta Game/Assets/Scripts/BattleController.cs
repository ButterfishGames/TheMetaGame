using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BattleController : MonoBehaviour
{
    public TextMeshProUGUI playerStats;

    public GameObject enemyImgPrefab;

    public GameObject enemyStatPrefab;

    public Transform enemyCharSpace;

    public Transform enemyStatPanel;

    public Troop[] troops;

    // Start is called before the first frame update
    void Start()
    {
        playerStats.text = "Player\n" 
            + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + "\n" 
            + GameController.singleton.GetMP() + "/" + GameController.singleton.maxMP;

        Troop troop = troops[Random.Range(0, troops.Length)];

        foreach(Enemy enemy in troop.enemies)
        {
            GameObject img = Instantiate(enemyImgPrefab, enemyCharSpace);
            img.GetComponent<Image>().sprite = enemy.sprite;

            GameObject stats = Instantiate(enemyStatPrefab, enemyStatPanel);
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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AttackCmd()
    {

    }

    public void MagicCmd()
    {

    }

    public void ItemCmd()
    {
        GameController.singleton.ErrDisp("Error: BattleController.cs does not contain a definition for 'ItemCmd'");
    }

    public void FleeCmd()
    {

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

    public Enemy()
    {
        currHP = maxHP;
    }

    public void UseAttack(int index)
    {
        int damage = attacks[index].baseDmg;
        damage = Mathf.FloorToInt(damage * Random.Range(attacks[index].var, 2 - attacks[index].var));
        Debug.Log(damage);
    }

    public int GetHP()
    {
        return currHP;
    }

    public void SetHP(int val)
    {
        currHP = val;
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
