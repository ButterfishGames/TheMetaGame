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

    /// <summary>
    /// Different inputs to find out if the special move has been performed
    /// </summary>
    private enum InputDirection
    {
        right,
        rightDown,
        down,
        leftDown,
        left,
        none
    };

    /// <summary>
    /// Used to store inputs done by the player to determine if the special move should happen.
    /// </summary>
    private InputDirection[] inputs;

    private InputDirection[] specialRight = { InputDirection.down, InputDirection.rightDown, InputDirection.right };

    private InputDirection[] specialLeft = { InputDirection.down, InputDirection.leftDown, InputDirection.left };

    public GameObject special;

    /// <summary>
    /// Different kinds of attacks the player can do.
    /// </summary>
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

    private bool attackCoRoutineRunning;

    /// <summary>
    /// A float to determine how much hitstun the player should have when hit
    /// </summary>
    private float hitstun;

    /// <summary>
    /// Hit box to hit enemies
    /// </summary>
    private BoxCollider2D hitbox;

    /// <summary>
    /// float to determine the length at which the hitbox is out.
    /// </summary>
    private float hitBoxActivationTime;

    protected override void Start()
    {
        base.Start();

        inputs = new InputDirection[3];

        for (int i = 0; i < inputs.Length - 1; i++)
        {
            inputs[i] = InputDirection.none;
        }
        attacking = false;

        hitbox = transform.Find("Hitbox").GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        Debug.Log(inputs[0]);
        Debug.Log(inputs[1]);
        Debug.Log(inputs[2]);
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

        AttackEnemy();

        if (attacking)
        {
            if (!attackCoRoutineRunning)
            {
                attackCoRoutineRunning = true;
                StartCoroutine(AttackCoRoutine());
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
            {
                Jump();
            }

            hitbox.gameObject.SetActive(false);
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
        if (!attacking)
        {
            float moveX = h * moveSpeed * Time.deltaTime;
            float moveY = rb.velocity.y;

            rb.velocity = new Vector2(moveX, moveY);


            if (h < 0)
            {
                dir = Direction.left;
            }
            else if (h > 0)
            {
                dir = Direction.right;
            }
        }

        for (int j = inputs.Length - 1; j > 1; j--)
        {
            inputs[j] = inputs[j-1];
        }
        
        if(h > 0 && v < 0)
        {
            inputs[0] = InputDirection.rightDown;
        }
        else if(h < 0 && v < 0)
        {
            inputs[0] = InputDirection.leftDown;
        }
        else if (h > 0)
        {
            inputs[0] = InputDirection.right;
        }
        else if (h < 0)
        {
            inputs[0] = InputDirection.left;
        }
        else if (v < 0)
        {
            inputs[0] = InputDirection.down;
        }
        else
        {
            inputs[0] = InputDirection.none;
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

    private void HitBoxSizeAndPos(float offsetX, float offsetY, float sizeX, float sizeY)
    {
        if(dir == Direction.left)
        {
            offsetX *= -1;
            offsetY *= -1;
        }
        hitbox.offset = new Vector2(offsetX, offsetY);
        hitbox.size = new Vector2(sizeX, sizeY);
    }

    private void AttackEnemy()
    {
        if (attacking == false)
        {
            if ((Input.GetAxis("Light") > 0 || Input.GetAxis("Medium") > 0 || Input.GetAxis("Heavy") > 0) && 
                (inputs.Equals(specialRight) || inputs.Equals(specialRight)))
            {
                if (dir == Direction.right)
                {
                    Instantiate<GameObject>(special, new Vector2(transform.position.x + 1, transform.position.y), Quaternion.identity);
                }
                else
                {
                    Instantiate<GameObject>(special, new Vector2(transform.position.x - 1, transform.position.y), Quaternion.identity);
                }
            }
            else if (Input.GetAxis("Light") > 0)
            {
                Debug.Log("Light");
                attackType = Attack.light;
                hitBoxActivationTime = 1;
                attacking = true;
            }
            else if (Input.GetAxis("Medium") > 0)
            {
                Debug.Log("Medium");
                attackType = Attack.medium;
                hitBoxActivationTime = 1;
                attacking = true;
            }
            else if (Input.GetAxis("Heavy") > 0)
            {
                Debug.Log("Heavy");
                attackType = Attack.heavy;
                hitBoxActivationTime = 1;
                attacking = true;
            }
        }
    }

    private IEnumerator AttackCoRoutine()
    {
        switch (attackType)
        {
            case Attack.light:
                HitBoxSizeAndPos(0.65f, 0.0f, 0.5f, 0.5f);
                break;
            case Attack.medium:
                HitBoxSizeAndPos(1.0f, 0.0f, 1.0f, 0.5f);
                break;
            case Attack.heavy:
                HitBoxSizeAndPos(1.0f, 0.0f, 2.0f, 0.5f);
                break;
            default:
                Debug.Log("ERROR: INVALID STARTING ATTACK");
                break;
        }
        hitbox.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitBoxActivationTime);
        attacking = false;
        attackCoRoutineRunning = false;
    }
}
