using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFController : Mover
{
    [Tooltip("The rate at which the object accelerates")]
    public float moveSpeed;

    [Tooltip("The maximum speed at which the object may move")]
    public float maxVelX;

    [Tooltip("The vertical force applied to the object when it jumps (includes wall jumping)")]
    public float jumpForce;

    [Tooltip("The horizontal force applied to the object when it wall jumps")]
    public float wallJumpForce;

    [Tooltip("The force applied for death animation")]
    public float deathForce;

    [Tooltip("The time in seconds for which the game waits after death by enemy before reloading")]
    public float deathWait;

    private Animator animator;

    /// <summary>
    /// Tracks whether the player is currently on the ground
    /// </summary>
    private bool grounded;

    private bool onWall;

    private int wallDir;

    private BoxCollider2D groundTrigger;

    protected override void Start()
    {
        base.Start();

        groundTrigger = null;
        BoxCollider2D[] cols = GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D col in cols)
        {
            if (col.name.Equals("GroundTrigger"))
            {
                groundTrigger = col;
            }
        }

        onWall = false;
        animator = GetComponentInChildren<Animator>();
    }

    protected override void Update()
    {
        if (GameController.singleton.GetPaused())
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") < 0)
        {
            StopCoroutine("GoThrough");
            StartCoroutine("GoThrough");
        }

        if (Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
        {
            Jump();
        }
    }

    private void LateUpdate()
    {
        if (GameController.singleton.GetPaused())
        {
            return;
        }

        base.Update();

        bool onGround = false;

        if (groundTrigger != null)
        {
            List<Collider2D> contacts = new List<Collider2D>();
            groundTrigger.GetContacts(contacts);
            foreach (Collider2D contact in contacts)
            {
                if (contact.CompareTag("Ground"))
                {
                    onGround = true;
                }
            }
        }

        if (onGround)
        {
            float distX = groundTrigger.bounds.extents.x;
            float distY = groundTrigger.bounds.extents.y;
            Vector3 origin, origin2;
            origin = origin2 = transform.position;
            origin.x -= distX;
            origin2.x += distX;
            RaycastHit2D hit, hit2;

            hit = Physics2D.Raycast(origin, Vector2.down, 0.4f * transform.localScale.y,
                ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));
            hit2 = Physics2D.Raycast(origin2, Vector2.down, 0.4f * transform.localScale.y,
                ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));

            if ((hit.collider == null || !hit.collider.CompareTag("Ground")) && (hit2.collider == null || !hit2.collider.CompareTag("Ground")))
            {
                onGround = false;
            }

            if (!onGround)
            {
                origin = origin2 = transform.position;
                origin.y += distY;
                origin2.y -= distY;
                RaycastHit2D hit3, hit4;
                hit = Physics2D.Raycast(origin, Vector2.right, 0.4f * transform.localScale.x,
                    ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));
                hit2 = Physics2D.Raycast(origin2, Vector2.right, 0.4f * transform.localScale.x,
                    ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));
                hit3 = Physics2D.Raycast(origin, Vector2.left, 0.4f * transform.localScale.x,
                    ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));
                hit4 = Physics2D.Raycast(origin2, Vector2.left, 0.4f * transform.localScale.x,
                    ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));

                if ((hit.collider != null && hit.collider.CompareTag("Ground")) || (hit2.collider != null && hit2.collider.CompareTag("Ground")))
                {
                    onWall = true;
                    wallDir = 1;
                    transform.rotation = Quaternion.Euler(0, 90 + (wallDir * 90), 0);
                }
                else if ((hit3.collider != null && hit3.collider.CompareTag("Ground")) || (hit4.collider != null && hit4.collider.CompareTag("Ground")))
                {
                    onWall = true;
                    wallDir = -1;
                    transform.rotation = Quaternion.Euler(0, 90 + (wallDir * 90), 0);
                }
                else
                {
                    onWall = false;
                }
            }
        }

        if (onGround)
        {
            onWall = false;
            animator.SetBool("grounded", true);
            animator.SetBool("walled", false);
            animator.SetBool("jumping", false);
            grounded = true;
        }
        else if (!onWall)
        {
            animator.SetBool("grounded", false);
            animator.SetBool("walled", false);
            grounded = false;
        }
        else
        {
            animator.SetBool("jumping", false);
            if (Input.GetAxisRaw("Horizontal") == wallDir)
            {
                animator.SetBool("walled", true);
            }
            else
            {
                animator.SetBool("walled", false);
            }
        }

        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    protected override void Move(float h, float v)
    {
        float moveX = Input.GetAxisRaw("Horizontal") == 0 ?
            (grounded ? h * moveSpeed * Time.deltaTime : rb.velocity.x) :
            (grounded ? Mathf.Clamp(rb.velocity.x + (h * moveSpeed * Time.deltaTime), -maxVelX, maxVelX) : 
            Mathf.Clamp(rb.velocity.x + (h * (moveSpeed/10) * Time.deltaTime), -maxVelX, maxVelX));
        float moveY = rb.velocity.y;
        if (moveX < 0 && !onWall)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            animator.SetBool("moving", true);
        }
        else if (moveX > 0 && !onWall)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
            animator.SetBool("moving", true);
        }

        rb.velocity = new Vector2(moveX, moveY);
    }

    private void Jump()
    {
        if (!grounded)
        {
            return;
        }

        animator.SetBool("grounded", false);
        animator.SetBool("walled", false);
        animator.SetBool("jumping", true);
        rb.velocity = new Vector2(rb.velocity.x, 0);

        float xForce = 0;
        Debug.Log(onWall);
        if (onWall)
        {
            xForce = -wallDir * wallJumpForce;
            Debug.Log(xForce);
        }

        rb.AddForce(new Vector2(xForce, jumpForce), ForceMode2D.Impulse);
        onWall = false;
        grounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            animator.SetBool("jumping", false);
            grounded = true;
        }
        else if (collision.CompareTag("Killbox"))
        {
            GameController.singleton.Die();
        }
    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if (GetComponent<FGController>().enabled == false) {
            if (!grounded && collision.CompareTag("Ground"))
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.3f, blockingLayer);

                if (hit.collider != null && hit.collider.CompareTag("Ground"))
                {
                    grounded = true;
                }
            }
        }
    }*/

    public IEnumerator Die()
    {
        GameController.singleton.SetPaused(true);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1);
        rb.AddForce(Vector2.up * deathForce, ForceMode2D.Impulse);
        col.enabled = false;
        GetComponentInChildren<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(deathWait);
        GameController.singleton.Die();
    }

    private IEnumerator GoThrough()
    {
        gameObject.layer = LayerMask.NameToLayer("ThroughTemp");
        yield return new WaitForSeconds(0.25f);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
}
