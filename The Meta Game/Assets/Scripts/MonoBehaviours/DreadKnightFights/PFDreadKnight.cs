using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFDreadKnight : EnemyBehaviour
{
    #region variables
    [Tooltip("The speed at which enemies move")]
    public float moveSpeed;

    [Tooltip("The amount by which speed is multiplied when an enemy sees the player")]
    public float chargeMult;

    [Tooltip("The distance at which the enemy can see the player")]
    public float viewDist;

    public enum Direction
    {
        right,
        left
    };

    [Tooltip("The direction the enemy starts facing")]
    public Direction startDir;

    [Tooltip("The max HP enemies have in gamemodes where they can take damage")]
    public int maxHP;

    [Tooltip("The time in seconds it takes for the red flash when damaged to fade")]
    public float damageFadeTime;

    public bool test = false;

    /// <summary>
    /// Used to make damage flash calculations more efficient
    /// </summary>
    private float inverseDamageFadeTime;

    /// <summary>
    /// The current HP a given enemy has
    /// </summary>
    private int currHP;

    /// <summary>
    /// Used to determine current direction; 1 is right, -1 is left
    /// </summary>
    [HideInInspector] public int dir = 1;

    /// <summary>
    /// Reference to Rigidbody2D component on object
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Reference to BoxCollider2D component on object
    /// </summary>
    private BoxCollider2D col;

    /// <summary>
    /// Value speed is multiplied by to determine if enemy is charging or not
    /// </summary>
    float mult;

    private bool grounded;

    private bool turning;

    private bool inCutscene;

    /// <summary>
    /// How much time until Dread Knight charges
    /// </summary>
    private float chargePrepTime;

    /// <summary>
    /// The Dread Knights charge prep animation
    /// </summary>
    private Animation chargePrepAnimaton;

    private float timeToGetUp = 2.08f;
    private float timeGettingUp;

    private float currentChargeTime;

    public GameObject cutscene;

    public GameObject barriers;
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        inCutscene = true;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        chargePrepAnimaton = GetComponent<Animation>();
        chargePrepTime = chargePrepAnimaton.clip.length;
        switch (startDir)
        {
            case Direction.right:
                dir = 1;
                break;

            case Direction.left:
                dir = -1;
                break;

            default:
                Debug.Log("ERROR: INVALID STARTING DIRECTION");
                break;
        }

        mult = 1;

        currHP = maxHP;

        inverseDamageFadeTime = 1.0f / damageFadeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.singleton.GetPaused())
        {
            return;
        }

        if (!grounded)
        {
            return;
        }

        if (!Camera.main.GetComponent<CameraScroll>().enabled)
        {
            if(cutscene != null)
            {
                Destroy(cutscene.gameObject);
            }
            if (currHP > 0)
            {
                barriers.SetActive(true);
                inCutscene = false;
            }
            else
            {
                barriers.SetActive(false);
                inCutscene = true;
            }
        }

        if (inCutscene)
        {
            animator.SetBool("cutscene", true);
        }
        if (!inCutscene)
        {
            animator.SetBool("cutscene", false);
            if (timeGettingUp <= 0)
            {
                animator.SetBool("hitWall", false);
                RaycastHit2D hit;
                Vector2 dVec = new Vector2(dir * 0.5f, -1).normalized;
                LayerMask mask = ~((1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("Enemy2")) + (1 << LayerMask.NameToLayer("Bounds")) + (1 << LayerMask.NameToLayer("DamageFloor")) + (1 << LayerMask.NameToLayer("Player")));

                hit = Physics2D.Raycast(transform.position, dVec, 20, mask);

                Debug.DrawRay(transform.position, dVec, Color.red);
                Debug.DrawRay(gameObject.transform.position, -Vector3.right, Color.green);

                if (hit.collider == null)
                {
                    if (test)
                    {
                        Debug.Log("null");
                    }
                    Debug.Log("Turning 1");
                    Turn();
                }
                else
                {
                    if (test)
                    {
                        Debug.Log(hit.collider.name);
                    }
                    if (!animator.GetBool("chargePrep") && !animator.GetBool("charging"))
                    {
                        currentChargeTime = chargePrepAnimaton.clip.length;
                    }

                        Vector3 origin = transform.position;
                        dVec = new Vector2(dir, 0);
                        origin.y -= 0.3f;
                        hit = Physics2D.Raycast(origin, dVec, viewDist, ~(1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Enemy2")));
                        if (hit.collider != null && hit.collider.CompareTag("Player") && !animator.GetBool("charging"))
                        {
                            animator.SetBool("chargePrep", true);
                            currentChargeTime -= Time.deltaTime;
                        }
                        else
                        {
                            animator.SetBool("chargePrep", false);
                        }
                        if (currentChargeTime <= 0)
                        {
                            animator.SetBool("moving", false);
                            animator.SetBool("charging", true);
                            animator.SetBool("chargePrep", false);
                            mult = chargeMult;
                        }
                        if (!animator.GetBool("chargePrep"))
                        {
                            rb.velocity = new Vector2(dir * moveSpeed * mult, rb.velocity.y);
                        }
                    
                }
                if (!turning)
                {
                    if (rb.velocity.x > 0 && !animator.GetBool("charging"))
                    {
                        animator.SetBool("moving", true);
                        if (transform.eulerAngles.y == 180)
                        {
                            transform.eulerAngles = new Vector3(0, 180, 0);
                        }
                    }
                    else if (rb.velocity.x < 0 && !animator.GetBool("charging"))
                    {
                        animator.SetBool("moving", true);
                        if (transform.eulerAngles.y == 0)
                        {
                            transform.eulerAngles = new Vector3(0, 0, 0);
                        }
                    }
                    else
                    {
                        animator.SetBool("moving", false);
                    }
                }

                if (turning)
                {
                    turning = false;
                }
            }
            else
            {
                timeGettingUp -= Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currHP > 0)
        {
            if (collision.collider.CompareTag("Player"))
            {
                if (collision.collider.gameObject.GetComponent<PFController>().enabled)
                {
                    rb.velocity = Vector2.zero;
                    PFController pfCon = collision.collider.GetComponent<PFController>();
                    GetComponent<AudioSource>().Play();
                    pfCon.StartCoroutine(pfCon.Die(true));
                }
            }
        }

        if (collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("Turning 3");
            Turn();
        }

        if (collision.collider.CompareTag("Ground"))
        {
            grounded = true;
        }

        if (collision.collider.CompareTag("Bound"))
        {
            if (animator.GetBool("charging"))
            {
                animator.SetBool("charging", false);
                animator.SetBool("hitWall", true);
                Hit(1);
                timeGettingUp = timeToGetUp;
                rb.velocity = new Vector2(-dir, 0.5f);
            }
            else
            {
                Turn();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!grounded && collision.collider.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Killbox"))
        {
            Destroy(gameObject);
        }
    }

    private void Turn()
    {
        turning = true;

        animator.SetBool("charging", false);
        dir *= -1;
        mult = 1;
        transform.Rotate(0, 180, 0);
    }

    public void Hit(int damage)
    {
        currHP -= damage;
        StartCoroutine(DamageFlash());
        if (currHP <= 0)
        {
            Debug.Log("Defeated Dread Knight");
        }
    }

    private IEnumerator DamageFlash()
    {
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        renderer.color = Color.red;
        Color temp = renderer.color;
        while (renderer.color.g < 1 || renderer.color.b < 1)
        {
            temp.g += inverseDamageFadeTime * Time.deltaTime;
            temp.b = temp.g;
            renderer.color = temp;
            yield return new WaitForEndOfFrame();
        }
    }

}
