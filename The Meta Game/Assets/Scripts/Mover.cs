// This script is based heavily on the scripts presented in the Unit Mechanics section of the Unity Learn 2D Roguelike tutorial.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{
    /// <summary>
    /// The BoxCollider2D component attached to this object
    /// </summary>
    protected BoxCollider2D col;

    /// <summary>
    /// The Rigidbody2D component attached to this object
    /// </summary>
    protected Rigidbody2D rb;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            Move(h, v);
        }
    }

    protected abstract void Move(float h, float v);
}
