using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFEnemy : EnemyBehaviour
{
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
    [HideInInspector]public int dir = 1;

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

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        switch(startDir)
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

        animator = GetComponentInChildren<Animator>();
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

        RaycastHit2D hit;
        Vector2 dVec = new Vector2(dir *0.5f, -1).normalized;
        LayerMask mask = ~((1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("Enemy2")) + (1 << LayerMask.NameToLayer("Bounds")) + (1 << LayerMask.NameToLayer("DamageFloor")) + (1 << LayerMask.NameToLayer("Player")));

        hit = Physics2D.Raycast(transform.position, dVec, 2, mask);

        if (hit.collider == null)
        {
            if (test)
            {
                Debug.Log("null");
            }
            Turn();
        }
        else
        {
            if (test)
            {
                Debug.Log(hit.collider.name);
            }
            dVec = new Vector2(dir, 0);
            hit = Physics2D.Raycast(transform.position, dVec, 0.5f, mask);
            if (hit.collider != null)
            {
                Turn();
            }
            else
            {
                Vector3 origin = transform.position;
                origin.y -= 0.3f;
                hit = Physics2D.Raycast(origin, dVec, viewDist, ~(1 << LayerMask.NameToLayer("Enemy")));
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    animator.SetBool("charging", true);
                    mult = chargeMult;
                }
                rb.velocity = new Vector2(dir * moveSpeed * mult, rb.velocity.y);
            }
        }

        if (rb.velocity.x > 0)
        {
            animator.SetBool("moving", true);
            if (transform.eulerAngles.y == 180) {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
        else if (rb.velocity.x < 0)
        {
            animator.SetBool("moving", true);
            if (transform.eulerAngles.y == 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        else
        {
            animator.SetBool("moving", false);
        }

        GetComponent<FGEnemy>().dir = dir;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (collision.collider.gameObject.GetComponent<PFController>().enabled) {
                rb.velocity = Vector2.zero;
                PFController pfCon = collision.collider.GetComponent<PFController>();
                GetComponent<AudioSource>().Play();
                pfCon.StartCoroutine(pfCon.Die(true));
            }
        }

        if (collision.collider.CompareTag("Enemy"))
        {
            Turn();
        }

        if (collision.collider.CompareTag("Ground"))
        {
            grounded = true;
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
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageFlash()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
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
