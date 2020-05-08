using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGDreadKnight : EnemyBehaviour
{
    #region variables
    [Tooltip("Cutscene for after the boss fight")]
    public GameObject secondCutscene;
    private bool hitTrigger;
    private bool fightEnded;
    #endregion

    void Start()
    {
        hitTrigger = false;
        fightEnded = false;
    }

    private void Update()
    {
        if (!GameController.singleton.GetPaused() && hitTrigger == true)
        {
            fightEnded = true;
        }
        if (fightEnded)
        {
            secondCutscene.SetActive(true);
            Destroy(GameObject.Find("Killbox"));
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Player" && hitTrigger == false)
        {
            StartCoroutine(WaitToFight());
        }
    }

    private IEnumerator WaitToFight()
    {
        yield return new WaitUntil(() => !CutsceneManager.singleton.scening);
        hitTrigger = true;
        GameController.singleton.SetPaused(true);
        GameController.singleton.SpecBattle(12);
    }
}
