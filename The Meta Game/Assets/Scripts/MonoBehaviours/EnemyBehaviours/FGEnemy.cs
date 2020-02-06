using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGEnemy : EnemyBehaviour
{
    /// <summary>
    /// Sprite renderer to see if the enemy is visible or not so we know whether to include them in the fight.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Fighting variable to determine whether the enemy should be fighting the player ot not.
    /// </summary>
    private bool fighting;

    /// <summary>
    /// Players X position so the sprite can face towardsthe player depending on what side they are on.
    /// </summary>
    private float playerPosX;

    public enum Direction
    {
        right,
        left
    };

    /// <summary>
    /// To determine where the enemy should face too look at the player 
    /// </summary>
    private Direction facingDirection;

    [Tooltip("The speed at which the enemy moves")]
    public float speed;

    [Tooltip("The max HP enemies have in gamemodes where they can take damage")]
    public int maxHP;

    [Tooltip("The time in seconds it takes for the red flash when damaged to fade")]
    public float damageFadeTime;

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
    [HideInInspector] public int dir;

    /// <summary>
    /// Reference to Rigidbody2D component on object
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Reference to BoxCollider2D component on object
    /// </summary>
    private BoxCollider2D col;

    /// <summary>
    /// Main Camera to tell if enemy is within the viewport
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// This variable is to determine whether the enemy started within the view when switching to fighting mode
    /// </summary>
    [HideInInspector]public bool changedInView;

    [Tooltip("How difficult the enemy AI is")]
    [Range(1,3)]
    public int difficultyLevel;

    /// <summary>
    /// How long until the enemy switches it's state
    /// </summary>
    private float secondsUntilStateSwitch;

    private float stateSwitchTime;

    [Tooltip("Max time until the enemy switches their state")]
    public float maxSecondsUntilStateSwitch;

    [Tooltip("Minimum time until the enemy switches their state")]
    public float minSecondsUntilStateSwitch;

    /// <summary>
    /// Int to randomly select a different state to go to
    /// </summary>
    private int randomInt;

    private bool grounded;

    private bool test = false;

    /// <summary>
    /// Different enemy behaviors
    /// </summary>
    private enum EnemyState
    {
        offense,
        neutral,
        defense
    };

    /// <summary>
    /// What EnemyState is currently active
    /// </summary>
    private EnemyState state;

    [HideInInspector]public float hitstun;

    private bool attacking;

    private BoxCollider2D hitbox;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<Camera>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        state = EnemyState.neutral;
        hitbox = transform.Find("hitbox").GetComponent<BoxCollider2D>();

        currHP = maxHP;

        inverseDamageFadeTime = 1.0f / damageFadeTime;
    }

    void Update()
    {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        if (changedInView == true) {
            fighting = true;
            if (viewPos.y < 0.0f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            fighting = false;
        }
        if (fighting == true)
        {
            playerPosX = GameObject.FindGameObjectWithTag("Player").gameObject.transform.position.x;
            if (transform.position.x > playerPosX)
            {
                facingDirection = Direction.left;
            }
            else
            {
                facingDirection = Direction.right;
            }
            switch (facingDirection)
            {
                case Direction.right:
                    if (dir == -1)
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    dir = 1;
                    break;
                case Direction.left:
                    if (dir == 1)
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);
                    }
                    dir = -1;
                    break;
                default:
                    Debug.Log("ERROR: INVALID DIRECTION");
                    break;
            }

            if (stateSwitchTime < secondsUntilStateSwitch)
            {
                stateSwitchTime += Time.deltaTime;
            }
            else
            {
                if (hitstun <= 0)
                {
                    if (grounded)
                    {
                        randomInt = Random.Range(1, 2);
                        switch (state)
                        {
                            case EnemyState.defense:
                                state = ChooseRandomState(EnemyState.neutral, EnemyState.offense);
                                break;
                            case EnemyState.neutral:
                                state = ChooseRandomState(EnemyState.defense, EnemyState.offense);
                                break;
                            case EnemyState.offense:
                                state = ChooseRandomState(EnemyState.defense, EnemyState.neutral);
                                break;
                            default:
                                Debug.Log("ERROR: STATE DOES NOT EXIST");
                                break;
                        }
                        secondsUntilStateSwitch = Random.Range(minSecondsUntilStateSwitch, maxSecondsUntilStateSwitch);
                        stateSwitchTime = 0;
                    }
                }
            }

            if (hitstun <= 0)
            {
                RaycastHit2D hit;
                Vector2 dVec = new Vector2(dir, -1).normalized;
                LayerMask mask = ~((1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("Enemy2")) + (1 << LayerMask.NameToLayer("Bounds")) + (1 << LayerMask.NameToLayer("DamageFloor")) + (1 << LayerMask.NameToLayer("Player")));

                hit = Physics2D.Raycast(transform.position, dVec, 0.4f, mask);

                if (hit.collider == null)
                {
                    if (test)
                    {
                        Debug.Log("null");
                    }

                }
                else
                {
                    if (test)
                    {
                        Debug.Log(hit.collider.name);
                    }
                    dVec = new Vector2(dir, 0);
                    hit = Physics2D.Raycast(transform.position, dVec, 0.25f, mask);
                    if (hit.collider != null)
                    {

                    }
                    else
                    {
                        hit = Physics2D.Raycast(transform.position, dVec, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Enemy")));
                        if (hit.collider != null && hit.collider.CompareTag("Player"))
                        {

                        }
                        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
                    }
                }

                switch (state)
                {
                    case EnemyState.defense:
                        switch (difficultyLevel)
                        {
                            case 1:
                                if (facingDirection == Direction.right)
                                {

                                }
                                else if (facingDirection == Direction.left)
                                {

                                }
                                break;
                            case 2:
                                if (facingDirection == Direction.right)
                                {

                                }
                                else if (facingDirection == Direction.left)
                                {

                                }
                                break;
                            case 3:
                                if (facingDirection == Direction.right)
                                {

                                }
                                else if (facingDirection == Direction.left)
                                {

                                }
                                break;
                            default:
                                Debug.Log("ERROR: LEVEL DOES NOT EXIST");
                                break;
                        }
                        break;
                    case EnemyState.neutral:
                        switch (difficultyLevel)
                        {
                            case 1:
                                if (facingDirection == Direction.right)
                                {

                                }
                                else if (facingDirection == Direction.left)
                                {

                                }
                                break;
                            case 2:
                                if (facingDirection == Direction.right)
                                {

                                }
                                else if (facingDirection == Direction.left)
                                {

                                }
                                break;
                            case 3:
                                if (facingDirection == Direction.right)
                                {

                                }
                                else if (facingDirection == Direction.left)
                                {

                                }
                                break;
                            default:
                                Debug.Log("ERROR: LEVEL DOES NOT EXIST");
                                break;
                        }
                        break;
                    case EnemyState.offense:
                        switch (difficultyLevel)
                        {
                            case 1:
                                if (facingDirection == Direction.right)
                                {

                                }
                                else if (facingDirection == Direction.left)
                                {

                                }
                                break;
                            case 2:
                                if (facingDirection == Direction.right)
                                {

                                }
                                else if (facingDirection == Direction.left)
                                {

                                }
                                break;
                            case 3:
                                if (facingDirection == Direction.right)
                                {

                                }
                                else if (facingDirection == Direction.left)
                                {

                                }
                                break;
                            default:
                                Debug.Log("ERROR: LEVEL DOES NOT EXIST");
                                break;
                        }
                        break;
                    default:
                        Debug.Log("ERROR: STATE DOES NOT EXIST");
                        break;
                }
            }
        }
        else
        {
            hitbox.gameObject.SetActive(false);
            attacking = false;
            hitstun -= Time.deltaTime;
        }
        GetComponent<PFEnemy>().dir = dir;
    }

    private EnemyState ChooseRandomState(EnemyState state1, EnemyState state2)
    {
        if (randomInt == 1)
        {
            return state1;
        }
        else if (randomInt == 2)
        {
            return state2;
        }
        else
        {
            return EnemyState.neutral;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            grounded = true;
        }
        else if (collision.CompareTag("PlayerHitbox"))
        {
            //hitstun = collision.GetComponent<>();
            //health -= collision.GetComponent<>();
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
