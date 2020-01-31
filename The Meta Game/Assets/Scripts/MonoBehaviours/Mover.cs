using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{
    [Tooltip("The layer on which normal level collision will be checked")]
    public LayerMask blockingLayer;

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
        if (GameController.singleton.GetPaused())
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            Move(h, v);
        }
    }

    protected abstract void Move(float h, float v);
}
