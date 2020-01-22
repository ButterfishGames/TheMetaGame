// This script is based heavily on the scripts presented in the Unit Mechanics section of the Unity Learn 2D Roguelike tutorial.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMover : Mover
{
    [Tooltip("The layer on which normal level collision will be checked")]
    public LayerMask blockingLayer;

    [Tooltip("The layer on which collision with camera bounds will be checked")]
    public LayerMask boundLayer;

    [Tooltip("The time it will take to move the object, in seconds")]
    [Range(0, 1)]
    public float moveTime = 0.1f;

    /// <summary>
    /// Used to make movement more efficient
    /// </summary>
    private float inverseMoveTime;

    /// <summary>
    /// Used to check if the player is already moving before attempting to move again
    /// </summary>
    private bool moving;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        inverseMoveTime = 1.0f / moveTime;
        moving = false;
    }

    protected override void Move(float h, float v)
    {
        if (moving)
        {
            return;
        }
        else
        {
            moving = true;
        }

        if (h < 0)
        {
            h = -1;
            v = 0;
        }
        else if (h > 0)
        {
            h = 1;
            v = 0;
        }
        else if (v < 0)
        {
            v = -1;
        }
        else if (v > 0)
        {
            v = 1;
        }

        h *= 0.5f;
        v *= 0.5f;

        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(h, v);

        col.enabled = false;
        RaycastHit2D hit;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        col.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
        }
        else
        {
            moving = false;
        }
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        end.z = 1;
        float dist = (transform.position - end).sqrMagnitude;
        
        while (dist > 1E-10)
        {
            Vector3 newPos = Vector3.MoveTowards(rb.position, end, inverseMoveTime * Time.deltaTime);
            rb.MovePosition(newPos);
            dist = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        moving = false;
    }
}
