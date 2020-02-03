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

    public void UseAttack(Attack attack)
    {
        int damage = attack.baseDmg;
        damage = Mathf.FloorToInt(damage * Random.Range(1 - attack.var, 1 + attack.var));
        GameController.singleton.Damage(damage);
    }
}
