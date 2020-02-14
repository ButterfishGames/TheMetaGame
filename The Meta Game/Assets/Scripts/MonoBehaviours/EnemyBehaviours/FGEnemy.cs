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
    private GameObject player;

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
    [HideInInspector] public bool changedInView;

    [Tooltip("How difficult the enemy AI is")]
    [Range(1, 3)]
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

    /// <summary>
    /// Is the enmy on the ground
    /// </summary>
    private bool grounded;

    /// <summary>
    /// To detect if the enemy has been hit on this frame because it picks up two instances for some reason
    /// </summary>
    private bool hitThisFrame;

    [Tooltip("How much force you want the enemy to have in their jump")]
    public float jumpForce;

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

    /// <summary>
    /// bool that checks if enemy is attacking
    /// </summary>
    private bool attacking;

    /// <summary>
    /// Enemies hitbox to hit the player with
    /// </summary>
    private BoxCollider2D hitbox;

    /// <summary>
    /// float offset for the rays that check if there is a cliff
    /// </summary>
    public float xOffsetForRay;

    private Vector3 v3Offset;

    /// <summary>
    /// bool to see if the enmy will move right or left
    /// </summary>
    private bool startRight;

    /// <summary>
    /// bool to check if the enemy is in the neutral state
    /// </summary>
    private bool inNeutral;

    /// <summary>
    /// float to see how long the enemy has been moving in one direction while in neutral
    /// </summary>
    private float secondsInOneDirection;

    /// <summary>
    /// float to determine the new max time to move in a direction while in neutral randomly
    /// </summary>
    private float randomTime;

    /// <summary>
    /// bool to see if a random time has been chosen
    /// </summary>
    private bool choseTime;

    /// <summary>
    /// Different kinds of attacks the enemy can do.
    /// </summary>
    private enum Attack
    {
        light,
        medium,
        heavy,
        special
    };

    [Tooltip("Special GameObject to instantiate")]
    public GameObject special;

    /// <summary>
    /// float to determine the length at which the hitbox is out.
    /// </summary>
    private float startupTime;

    /// <summary>
    /// float to determine the length at which the hitbox is out.
    /// </summary>
    private float hitBoxActivationTime;

    /// <summary>
    /// float to determine the length of time after the hitbox has dissapeared.
    /// </summary>
    private float endLagTime;

    /// <summary>
    /// Stores current attack
    /// </summary>
    private Attack attackType;

    private bool attackCoRoutineRunning;

    public FGStatsAttackClass lightAttackStats;
    public FGStatsAttackClass mediumAttackStats;
    public FGStatsAttackClass heavyAttackStats;

    private bool[] usedAttack;


    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<Camera>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        state = EnemyState.neutral;
        hitbox = transform.Find("EnemyHitbox").GetComponent<BoxCollider2D>();
        hitThisFrame = false;
        v3Offset = new Vector3(xOffsetForRay,0,0);
        attackCoRoutineRunning = false;
        usedAttack = new bool[3];
        for (int i = 0; i < usedAttack.Length - 1; i++)
        {
            usedAttack[i] = false;
        }

        currHP = maxHP;

        inverseDamageFadeTime = 1.0f / damageFadeTime;
    }

    void Update()
    {
        hitThisFrame = false;
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        if (changedInView == true) {
            fighting = true;
            if (viewPos.y < 0.0f)
            {
                Destroy(gameObject);
            }
            if(currHP <= 0)
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
            player = GameObject.FindGameObjectWithTag("Player");
             if (transform.position.x > player.transform.position.x)
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
                if (!attacking)
                {
                    stateSwitchTime += Time.deltaTime;
                }
            }
            else
            {
                if (hitstun <= 0)
                {

                        randomInt = Random.Range(1, 3);
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

            if (hitstun <= 0)
            {
                //dVecF = Front, dVecB = Back, offset and facing down
                if (attacking == false)
                {
                    hitbox.enabled = false;
                }
                else
                {
                    hitbox.enabled = true;
                }
                RaycastHit2D hitF;
                RaycastHit2D hitB;
                Vector2 dVec = new Vector2(0, - 1).normalized;
                LayerMask mask = ~((1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("Enemy2")) + (1 << LayerMask.NameToLayer("Bounds")) + (1 << LayerMask.NameToLayer("DamageFloor")) + (1 << LayerMask.NameToLayer("Player")));

                hitF = Physics2D.Raycast(transform.position + (v3Offset * dir), dVec, 0.5f, mask);
                hitB = Physics2D.Raycast(transform.position - (v3Offset * dir), dVec, 0.5f, mask);
                Debug.DrawRay(transform.position + (v3Offset * dir), new Vector2(0, -1).normalized,Color.black);
                Debug.DrawRay(transform.position - (v3Offset * dir), new Vector2(0, -1).normalized, Color.grey);
                Debug.DrawRay(transform.position, new Vector2(-dir, 0).normalized, Color.magenta);
                Debug.Log(state);
                switch (state)
                {
                    case EnemyState.defense:
                        inNeutral = false;
                        choseTime = false;
                        switch (difficultyLevel)
                        {
                            case 1:
                                if (hitB.collider != null)
                                {
                                    if (Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x , 2)) <= 3)
                                    {
                                        rb.velocity = new Vector2(-dir * speed, rb.velocity.y);
                                    }
                                }
                                AutoOffenseSwitch(0.25f);
                                break;
                            case 2:
                                if (hitB.collider != null)
                                {
                                    if (Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x, 2)) <= 3)
                                    {
                                        rb.velocity = new Vector2(-dir * speed, rb.velocity.y);
                                    }
                                }
                                AutoOffenseSwitch(0.2f);
                                break;
                            case 3:
                                if (hitB.collider != null)
                                {
                                    if (Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x, 2)) <= 3)
                                    {
                                        rb.velocity = new Vector2(-dir * speed, rb.velocity.y);
                                    }
                                }
                                AutoOffenseSwitch(0.1f);
                                break;
                            default:
                                Debug.Log("ERROR: LEVEL DOES NOT EXIST");
                                break;
                        }
                        break;
                    case EnemyState.neutral:
                        if(inNeutral == false)
                        {
                            randomInt = Random.Range(1, 3);
                            if (randomInt == 1)
                            {
                                startRight = true;
                            }
                            else if (randomInt == 2)
                            {
                                startRight = false;
                            }
                            inNeutral = true;
                        }
                        
                        AutoOffenseSwitch(0.75f);
                        
                        if(choseTime == false)
                        {
                            randomTime = Random.Range(0.5f,2.0f);
                            choseTime = true;
                        }

                        if (facingDirection == Direction.right)
                        {
                            if (secondsInOneDirection < randomTime)
                            {
                                if (startRight == true)
                                {
                                    if (hitF.collider != null)
                                    {
                                        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
                                    }
                                }
                                else
                                {
                                    if (hitB.collider != null)
                                    {
                                        rb.velocity = new Vector2(-dir * speed, rb.velocity.y);
                                    }
                                }
                                secondsInOneDirection += Time.deltaTime;
                            }
                            else
                            {
                                if (startRight)
                                {
                                    startRight = false;
                                }
                                else
                                {
                                    startRight = true;
                                }
                                choseTime = false;
                                secondsInOneDirection = 0;
                            }
                        }
                        else if (facingDirection == Direction.left)
                        {
                            if (secondsInOneDirection < randomTime)
                            {
                                if (startRight == true)
                                {
                                    if (hitB.collider != null)
                                    {
                                        rb.velocity = new Vector2(-dir * speed, rb.velocity.y);
                                    }
                                }
                                else
                                {
                                    if (hitF.collider != null)
                                    {
                                        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
                                    }
                                }
                                secondsInOneDirection += Time.deltaTime;
                            }
                            else
                            {
                                if (startRight)
                                {
                                    startRight = false;
                                }
                                else{
                                    startRight = true;
                                }
                                choseTime = false;
                                secondsInOneDirection = 0;
                            }
                        }        
                        break;
                    case EnemyState.offense:
                        //Debug.Log(Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x, 2)));
                        inNeutral = false;
                        choseTime = false;
                        switch (difficultyLevel)
                        {
                            case 1:
                                if (!attacking)
                                {
                                    JumpCheck(4.0f, 2);
                                    //hit = Physics2D.Raycast(transform.position, dVecF, 2, mask);
                                    if (hitF.collider != null)
                                    {
                                        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
                                    }
                                    else {
                                        Jump(100);
                                    }
                                    if(Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x, 2)) <= 1)
                                    {
                                        if (!attackCoRoutineRunning)
                                        {
                                            BasicAttack(Attack.medium, mediumAttackStats.hitboxActivationTime, mediumAttackStats.moveLag, mediumAttackStats.xVelocity, mediumAttackStats.yVelocity, mediumAttackStats.hitstun, mediumAttackStats.damage, mediumAttackStats.startup);
                                            attackCoRoutineRunning = true;
                                            StartCoroutine(AttackCoRoutine());
                                        }
                                    }
                                    else
                                    {
                                        hitbox.gameObject.SetActive(false);
                                        StopCoroutine(AttackCoRoutine());
                                        attacking = false;
                                        attackCoRoutineRunning = false;
                                    }
                                }
                                break;
                            case 2:
                                if (!attacking)
                                {
                                    JumpCheck(4.0f, 5);
                                    if (hitF.collider != null)
                                    {
                                        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
                                    }
                                    else
                                    {
                                        Jump(100);
                                    }
                                    if (Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x, 2)) <= 1)
                                    {
                                        if (!attackCoRoutineRunning && !usedAttack[2])
                                        {
                                            BasicAttack(Attack.heavy, heavyAttackStats.hitboxActivationTime, heavyAttackStats.moveLag, heavyAttackStats.xVelocity, heavyAttackStats.yVelocity, heavyAttackStats.hitstun, heavyAttackStats.damage, heavyAttackStats.startup);
                                            usedAttack[2] = true;
                                            attackCoRoutineRunning = true;
                                            StartCoroutine(AttackCoRoutine());
                                        }
                                        else if (!attackCoRoutineRunning)
                                        {
                                            usedAttack[2] = false;
                                            if (facingDirection == Direction.right)
                                            {
                                                attacking = true;
                                                GameObject specialMove = Instantiate(special, new Vector3(transform.position.x + 1, transform.position.y, -2.0f), Quaternion.identity) as GameObject;
                                                specialMove.tag = "EnemyHitbox";
                                                attackType = Attack.special;
                                                Debug.Log("Hado right");
                                            }
                                            else
                                            {
                                                attacking = true;
                                                GameObject specialMove = Instantiate(special, new Vector3(transform.position.x - 1, transform.position.y, -2.0f), Quaternion.Euler(0, 180, 0)) as GameObject;
                                                specialMove.tag = "EnemyHitbox";
                                                attackType = Attack.special;
                                                Debug.Log("Hado left");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        hitbox.gameObject.SetActive(false);
                                        StopCoroutine(AttackCoRoutine());
                                        attacking = false;
                                        attackCoRoutineRunning = false;
                                        usedAttack[2] = false;
                                    }
                                }
                                break;
                            case 3:
                                if (!attacking)
                                {
                                    JumpCheck(5.0f, 10);
                                    if (hitF.collider != null)
                                    {
                                        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
                                    }
                                    else
                                    {
                                        Jump(100);
                                    }
                                    if (Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x, 2)) <= 2)
                                    {
                                        if (!attackCoRoutineRunning)
                                        {
                                            if (!usedAttack[0])
                                            {
                                                BasicAttack(Attack.light, lightAttackStats.hitboxActivationTime, lightAttackStats.moveLag, lightAttackStats.xVelocity, lightAttackStats.yVelocity, lightAttackStats.hitstun, lightAttackStats.damage, lightAttackStats.startup);
                                                usedAttack[0] = true;
                                                attackCoRoutineRunning = true;
                                                StartCoroutine(AttackCoRoutine());
                                                Debug.Log("L");
                                            }
                                            else if (!usedAttack[1])
                                            {
                                                BasicAttack(Attack.medium, mediumAttackStats.hitboxActivationTime, mediumAttackStats.moveLag, mediumAttackStats.xVelocity, mediumAttackStats.yVelocity, mediumAttackStats.hitstun, mediumAttackStats.damage, mediumAttackStats.startup);
                                                usedAttack[1] = true;
                                                attackCoRoutineRunning = true;
                                                StartCoroutine(AttackCoRoutine());
                                                Debug.Log("M");
                                            }
                                            else if (!usedAttack[2])
                                            {
                                                BasicAttack(Attack.heavy, heavyAttackStats.hitboxActivationTime, heavyAttackStats.moveLag, heavyAttackStats.xVelocity, heavyAttackStats.yVelocity, heavyAttackStats.hitstun, heavyAttackStats.damage, heavyAttackStats.startup);
                                                usedAttack[2] = true;
                                                attackCoRoutineRunning = true;
                                                StartCoroutine(AttackCoRoutine());
                                                Debug.Log("H");
                                            }
                                            else
                                            {
                                                for (int i = 0; i < usedAttack.Length - 1; i++)
                                                {
                                                    usedAttack[i] = false;
                                                }
                                                if (facingDirection == Direction.right)
                                                {
                                                    attacking = true;
                                                    GameObject specialMove = Instantiate(special, new Vector3(transform.position.x + 0.5f, transform.position.y, -2.0f), Quaternion.identity) as GameObject;
                                                    specialMove.tag = "EnemyHitbox";
                                                    attackType = Attack.special;
                                                    Debug.Log("Hado right");
                                                }
                                                else
                                                {
                                                    attacking = true;
                                                    GameObject specialMove = Instantiate(special, new Vector3(transform.position.x - 0.5f, transform.position.y, -2.0f), Quaternion.Euler(0, 180, 0)) as GameObject;
                                                    specialMove.tag = "EnemyHitbox";
                                                    attackType = Attack.special;
                                                    Debug.Log("Hado left");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        hitbox.gameObject.SetActive(false);
                                        StopCoroutine(AttackCoRoutine());
                                        attacking = false;

                                        attackCoRoutineRunning = false;
                                        for (int i = 0; i < usedAttack.Length - 1; i++)
                                        {
                                            usedAttack[i] = false;
                                        }
                                    }
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
            else
            {
                for (int i = 0; i < usedAttack.Length - 1; i++)
                {
                    usedAttack[i] = false;
                }
                hitbox.gameObject.SetActive(false);
                if(attacking == true)
                {
                    StopCoroutine(AttackCoRoutine());
                    attackCoRoutineRunning = false;
                    attacking = false;
                }
                hitstun -= Time.deltaTime;
            }
        }
        GetComponent<PFEnemy>().dir = dir;
    }

    private void Jump(int jumpChance)
    {
        if (!grounded)
        {
            return;
        }
        if (randomInt <= jumpChance) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2((jumpForce / 2) * dir, jumpForce), ForceMode2D.Impulse);
            grounded = false;
        }
    }

    private void JumpCheck(float distance, int jumpChance)
    {
        if (Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x, 2)) >= distance)
        {
            randomInt = Random.Range(1, 101);
            Jump(jumpChance);
        }
    }

    private void AutoOffenseSwitch(float distance)
    {
        if (Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x, 2)) <= distance)
        {
            state = EnemyState.offense;
            stateSwitchTime = 0;
        }
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

    private void BasicAttack(Attack attack, float hitboxTime, float lag, float xVelocity, float yVelocity, float hitstunGiven, int damage, float startup)
    {
        attackType = attack;
        startupTime = startup;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            grounded = true;
        }
        else if (collision.CompareTag("PlayerHitbox"))
        {
            if (hitThisFrame == false)
            {
                hitThisFrame = true;
                hitstun = collision.GetComponent<FightingHitbox>().hitstun;
                currHP -= collision.GetComponent<FightingHitbox>().damage;
                if (collision.name == "SpecialMove(Clone)")
                {
                    Destroy(collision.gameObject);
                }
            }
        }
    }

    private void HitBoxSizeAndPos(float offsetX, float offsetY, float sizeX, float sizeY)
    {
        hitbox.offset = new Vector2(offsetX, offsetY);
        hitbox.size = new Vector2(sizeX, sizeY);
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
                HitBoxSizeAndPos(0.6f, 0.0f, 0.5f, 0.5f);
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
        yield return new WaitForSeconds(startupTime);
        hitbox.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitBoxActivationTime);
        hitbox.gameObject.SetActive(false);
        yield return new WaitForSeconds(endLagTime);
        randomInt = Random.Range(1, 3);
        state = ChooseRandomState(EnemyState.defense, EnemyState.neutral);
        attacking = false;
        attackCoRoutineRunning = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (grounded && collision.CompareTag("Ground"))
        {
            grounded = false;
        }
    }
}
