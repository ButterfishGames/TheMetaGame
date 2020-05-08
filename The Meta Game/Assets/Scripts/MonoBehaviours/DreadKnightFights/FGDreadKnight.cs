using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGDreadKnight : DreadKnightBehavior
{
    #region variables
    /// <summary>
    /// Sprite renderer to see if the enemy is visible or not so we know whether to include them in the fight.
    /// </summary>
    private SpriteRenderer spriteRenderer;

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
    private CapsuleCollider2D col;

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

    [HideInInspector] public float hitstun;

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

    public Animator attackEffects;

    public FGStatsAttackClass lightAttackStats;
    public FGStatsAttackClass mediumAttackStats;
    public FGStatsAttackClass heavyAttackStats;

    private bool[] usedAttack;

    [Tooltip("Distance the special move will spawn from the enemy")]
    public float hadoDistanceFromEnemy;

    [Tooltip("how much startup the Hadouken has in frames")]
    public float hadoStartup;

    [Tooltip("how much endlag the Hadouken has in frames")]
    public float hadoEndLag;

    private string animationAttackBoolString;

    private bool comboed;

    private bool justComboed;

    private BoxCollider2D groundTrigger;

    private bool inCutscene;

    /// <summary>
    /// How much time until Dread Knight charges
    /// </summary>
    private float chargePrepTime;

    /// <summary>
    /// The Dread Knights charge prep animation
    /// </summary>
    private Animation chargePrepAnimaton;

    private float timeToGetUp = 2.08f;
    private float timeGettingUp;

    private float currentChargeTime;

    [Tooltip("Cutscene before fight.")]
    public GameObject cutscene;

    [Tooltip("Cutscene after fight. MAKE SURE IT IS INACTIVE IN THE SCENE.")]
    public GameObject endCutscene;

    [Tooltip("Boss fight room barriers.")]
    public GameObject barriers;

    private bool bossCutsceneBegun;

    private Transform playerTransform;
    private Animator playerAnimator;
    #endregion

    private void Start()
    {
        player = GameObject.Find("Player");
        playerAnimator = player.GetComponentInChildren<Animator>();
        animator.SetBool("fighter", true);
        inCutscene = true;
        bossCutsceneBegun = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        gameObject.layer = LayerMask.NameToLayer("Blocking");
        state = EnemyState.neutral;
        hitbox = transform.Find("EnemyHitbox").GetComponent<BoxCollider2D>();
        hitThisFrame = false;
        v3Offset = new Vector3(xOffsetForRay, 0, 0);
        attackCoRoutineRunning = false;
        usedAttack = new bool[3];
        for (int i = 0; i < usedAttack.Length; i++)
        {
            usedAttack[i] = false;
        }
        groundTrigger = null;
        BoxCollider2D[] cols = GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D col in cols)
        {
            if (col.name.Equals("GroundTrigger"))
            {
                groundTrigger = col;
            }
        }

        currHP = maxHP;

        inverseDamageFadeTime = 1.0f / damageFadeTime;
    }

    void Update()
    {
        if (player.transform.position.x < 528)
        {
            return;
        }

        if (GameController.singleton.GetPaused())
        {
            return;
        }

        if (CutsceneManager.singleton.scening)
        {
            bossCutsceneBegun = true;
        }

        if (!CutsceneManager.singleton.scening && bossCutsceneBegun)
        {
            if (cutscene != null)
            {
                Destroy(cutscene.gameObject);
            }
            if (currHP > 0)
            {
                gameObject.layer = LayerMask.NameToLayer("Enemy2");
                barriers.SetActive(true);
                inCutscene = false;
                rb.gravityScale = 1.0f;
                col.enabled = true;
            }
            else
            {
                if (!animator.GetBool("hitWall"))
                {
                    barriers.SetActive(false);
                    endCutscene.SetActive(true);
                    rb.gravityScale = 0.0f;
                    col.enabled = false;
                    inCutscene = true;
                }
            }
        }

        hitThisFrame = false;
        if (inCutscene)
        {
            animator.SetBool("cutscene", true);
        }
        if (!inCutscene)
        {
            if (GameController.singleton.IsUnlocked("Platformer"))
            {
                GameController.singleton.Lock("Platformer");
            }
            if (GameController.singleton.equipped != GameController.GameMode.fighting)
            {
                GameController.singleton.SwitchMode(GameController.GameMode.fighting);
            }
            playerAnimator.SetBool("fighter", true);
            playerAnimator.SetBool("platformer", false);

            animator.SetBool("cutscene", false);
            
            if (groundTrigger != null)
            {
                List<Collider2D> contacts = new List<Collider2D>();
                groundTrigger.GetContacts(contacts);
                foreach (Collider2D contact in contacts)
                {
                    if (contact.CompareTag("Ground"))
                    {
                        grounded = true;
                    }
                    else
                    {
                        grounded = false;
                    }
                }
            }
            if (grounded == true)
            {
                animator.SetBool("jumping", false);
            }
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
                        transform.eulerAngles = new Vector3(0, 180, 0);
                    }
                    dir = 1;
                    break;
                case Direction.left:
                    if (dir == 1)
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
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
                animator.SetBool("hit", false);
                attackEffects.SetBool("hit", false);
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
                Vector2 dVec = new Vector2(0, -1).normalized;
                LayerMask mask = ~((1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("Enemy2")) + (1 << LayerMask.NameToLayer("Bounds")) + (1 << LayerMask.NameToLayer("DamageFloor")) + (1 << LayerMask.NameToLayer("Player")));

                hitF = Physics2D.Raycast(transform.position + (v3Offset * dir), dVec, 1.75f, mask);
                hitB = Physics2D.Raycast(transform.position - (v3Offset * dir), dVec, 1.75f, mask);
                Debug.Log(hitF.collider + "F");
                Debug.Log(hitB.collider + "B");
                Debug.DrawRay(transform.position + (v3Offset * dir), new Vector2(0, -1).normalized, Color.black);
                Debug.DrawRay(transform.position - (v3Offset * dir), new Vector2(0, -1).normalized, Color.grey);
                Debug.Log(state);
                switch (state)
                {
                    case EnemyState.defense:
                        #region defense
                        inNeutral = false;
                        choseTime = false;
                        DefenseMode(hitB, 3, 0.1f);
                        #endregion
                        break;
                    case EnemyState.neutral:
                        #region neutral
                        if (inNeutral == false)
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

                        if (justComboed == false)
                        {
                            AutoOffenseSwitch(0.75f);
                        }

                        if (choseTime == false)
                        {
                            randomTime = Random.Range(0.5f, 2.0f);
                            choseTime = true;
                        }

                        if (facingDirection == Direction.right)
                        {
                            NeutralDirectionMovement(hitF, hitB, dir);
                        }
                        else if (facingDirection == Direction.left)
                        {
                            NeutralDirectionMovement(hitB, hitF, -dir);
                        }
                        #endregion
                        break;
                    case EnemyState.offense:
                        #region offense
                        inNeutral = false;
                        choseTime = false;  
                        if (!attacking)
                        {
                            JumpCheck(5.0f, 10);
                            Debug.Log(hitF.collider);
                            if (hitF.collider != null)
                            {
                                animator.SetBool("moving", true);
                                rb.velocity = new Vector2(dir * speed, rb.velocity.y);
                            }
                            else
                            {
                                Debug.Log("collider not hit");
                                Jump(100);
                                animator.SetBool("moving", false);
                            }
                            if (Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x, 2)) <= 2)
                            {
                                animator.SetBool("close", true);
                                if (!attackCoRoutineRunning)
                                {
                                    if (!usedAttack[0])
                                    {
                                        AkSoundEngine.PostEvent("sfx_punch", gameObject);
                                        BasicAttack(Attack.light, lightAttackStats.hitboxActivationTime, lightAttackStats.moveLag, lightAttackStats.xVelocity, lightAttackStats.yVelocity, lightAttackStats.hitstun, lightAttackStats.damage, lightAttackStats.startup, "lightattack");
                                        usedAttack[0] = true;
                                        attackCoRoutineRunning = true;
                                        StartCoroutine(AttackCoRoutine());
                                    }
                                    else if (!usedAttack[1])
                                    {
                                        AkSoundEngine.PostEvent("sfx_uppercut", gameObject);
                                        BasicAttack(Attack.medium, mediumAttackStats.hitboxActivationTime, mediumAttackStats.moveLag, mediumAttackStats.xVelocity, mediumAttackStats.yVelocity, mediumAttackStats.hitstun, mediumAttackStats.damage, mediumAttackStats.startup, "mediumattack");
                                        usedAttack[1] = true;
                                        attackCoRoutineRunning = true;
                                        StartCoroutine(AttackCoRoutine());
                                    }
                                    else if (!usedAttack[2])
                                    {
                                        AkSoundEngine.PostEvent("sfx_kick", gameObject);
                                        BasicAttack(Attack.heavy, heavyAttackStats.hitboxActivationTime, heavyAttackStats.moveLag, heavyAttackStats.xVelocity, heavyAttackStats.yVelocity, heavyAttackStats.hitstun, heavyAttackStats.damage, heavyAttackStats.startup, "heavyattack");
                                        usedAttack[2] = true;
                                        attackCoRoutineRunning = true;
                                        StartCoroutine(AttackCoRoutine());
                                    }
                                    else
                                    {
                                        comboed = true;
                                        animator.SetBool("comboed", true);
                                        for (int i = 0; i < usedAttack.Length; i++)
                                        {
                                            usedAttack[i] = false;
                                        }
                                        animator.SetTrigger("specialattack");
                                        attacking = true;
                                        animationAttackBoolString = "specialattack";
                                        attackType = Attack.special;
                                        StartCoroutine(AttackCoRoutine());
                                    }
                                }
                            }
                            else
                            {
                                comboed = false;
                                animator.SetBool("close", false);
                                hitbox.gameObject.SetActive(false);
                                StopCoroutine(AttackCoRoutine());
                                attacking = false;
                                attackCoRoutineRunning = false;
                                for (int i = 0; i < usedAttack.Length; i++)
                                {
                                    usedAttack[i] = false;
                                }
                            }
                        }

                        #endregion
                        break;
                    default:
                        Debug.Log("ERROR: STATE DOES NOT EXIST");
                        break;
                }
            }
            else
            {
                comboed = false;
                justComboed = false;
                animator.SetBool("comboed", false);
                animator.SetBool("hit", true);
                attackEffects.SetBool("hit", true);
                for (int i = 0; i < usedAttack.Length; i++)
                {
                    usedAttack[i] = false;
                }
                hitbox.gameObject.SetActive(false);
                if (attacking == true)
                {
                    StopCoroutine(AttackCoRoutine());
                    attackCoRoutineRunning = false;
                    attacking = false;
                }
                hitstun -= Time.deltaTime;
            }
        }
        //GetComponent<PFEnemy>().dir = dir;
    }

    private void Jump(int jumpChance)
    {
        if (!grounded)
        {
            return;
        }
        if (randomInt <= jumpChance)
        {
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

    private void BasicAttack(Attack attack, float hitboxTime, float lag, float xVelocity, float yVelocity, float hitstunGiven, int damage, float startup, string animationAttackTrigger)
    {
        attackType = attack;
        startupTime = startup;
        hitBoxActivationTime = hitboxTime;
        endLagTime = lag;
        if (grounded)
        {
            rb.velocity = new Vector2(xVelocity, yVelocity);
        }
        hitbox.gameObject.GetComponent<FightingHitbox>().hitstun = hitstunGiven / 60;
        hitbox.gameObject.GetComponent<FightingHitbox>().damage = damage;
        attacking = true;
        animationAttackBoolString = animationAttackTrigger;
        animator.SetTrigger(animationAttackTrigger);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerHitbox"))
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void HitBoxSizeAndPos(float offsetX, float offsetY, float sizeX, float sizeY)
    {
        hitbox.offset = new Vector2(offsetX, offsetY);
        hitbox.size = new Vector2(sizeX, sizeY);
    }

    private IEnumerator AttackCoRoutine()
    {
        animator.SetBool("moving", false);
        animator.SetBool("attacking", true);
        if (grounded)
        {
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
        switch (attackType)
        {
            case Attack.light:
                HitBoxSizeAndPos(lightAttackStats.offsetX, lightAttackStats.offsetY, lightAttackStats.sizeX, lightAttackStats.sizeY);
                break;
            case Attack.medium:
                HitBoxSizeAndPos(mediumAttackStats.offsetX, mediumAttackStats.offsetY, mediumAttackStats.sizeX, mediumAttackStats.sizeY);
                break;
            case Attack.heavy:
                HitBoxSizeAndPos(heavyAttackStats.offsetX, heavyAttackStats.offsetY, heavyAttackStats.sizeX, heavyAttackStats.sizeY);
                break;
            case Attack.special:
                startupTime = hadoStartup;
                endLagTime = hadoEndLag;
                hitBoxActivationTime = 0;
                break;
            default:
                Debug.Log("ERROR: INVALID STARTING ATTACK");
                break;
        }
        yield return new WaitForSeconds(startupTime / 60);
        if (animationAttackBoolString == "specialattack")
        {
            if (facingDirection == Direction.right)
            {
                GameObject specialMove = Instantiate(special, new Vector3(transform.position.x + hadoDistanceFromEnemy, transform.position.y, -2.0f), Quaternion.identity) as GameObject;
                specialMove.tag = "EnemyHitbox";
            }
            else
            {
                GameObject specialMove = Instantiate(special, new Vector3(transform.position.x - hadoDistanceFromEnemy, transform.position.y, -2.0f), Quaternion.Euler(0, 180, 0)) as GameObject;
                specialMove.tag = "EnemyHitbox";
            }
        }
        else
        {
            attackEffects.SetBool(animationAttackBoolString, true);
            hitbox.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(hitBoxActivationTime / 60);
        attackEffects.SetBool(animationAttackBoolString, false);
        hitbox.gameObject.SetActive(false);
        yield return new WaitForSeconds(endLagTime / 60);
        randomInt = Random.Range(1, 3);
        if (comboed == true)
        {
            comboed = false;
            animator.SetBool("comboed", false);
            justComboed = true;
            state = ChooseRandomState(EnemyState.defense, EnemyState.neutral);
        }
        attacking = false;
        animator.SetBool("attacking", false);
        attackCoRoutineRunning = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (grounded && collision.collider.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    private IEnumerator Death()
    {
        animator.SetBool("dead", true);
        Destroy(rb);
        col.enabled = false;
        yield return new WaitForSeconds(animator.GetNextAnimatorStateInfo(0).length + 2);
        Destroy(gameObject);
    }

    private void NeutralDirectionMovement(RaycastHit2D ray1, RaycastHit2D ray2, int directionInt)
    {
        if (secondsInOneDirection < randomTime)
        {
            if (startRight == true)
            {
                if (ray1.collider != null)
                {
                    animator.SetBool("moving", true);
                    rb.velocity = new Vector2(directionInt * speed, rb.velocity.y);
                }
                else
                {
                    Debug.Log("Neutral no move right");
                    animator.SetBool("moving", false);
                }
            }
            else
            {
                if (ray2.collider != null)
                {
                    animator.SetBool("moving", true);
                    rb.velocity = new Vector2(-directionInt * speed, rb.velocity.y);
                }
                else
                {
                    Debug.Log("Neutral no move left");
                    animator.SetBool("moving", false);
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

    private void DefenseMode(RaycastHit2D hit, float distanceFromPlayer, float distanceSwitch)
    {
        if (hit.collider != null)
        {
            if (Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x, 2)) <= distanceFromPlayer)
            {
                animator.SetBool("moving", true);
                rb.velocity = new Vector2(-dir * speed, rb.velocity.y);
            }
            else
            {
                Debug.Log("Defense no move back");
                animator.SetBool("moving", false);
            }
        }
        else
        {
            Debug.Log("Defense no move back");
            animator.SetBool("moving", false);
        }
        if (justComboed == false)
        {
            AutoOffenseSwitch(distanceSwitch);
        }
    }
}
