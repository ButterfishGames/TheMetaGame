using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGController : Mover
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

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        Vector3 viewPos = FindObjectOfType<Camera>().WorldToViewportPoint(transform.position);
        if (GameController.singleton.GetPaused() == false)
        {
            if (viewPos.y < 0.0f) {
                GameController.singleton.Die();
            }
        }

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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (grounded && collision.CompareTag("Ground"))
        {
            grounded = false;
        }
    }
}
