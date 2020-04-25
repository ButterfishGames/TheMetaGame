using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "ScriptableObjects/Skill", order = 3)]
public class Skill : ScriptableObject
{
    public string skillName;

    public enum SkillType
    {
        damage,
        tripleDamage,
        guard,
        aim,
        focus,
        counter
    }

    public SkillType skillType;

    public enum DamageType
    {
        slash,
        pierce,
        blunt
    }

    public DamageType damageType;

    public int spCost;

    public int baseAmt;

    [Range(0, 1)]
    public float var;

    public string anim;
    public string effect;
}
