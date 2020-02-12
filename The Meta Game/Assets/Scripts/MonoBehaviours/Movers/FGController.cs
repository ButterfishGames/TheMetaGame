using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Tooltip("Players max health")]
    public int maxHealth;

    private int health;

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

    private InputDirection[] specialRight = { InputDirection.right, InputDirection.rightDown, InputDirection.down };

    private InputDirection[] specialLeft = { InputDirection.left, InputDirection.leftDown, InputDirection.down };

    /// <summary>
    /// Bool to see if direction is being held so the input doesn't repeat every frame.
    /// Left [0], LeftDown [1], Down [2], RightDown [3], Right [4]
    /// </summary>
    private bool[] inputsHeld;

    /// <summary>
    /// Timer to clear the input buffer
    /// </summary>
    private float inputResetTimer;

    [Tooltip("The maximum time that is allowed to pass before the read inputs for the special move reset.")]
    public float maxTimeTillReset;

    [Tooltip("Special GameObject to instantiate")]
    public GameObject special;

    /// <summary>
    /// Different kinds of attacks the player can do.
    /// </summary>
    private enum Attack
    {
        light,
        medium,
        heavy,
        special
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
    [HideInInspector]public float hitstun;

    [Tooltip("How much hitstun you want to give to the enemy when a light attack is performed")]
    public float lightHitstun;

    [Tooltip("How much hitstun you want to give to the enemy when a medium attack is performed")]
    public float mediumHitstun;

    [Tooltip("How much hitstun you want to give to the enemy when a heavy attack is performed")]
    public float heavyHitstun;

    [Tooltip("How much damage you want to deal when a light attack is performed")]
    public int lightDamage;

    [Tooltip("How much hitstun you want to deal when a medium attack is performed")]
    public int mediumDamage;

    [Tooltip("How much hitstun you want to deal when a heavy attack is performed")]
    public int heavyDamage;

    /// <summary>
    /// Hit box to hit enemies
    /// </summary>
    private BoxCollider2D hitbox;

    /// <summary>
    /// float to determine the length at which the hitbox is out.
    /// </summary>
    private float hitBoxActivationTime;

    /// <summary>
    /// float to determine the length of time after the hitbox has dissapeared.
    /// </summary>
    private float endLagTime;

    /// <summary>
    /// To detect if the enemy has been hit on this frame because it picks up two instances for some reason
    /// </summary>
    private bool hitThisFrame;

    private bool[] hitConfirm;

    protected override void Start()
    {
        base.Start();

        health = maxHealth;

        inputs = new InputDirection[3];
        inputsHeld = new bool[5];
        hitConfirm = new bool[3];

        for (int i = 0; i < inputs.Length - 1; i++)
        {
            inputs[i] = InputDirection.none;
        }
        for (int i = 0; i < inputsHeld.Length - 1; i++)
        {
            inputsHeld[i] = false;
        }
        for (int i = 0; i < hitConfirm.Length - 1; i++)
        {
            hitConfirm[i] = false;
        }
        attacking = false;
        hitThisFrame = false;

        hitbox = transform.Find("Hitbox").GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        if(health <= 0)
        {
            GameController.singleton.Die();
            health = maxHealth; 
        }
        hitThisFrame = false;
        if (hitstun <= 0)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Move(h, v);

            if (inputResetTimer < maxTimeTillReset / 60)
            {
                inputResetTimer += Time.deltaTime;
            }
            else
            {
                inputs[0] = InputDirection.none;
                inputResetTimer = 0;
            }

            Vector3 viewPos = FindObjectOfType<Camera>().WorldToViewportPoint(transform.position);
            if (GameController.singleton.GetPaused() == false)
            {
                if (viewPos.y < 0.0f)
                {
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
        else
        {
            hitbox.gameObject.SetActive(false);
            attacking = false;
            hitstun -= Time.deltaTime;
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
            if (hitstun <= 0)
            {
                float moveX = h * moveSpeed * Time.deltaTime;
                float moveY = rb.velocity.y;

                rb.velocity = new Vector2(moveX, moveY);


                if (h < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    dir = Direction.left;
                }
                else if (h > 0)
                {
                    transform.rotation = Quaternion.Euler(Vector3.zero);
                    dir = Direction.right;
                }
            }
        }

        inputsHeld[0] = CheckIfStillHeld(inputsHeld[0], h < 0);
        inputsHeld[1] = CheckIfStillHeld(inputsHeld[1], h < 0 && v < 0);
        inputsHeld[2] = CheckIfStillHeld(inputsHeld[2], v < 0);
        inputsHeld[3] = CheckIfStillHeld(inputsHeld[3], h > 0 && v < 0);
        inputsHeld[4] = CheckIfStillHeld(inputsHeld[4], h > 0);

        if (h == 1 && v == -1)
        {
            inputsHeld[3] = NewDirectionAndHeld(InputDirection.rightDown, inputsHeld[3]);
        }
        else if (h == -1 && v == -1)
        {
            inputsHeld[1] = NewDirectionAndHeld(InputDirection.leftDown, inputsHeld[1]);
        }
        else if (h == 1)
        {
            inputsHeld[4] = NewDirectionAndHeld(InputDirection.right, inputsHeld[4]);
        }
        else if (h == -1)
        {
            inputsHeld[0] = NewDirectionAndHeld(InputDirection.left, inputsHeld[0]);
        }
        else if (v == -1)
        {
            inputsHeld[2] = NewDirectionAndHeld(InputDirection.down, inputsHeld[2]);
        }
    }

    private bool NewDirectionAndHeld(InputDirection input, bool held)
    {
        if (held == false)
        {
            held = true;
            for (int j = inputs.Length - 1; j > 0; j--)
            {
                inputs[j] = inputs[j - 1];
            }
            inputs[0] = input;
            inputResetTimer = 0;

            return held;
        }
        return held;
    }

    private bool CheckIfStillHeld(bool held, bool hvdirection)
    {
        if(hvdirection == false)
        {
            held = false;
            return held;
        }
        return held;
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
        else if (collision.CompareTag("EnemyHitbox"))
        {
            if (hitThisFrame == false) {
                hitThisFrame = true;
                hitstun = collision.GetComponent<FightingHitbox>().hitstun;
                health -= collision.GetComponent<FightingHitbox>().damage;
                if(collision.name == "SpecialMove(Clone)" && collision.CompareTag("EnemyHitbox"))
                {
                    Destroy(collision.gameObject);
                }
            }
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
        //if(dir == Direction.left)
        //{
        //    offsetX *= -1;
        //    offsetY *= -1;
        //}
        hitbox.offset = new Vector2(offsetX, offsetY);
        hitbox.size = new Vector2(sizeX, sizeY);
    }

    private void AttackEnemy()
    {
        if (attacking == false)
        {
            if ((Input.GetAxis("Light") > 0 || Input.GetAxis("Medium") > 0 || Input.GetAxis("Heavy") > 0) && 
                (inputs.SequenceEqual(specialRight) || inputs.SequenceEqual(specialLeft)))
            {
                if (dir == Direction.right)
                {
                    //if(hitConfirm[1] == true || hitConfirm[2] == true || hitConfirm[3] == true)
                    //{
                    //    for (int i = 0; i < hitConfirm.Length - 1; i++)
                    //    {
                    //        hitConfirm[i] = false;
                    //    }
                    //    StopCoroutine(AttackCoRoutine());
                    //    hitbox.gameObject.SetActive(false);
                    //}
                    attacking = true;
                    GameObject specialMove = Instantiate(special, new Vector3(transform.position.x + 1, transform.position.y, -2.0f), Quaternion.identity) as GameObject;
                    specialMove.tag = "PlayerHitbox";
                    inputs[0] = InputDirection.none;
                    attackType = Attack.special;
                }
                else
                {
                    attacking = true;
                    GameObject specialMove = Instantiate(special, new Vector3(transform.position.x - 1, transform.position.y, -2.0f), Quaternion.Euler(0, 180, 0)) as GameObject;
                    specialMove.tag = "PlayerHitbox";
                    inputs[0] = InputDirection.none;
                    attackType = Attack.special;
                }
            }
            else if (Input.GetAxis("Light") > 0)
            {
                BasicAttack(Attack.light, 0.2f, 0.3f, 0.0f, 0.0f, lightHitstun, lightDamage);
            }
            else if (Input.GetAxis("Medium") > 0)
            {
                BasicAttack(Attack.medium, 0.3f, 0.5f, 0.0f, 0.0f, mediumHitstun, mediumDamage);
            }
            else if (Input.GetAxis("Heavy") > 0)
            {
                BasicAttack(Attack.heavy, 0.4f, 0.7f, 0.0f, 0.0f, heavyHitstun, heavyDamage);
            }
        }
    }

    private void BasicAttack(Attack attack, float hitboxTime, float lag, float xVelocity,  float yVelocity, float hitstunGiven, int damage)
    {
        attackType = attack;
        hitBoxActivationTime = hitboxTime;
        endLagTime = lag;
        if (grounded)
        {
            rb.velocity = new Vector2(xVelocity, yVelocity);
        }
        hitbox.gameObject.GetComponent<FightingHitbox>().hitstun = hitstunGiven;
        hitbox.gameObject.GetComponent<FightingHitbox>().damage = damage;
        attacking = true;
    }

    private IEnumerator AttackCoRoutine()
    {
        if (grounded)
        {
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
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
            case Attack.special:
                break;
            default:
                Debug.Log("ERROR: INVALID STARTING ATTACK");
                break;
        }
        hitbox.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitBoxActivationTime);
        hitbox.gameObject.SetActive(false);
        yield return new WaitForSeconds(endLagTime);
        attacking = false;
        attackCoRoutineRunning = false;
    }
}
