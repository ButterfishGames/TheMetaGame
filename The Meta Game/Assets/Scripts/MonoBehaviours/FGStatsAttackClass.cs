using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FGStatsAttackClass
{
    [Tooltip("Name of the attack")]
    public string attackName;

    [Tooltip("How much hitstun you want to give to the player when this attack is performed in frames")]
    public float hitstun;

    [Tooltip("How much damage you want to deal when this attack is performed")]
    public int damage;

    [Tooltip("How long the startup of the move is in frames")]
    public float startup;

    [Tooltip("How long the hitbox is active in frames")]
    public float hitboxActivationTime;

    [Tooltip("How much lag the move has after the hitbox has ended in frames")]
    public float moveLag;

    [Tooltip("How much x velocity")]
    public float xVelocity;

    [Tooltip("How much y velocity")]
    public float yVelocity;

    [Tooltip("How much the hitbox is offset in the x direction")]
    public float offsetX;

    [Tooltip("How much the hitbox is offset in the y direction")]
    public float offsetY;

    [Tooltip("Hitbox size in the x direction")]
    public float sizeX;

    [Tooltip("Hitbox size in the y direction")]
    public float sizeY;
}
