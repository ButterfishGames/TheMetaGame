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

    [Tooltip("Players health")]
    public float health;

    /// <summary>
    /// Tracks whether the player is currently on the ground
    /// </summary>
    private bool grounded;

    /// <summary>
    /// Used to store current direction facing
    /// </summary>
    private enum Direction
    {
        left,
        right
    };

    private enum Attack
    {
        light,
        medium,
        heavy
    };

    /// <summary>
    /// Stores current direction facing
    /// </summary>
    private Direction dir;

    /// <summary>
    /// Stores current attack
    /// </summary>
    private Attack attackType;

    /// <summary>
    /// A bool to see whether the player is using an attack
    /// </summary>
    private bool attacking;

    /// <summary>
    /// A float to determine how much hitstun the player should have when hit
    /// </summary>
    private float hitstun;

    /// <summary>
    /// Hit box to hit enemies
    /// </summary>
    private BoxCollider2D hitbox;

    protected override void Start()
    {
        base.Start();

        hitbox = transform.Find("Hitbox").GetComponent<BoxCollider2D>();
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

        if (attacking)
        {
            switch (attackType)
            {
                case Attack.light:

                    break;

                case Attack.medium:

                    break;

                case Attack.heavy:

                    break;

                default:
                    break;
            }
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

        if(h < 0)
        {
            dir = Direction.left;
        }
        else if (h > 0)
        {
            dir = Direction.left;
        }
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

    private void HitBoxSize()
    {
        
    }
}
