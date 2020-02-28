using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private int grace;

    private int chance;

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

    private bool canEnc;

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

    protected override void OnEnable()
    {
        base.OnEnable();

        controls.UI.Submit.performed += InteractHandle;

        controls.UI.Submit.Enable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        controls.UI.Submit.performed -= InteractHandle;

        controls.UI.Submit.Disable();
    }

    private void InteractHandle(InputAction.CallbackContext context)
    {
        if (GameController.singleton.GetPaused())
        {
            return;
        }

        if (DialogueManager.singleton.GetDisplaying())
        {
            return;
        }

        Interact();
    }

    // Start is called before the first frame update
    private void Start()
    {
        inverseMoveTime = 1.0f / moveTime;
        moving = false;
        onDamageFloor = false;
        encountering = false;
        canEnc = true;
        grace = encGrace;
        chance = encRate;

        dir = Direction.down;
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
            transform.rotation = Quaternion.identity;

            h = -1;
            v = 0;
            dir = Direction.left;
            animator.SetInteger("dir", 3);
        }
        else if (h > 0)
        {
            transform.rotation = Quaternion.identity;

            h = 1;
            v = 0;
            dir = Direction.right;
            animator.SetInteger("dir", 2);
        }
        else if (v < 0)
        {
            v = -1;
            dir = Direction.down;
            animator.SetInteger("dir", 0);
        }
        else if (v > 0)
        {
            v = 1;
            dir = Direction.up;
            animator.SetInteger("dir", 1);
        }
        else
        {
            return;
        }

        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(h, v);
        
        RaycastHit2D hit;
        hit = Physics2D.Linecast(start, end * 1.49f, blockingLayer + boundLayer + enemyLayer);

        if (hit.transform == null)
        {
            mvmtCoroutine = SmoothMovement(end);
            StartCoroutine(mvmtCoroutine);
            animator.SetBool("moving", true);
            canEnc = true;
        }
        else
        {
            moving = false;
            animator.SetBool("moving", false);
        }
    }

    // This method is based heavily on the scripts presented in the Unit Mechanics section of the Unity Learn 2D Roguelike tutorial.
    private IEnumerator SmoothMovement(Vector3 end)
    {
        float dist = (transform.position - end).sqrMagnitude;
        
        while (dist > 1E-10)
        {
            bool damage = onDamageFloor;
            Vector3 newPos = dist < 0.01f ? end : Vector3.MoveTowards(rb.position, end, inverseMoveTime * Time.deltaTime);
            rb.MovePosition(newPos);
            if (dist <= 0.1f)
            {
                if (!encountering && canEnc)
                {
                    EncCheck();
                    canEnc = false;
                }

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
                Vector3 lcTarget;
                if (!encountering)
                {
                    switch (dir)
                    {
                        case Direction.right:
                            if (hor > 0)
                            {
                                target = end;
                                target.x += 1;
                                lcTarget = target * 1.49f;

                                RaycastHit2D hit;
                                hit = Physics2D.Linecast(transform.position, lcTarget, blockingLayer + boundLayer + enemyLayer);

                                if (hit.transform == null)
                                {
                                    end = target;
                                    canEnc = true;
                                }
                            }
                            break;

                        case Direction.left:
                            if (hor < 0)
                            {
                                target = end;
                                target.x -= 1f;
                                lcTarget = target * 1.49f;

                                RaycastHit2D hit;
                                hit = Physics2D.Linecast(transform.position, lcTarget, blockingLayer + boundLayer + enemyLayer);

                                if (hit.transform == null)
                                {
                                    end = target;
                                    canEnc = true;
                                }
                            }
                            break;

                        case Direction.up:
                            if (ver > 0)
                            {
                                target = end;
                                target.y += 1f;
                                lcTarget = target * 1.49f;

                                RaycastHit2D hit;
                                hit = Physics2D.Linecast(transform.position, lcTarget, blockingLayer + boundLayer + enemyLayer);

                                if (hit.transform == null)
                                {
                                    end = target;
                                    canEnc = true;
                                }
                            }
                            break;

                        case Direction.down:
                            if (ver < 0)
                            {
                                target = end;
                                target.y -= 1f;
                                lcTarget = target * 1.49f;

                                RaycastHit2D hit;
                                hit = Physics2D.Linecast(transform.position, lcTarget, blockingLayer + boundLayer + enemyLayer);

                                if (hit.transform == null)
                                {
                                    end = target;
                                    canEnc = true;
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
        animator.SetBool("moving", false);
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
        Debug.Log("test");

        if (grace > 0)
        {
            grace--;
            return;
        }

        int det = Random.Range(0, chance + 1);
        if (det == chance)
        {
            hRaw = vRaw = hor = ver = 0;

            GameController.singleton.SetPaused(true);
            encountering = true;
            grace = encGrace;
            chance = encRate;
            GameController.singleton.StartCoroutine(GameController.singleton.Battle());
        }
        else
        {
            chance--;
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
