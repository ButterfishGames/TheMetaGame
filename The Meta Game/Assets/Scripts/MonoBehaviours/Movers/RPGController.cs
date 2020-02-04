using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGController : Mover
{
    [Tooltip("The layer on which collision with camera bounds will be checked")]
    public LayerMask boundLayer;

    [Tooltip("The layer on which collision with enemies will be checked")]
    public LayerMask enemyLayer;

    [Tooltip("The time it will take to move the object, in seconds")]
    [Range(0, 1)]
    public float moveTime = 0.1f;

    [Tooltip("Movement coroutine for stopping from GameController. DO NOT EDIT, HANDLED BY CODE")]
    public IEnumerator mvmtCoroutine;

    [Tooltip("How many steps on average the player will go before encountering a battle")]
    public int encRate;

    [Tooltip("How many steps can you go with 0 chance of encounter after a battle")]
    public int encGrace;

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
    private bool onDamageFloor;

    private bool encountering;

    /// <summary>
    /// Used to store current direction facing
    /// </summary>
    private enum Direction
    {
        up,
        down,
        left,
        right
    };

    /// <summary>
    /// Stores current direction facing
    /// </summary>
    private Direction dir;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        inverseMoveTime = 1.0f / moveTime;
        moving = false;
        onDamageFloor = false;
        encountering = false;

        dir = Direction.down;
    }

    protected override void Update()
    {
        base.Update();

        if (!DialogueManager.singleton.GetDisplaying() && Input.GetButtonUp("Submit"))
        {
            Interact();
        }
    }

    protected override void Move(float h, float v)
    {
        if (moving || encountering)
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
        else
        {
            return;
        }

        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(h, v);
        
        RaycastHit2D hit;
        hit = Physics2D.Linecast(start, end, blockingLayer + boundLayer + enemyLayer);

        if (hit.transform == null)
        {
            mvmtCoroutine = SmoothMovement(end);
            EncCheck();
            StartCoroutine(mvmtCoroutine);
        }
        else
        {
            moving = false;
        }
    }

    // This method is based heavily on the scripts presented in the Unit Mechanics section of the Unity Learn 2D Roguelike tutorial.
    private IEnumerator SmoothMovement(Vector3 end)
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
                    rb.MovePosition(end);
                    yield return new WaitForSeconds(0.1f);
                    if (onDamageFloor)
                    {
                        GameController.singleton.FloorDamage();
                        yield return new WaitForSeconds(GameController.singleton.damageFadeTime);
                    }
                    break;
                }

                Vector3 target;
                if (!encountering)
                {
                    switch (dir)
                    {
                        case Direction.right:
                            if (Input.GetAxisRaw("Horizontal") > 0)
                            {
                                target = end;
                                target.x += 0.5f;

                                RaycastHit2D hit;
                                hit = Physics2D.Linecast(transform.position, target, blockingLayer + boundLayer + enemyLayer);

                                if (hit.transform == null)
                                {
                                    EncCheck();
                                    end = target;
                                }
                            }
                            break;

                        case Direction.left:
                            if (Input.GetAxisRaw("Horizontal") < 0)
                            {
                                target = end;
                                target.x -= 0.5f;

                                RaycastHit2D hit;
                                hit = Physics2D.Linecast(transform.position, target, blockingLayer + boundLayer + enemyLayer);

                                if (hit.transform == null)
                                {
                                    EncCheck();
                                    end = target;
                                }
                            }
                            break;

                        case Direction.up:
                            if (Input.GetAxisRaw("Vertical") > 0)
                            {
                                target = end;
                                target.y += 0.5f;

                                RaycastHit2D hit;
                                hit = Physics2D.Linecast(transform.position, target, blockingLayer + boundLayer + enemyLayer);

                                if (hit.transform == null)
                                {
                                    EncCheck();
                                    end = target;
                                }
                            }
                            break;

                        case Direction.down:
                            if (Input.GetAxisRaw("Vertical") < 0)
                            {
                                target = end;
                                target.y -= 0.5f;

                                RaycastHit2D hit;
                                hit = Physics2D.Linecast(transform.position, target, blockingLayer + boundLayer + enemyLayer);

                                if (hit.transform == null)
                                {
                                    EncCheck();
                                    end = target;
                                }
                            }
                            break;

                        default:
                            Debug.Log("ERROR: INVALID DIRECTION PASSED TO SMOOTHMOVEMENT COROUTINE");
                            break;
                    }
                }
            }
            dist = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        moving = false;
    }

    private void Interact()
    {
        Vector2 dVec = Vector3.zero;
        switch (dir)
        {
            case Direction.right:
                dVec = Vector2.right;
                break;

            case Direction.left:
                dVec = Vector2.left;
                break;

            case Direction.up:
                dVec = Vector2.up;
                break;

            case Direction.down:
                dVec = Vector2.down;
                break;

            default:
                Debug.Log("ERROR: INVALID DIRECTION PASSED TO INTERACT METHOD");
                break;
        }

        Vector3 target = transform.position + (Vector3)dVec;
        RaycastHit2D hit;
        hit = Physics2D.Linecast(transform.position, target, enemyLayer);

        if (hit.collider != null)
        {
            hit.collider.GetComponent<NPC>().Interact();
        }
    }

    private void EncCheck()
    {
        int det = Random.Range(0, encRate + 1);
        if (det == encRate)
        {
            GameController.singleton.SetPaused(true);
            encountering = true;
            GameController.singleton.StartCoroutine(GameController.singleton.Battle());
        }
    }

    public void SetMoving(bool val)
    {
        moving = val;
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

    public void SetEncountering (bool val)
    {
        encountering = val;
    }
}
