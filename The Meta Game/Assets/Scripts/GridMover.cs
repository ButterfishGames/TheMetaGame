// This script is based heavily on the scripts presented in the Unit Mechanics section of the Unity Learn 2D Roguelike tutorial.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMover : Mover
{
    [Tooltip("The time it will take to move the object, in seconds")]
    [Range(0, 1)]
    public float moveTime = 0.1f;

    /// <summary>
    /// Used to make movement more efficient
    /// </summary>
    private float inverseMoveTime;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        base.Start();

        inverseMoveTime = 1.0f / moveTime;
    }

    protected override void Move(float h, float v)
    {

    }
}
