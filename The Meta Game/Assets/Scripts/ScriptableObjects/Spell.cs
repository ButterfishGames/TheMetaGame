﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "ScriptableObjects/Spell", order = 3)]
public class Spell : ScriptableObject
{
    public string spellName;

    public enum SpellType
    {
        damage,
        damageAll,
        heal,
        drain
    }
    public SpellType spellType;

    public enum DamageType
    {
        fire,
        ice,
        lightning,
        magic
    }
    public DamageType damageType;

    public int manaCost;

    public int baseAmt;

    [Range(0, 1)]
    public float var;
    
    public string anim;
    public string effect;
}
