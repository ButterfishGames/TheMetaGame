using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "ScriptableObjects/RPG Enemy", order = 2)]
public class RPGEnemy : ScriptableObject
{
    new public string name;

    public Sprite sprite;
    public int maxHP;
    public Attack[] attacks;
    public int goldReward;

    private void OnDestroy()
    {
        Debug.Log(goldReward);
    }

    public void UseAttack(Attack attack, bool guarding)
    {
        int damage = attack.baseDmg;
        damage = Mathf.FloorToInt(damage * Random.Range(1 - attack.var, 1 + attack.var));
        if (guarding)
        {
            damage = Mathf.FloorToInt(damage / 2);
        }
        GameController.singleton.Damage(damage);
    }
}
