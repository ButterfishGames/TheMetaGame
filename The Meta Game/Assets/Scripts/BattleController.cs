using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleController : MonoBehaviour
{
    public TextMeshProUGUI playerStats;

    public GameObject enemyStatPrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerStats.text = "Player\n" + GameController.singleton.GetHP() + "/" + GameController.singleton.maxHP + "\n";
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

    }

    public void FleeCmd()
    {

    }
}
