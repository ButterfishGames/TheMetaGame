using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "ScriptableObjects/Spell", order = 1)]
public class Spell : ScriptableObject
{
    public string spellName;

    public enum SpellType
    {
        damage,
        damageAll,
        heal
    }
    public SpellType spellType;

    public enum DamageType
    {
        fire,
        ice,
        lightning
    }
    public DamageType damageType;

    public int baseAmt;

    [Range(0, 1)]
    public float var;
}
