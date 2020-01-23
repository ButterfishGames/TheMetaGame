﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGController : Mover
{
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

    /// <summary>
    /// Used to check if the player is currently on a damage floor
    /// </summary>
    public bool onDamageFloor;

    /// <summary>
    /// Used to store directions to be passed to Smooth Movement
    /// </summary>
    private enum Direction
    {
        up,
        down,
        left,
        right
    };

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        inverseMoveTime = 1.0f / moveTime;
        moving = false;
        onDamageFloor = false;
    }

    protected override void Move(float h, float v)
    {
        Direction? dir = null;

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
            h = -0.5f;
            v = 0;
            dir = Direction.left;
        }
        else if (h > 0)
        {
            h = 0.5f;
            v = 0;
            dir = Direction.right;
        }
        else if (v < 0)
        {
            v = -0.5f;
            dir = Direction.down;
        }
        else if (v > 0)
        {
            v = 0.5f;
            dir = Direction.up;
        }

        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(h, v);

        col.enabled = false;
        RaycastHit2D hit;
        hit = Physics2D.Linecast(start, end, blockingLayer + boundLayer);
        col.enabled = true;

        if (hit.transform == null && dir != null)
        {
            StartCoroutine(SmoothMovement(end, (Direction)dir));
        }
        else
        {
            moving = false;
        }
    }

    // This method is based heavily on the scripts presented in the Unit Mechanics section of the Unity Learn 2D Roguelike tutorial.
    private IEnumerator SmoothMovement(Vector3 end, Direction dir)
    {
        end.z = 1;
        float dist = (transform.position - end).sqrMagnitude;
        
        while (dist > 1E-10)
        {
            bool damage = onDamageFloor;
            Vector3 newPos = dist < 0.01f ? end : Vector3.MoveTowards(rb.position, end, inverseMoveTime * Time.deltaTime);
            rb.MovePosition(newPos);
            if (dist <= 0.1f)
            {
                if (damage)
                {
                    GameController.singleton.FloorDamage();
                    rb.MovePosition(end);
                    yield return new WaitForSeconds(GameController.singleton.damageFadeTime);
                    break;
                }

                Vector3 target;
                switch (dir)
                {
                    case Direction.right:
                        if (Input.GetAxisRaw("Horizontal") > 0)
                        {
                            target = end;
                            target.x += 0.5f;

                            col.enabled = false;
                            RaycastHit2D hit;
                            hit = Physics2D.Linecast(transform.position, target, blockingLayer + boundLayer);
                            col.enabled = true;

                            if (hit.transform == null)
                            {
                                end = target;
                            }
                        }
                        break;

                    case Direction.left:
                        if (Input.GetAxisRaw("Horizontal") < 0)
                        {
                            target = end;
                            target.x -= 0.5f;

                            col.enabled = false;
                            RaycastHit2D hit;
                            hit = Physics2D.Linecast(transform.position, target, blockingLayer + boundLayer);
                            col.enabled = true;

                            if (hit.transform == null)
                            {
                                end = target;
                            }
                        }
                        break;

                    case Direction.up:
                        if (Input.GetAxisRaw("Vertical") > 0)
                        {
                            target = end;
                            target.y += 0.5f;

                            col.enabled = false;
                            RaycastHit2D hit;
                            hit = Physics2D.Linecast(transform.position, target, blockingLayer + boundLayer);
                            col.enabled = true;

                            if (hit.transform == null)
                            {
                                end = target;
                            }
                        }
                        break;

                    case Direction.down:
                        if (Input.GetAxisRaw("Vertical") < 0)
                        {
                            target = end;
                            target.y -= 0.5f;

                            col.enabled = false;
                            RaycastHit2D hit;
                            hit = Physics2D.Linecast(transform.position, target, blockingLayer + boundLayer);
                            col.enabled = true;

                            if (hit.transform == null)
                            {
                                end = target;
                            }
                        }
                        break;

                    default:
                        Debug.Log("ERROR: INVALID DIRECTION PASSED TO SMOOTHMOVEMENT COROUTINE");
                        break;
                }
            }
            dist = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        moving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DamageFloor"))
        {
            onDamageFloor = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("DamageFloor") && !onDamageFloor)
        {
            onDamageFloor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("DamageFloor"))
        {
            onDamageFloor = false;
        }
    }
}
