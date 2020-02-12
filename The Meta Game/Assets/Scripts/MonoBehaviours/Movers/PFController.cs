using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFController : Mover
{
    [Tooltip("The rate at which the object moves")]
    public float moveSpeed;

    [Tooltip("The force applied to the object when it jumps")]
    public float jumpForce;

    [Tooltip("The force applied for death animation")]
    public float deathForce;

    [Tooltip("The time in seconds for which the game waits after death by enemy before reloading")]
    public float deathWait;

    /// <summary>
    /// Tracks whether the player is currently on the ground
    /// </summary>
    private bool grounded;

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
        bool onWall = false;

        if (groundTrigger != null)
        {
            List<Collider2D> contacts = new List<Collider2D>();
            groundTrigger.GetContacts(contacts);
            foreach (Collider2D contact in contacts)
            {
                Debug.Log(contact.tag);
                if (contact.CompareTag("Ground"))
                {
                    onGround = true;
                }
            }
        }

        if (onGround)
        {
            float distX = groundTrigger.bounds.extents.x;
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

            hit = Physics2D.Raycast(transform.position, Vector2.right, 0.4f * transform.localScale.x,
                ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                onWall = true;
            }

            hit = Physics2D.Raycast(transform.position, Vector2.left, 0.4f * transform.localScale.x,
                ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                onWall = true;
            }
        }

        if (onGround)
        {
            grounded = true;
        }
        else if (!onWall)
        {
            grounded = false;
        }
    }

    protected override void Move(float h, float v)
    {
        float moveX = h * moveSpeed * Time.deltaTime;
        float moveY = rb.velocity.y;
        if (moveX < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (moveX > 0)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        rb.velocity = new Vector2(moveX, moveY);
    }

    private void Jump()
    {
        if (!grounded)
        {
            return;
        }

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        grounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
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
