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

    protected override void Update()
    {
        if (GameController.singleton.GetPaused())
        {
            return;
        }

        if (Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (GameController.singleton.GetPaused())
        {
            return;
        }

        base.Update();
    }

    protected override void Move(float h, float v)
    {
        float moveX = h * moveSpeed * Time.deltaTime;
        float moveY = rb.velocity.y;

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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (enabled) {
            if (!grounded && collision.CompareTag("Ground"))
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.3f, blockingLayer);

                if (hit.collider != null && hit.collider.CompareTag("Ground"))
                {
                    grounded = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (grounded && collision.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

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
}
