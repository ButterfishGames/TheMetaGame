/* This script is based heavily on the scripts presented in the Unit Mechanics
 * section of the Unity Learn 2D Roguelike tutorial.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMover : MonoBehaviour
{
    [Tooltip("The time it will take to move the object, in seconds")]
    [Range(0, 1)]
    public float moveTime = 0.1f;

    [Tooltip("The layer on which collision will be checked")]
    public LayerMask blockingLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
