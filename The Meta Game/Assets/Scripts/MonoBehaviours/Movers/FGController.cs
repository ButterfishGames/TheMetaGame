using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FGController : Mover
{
    #region variables
    [Tooltip("The rate at which the object moves")]
    public float moveSpeed;

    [Tooltip("The force applied to the object when it jumps")]
    public float jumpForce;

    [Tooltip("The force applied for death animation")]
    public float deathForce;

    [Tooltip("The time in seconds for which the game waits after death by enemy before reloading")]
    public float deathWait;

    [Tooltip("Can you jump with up on the stick?")]
    public bool canStickJump;

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

    private readonly InputDirection[] specialRight = { InputDirection.right, InputDirection.rightDown, InputDirection.down };

    private readonly InputDirection[] specialLeft = { InputDirection.left, InputDirection.leftDown, InputDirection.down };

    /// <summary>
    /// Bool to see if direction is being held so the input doesn't repeat every frame.
    /// Left [0], LeftDown [1], Down [2], RightDown [3], Right [4]
    /// </summary>
    private bool[] inputsHeld;

    private bool[] heldAttackButton;

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
    [HideInInspector] public float hitstun;

    /// <summary>
    /// Hit box to hit enemies
    /// </summary>
    private BoxCollider2D hitbox;

    /// <summary>
    /// float to determine the length of time before the hitbox has appeared.
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
    /// To detect if the enemy has been hit on this frame because it picks up two instances for some reason
    /// </summary>
    private bool hitThisFrame;

    [Tooltip("how much startup the Hadouken has in frames")]
    public float hadoStartup;

    [Tooltip("how much endlag the Hadouken has in frames")]
    public float hadoEndLag;

    public Animator attackEffects;

    public FGStatsAttackClass lightAttackStats;
    public FGStatsAttackClass mediumAttackStats;
    public FGStatsAttackClass heavyAttackStats;

    private string animationAttackBoolString;

    [Tooltip("Distance the special move will spawn from the player")]
    public float hadoDistanceFromPlayer;

    private bool dead;
    
    private InputAction light, medium, heavy;

    private float moveVelX;

    private float moveVelY;

    private bool dying;

    private Slider healthSlider;

    private Slider hitstunSlider;

    private Slider enemyHealthSlider;

    private Slider enemyHitstunSlider;

    private GameObject closestEnemy;

    private List<GameObject> enemiesInView = new List<GameObject>();
    #endregion

    protected override void OnEnable()
    {
        base.OnEnable();

        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(true);
        }
        if (hitstunSlider != null)
        {
            hitstunSlider.gameObject.SetActive(true);
        }
        if (enemyHealthSlider != null)
        {
            enemyHealthSlider.gameObject.SetActive(true);
        }
        if (enemyHitstunSlider != null)
        {
            enemyHitstunSlider.gameObject.SetActive(true);
        }

        dying = false;
        OnControlsChanged(GetComponent<PlayerInput>());
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
        }
        if (hitstunSlider != null)
        {
            hitstunSlider.gameObject.SetActive(false);
        }
        if (enemyHealthSlider != null)
        {
            enemyHealthSlider.gameObject.SetActive(false);
        }
        if (enemyHitstunSlider != null)
        {
            enemyHitstunSlider.gameObject.SetActive(false);
        }

        light.performed -= LightPerfHandle;
        light.canceled -= LightCancHandle;
        medium.performed -= MedPerfHandle;
        medium.canceled -= MedCancHandle;
        heavy.performed -= HeavyPerfHandle;
        heavy.canceled -= HeavyCancHandle;

        light.Disable();
        medium.Disable();
        heavy.Disable();
    }

    private void OnControlsChanged(PlayerInput pIn)
    {
        light = pIn.actions["Light"];
        medium = pIn.actions["Medium"];
        heavy = pIn.actions["Heavy"];

        if (enabled)
        {
            light.performed += LightPerfHandle;
            light.canceled += LightCancHandle;
            medium.performed += MedPerfHandle;
            medium.canceled += MedCancHandle;
            heavy.performed += HeavyPerfHandle;
            heavy.canceled += HeavyCancHandle;

            light.Enable();
            medium.Enable();
            heavy.Enable();
        }
    }

    private void OnJump (InputValue value)
    {
        if (!enabled
            || hitstun > 0
            || attacking
            || GameController.singleton.GetPaused()
            || DialogueManager.singleton.GetDisplaying()
            || CutsceneManager.singleton.scening)
        {
            return;
        }

        Jump();
    }

    private void OnUpJump(InputValue value)
    {
        if(!enabled
            || hitstun > 0
            || attacking
            || GameController.singleton.GetPaused()
            || DialogueManager.singleton.GetDisplaying()
            || CutsceneManager.singleton.scening
            || !SettingsController.singleton.fgUpJump)
        {
            return;
        }

        Jump();
    }

    private void LightPerfHandle(InputAction.CallbackContext context)
    {
        if (!enabled || GameController.singleton.GetPaused() || hitstun > 0 || attacking)
        {
            return;
        }

        if (DialogueManager.singleton.GetDisplaying())
        {
            return;
        }

        if (CutsceneManager.singleton.scening)
        {
            return;
        }

        if (inputs.SequenceEqual(specialRight) || inputs.SequenceEqual(specialLeft))
        {
            attacking = true;
            inputs[0] = InputDirection.none;
            attackType = Attack.special;
            animator.SetTrigger("specialattack");
            animationAttackBoolString = "specialattack";
        }
        else
        {
            AkSoundEngine.PostEvent("sfx_punch", gameObject);
            heldAttackButton[0] = true;
            BasicAttack(Attack.light, lightAttackStats.hitboxActivationTime, lightAttackStats.moveLag, lightAttackStats.xVelocity, lightAttackStats.yVelocity, lightAttackStats.hitstun, lightAttackStats.damage, lightAttackStats.startup, "lightattack");
        }
    }

    private void LightCancHandle(InputAction.CallbackContext context)
    {
        if (!enabled)
        {
            return;
        }

        heldAttackButton[0] = false;
    }

    private void MedPerfHandle(InputAction.CallbackContext context)
    {
        if (!enabled || GameController.singleton.GetPaused() || hitstun > 0 || attacking)
        {
            return;
        }

        if (DialogueManager.singleton.GetDisplaying())
        {
            return;
        }

        if (CutsceneManager.singleton.scening)
        {
            return;
        }

        if (inputs.SequenceEqual(specialRight) || inputs.SequenceEqual(specialLeft))
        {
            attacking = true;
            inputs[0] = InputDirection.none;
            attackType = Attack.special;
            animator.SetTrigger("specialattack");
            animationAttackBoolString = "specialattack";
        }
        else
        {
            AkSoundEngine.PostEvent("sfx_uppercut", gameObject);
            heldAttackButton[1] = true;
            BasicAttack(Attack.medium, mediumAttackStats.hitboxActivationTime, mediumAttackStats.moveLag, mediumAttackStats.xVelocity, mediumAttackStats.yVelocity, mediumAttackStats.hitstun, mediumAttackStats.damage, mediumAttackStats.startup, "mediumattack");
        }
    }

    private void MedCancHandle(InputAction.CallbackContext context)
    {
        if (!enabled)
        {
            return;
        }

        heldAttackButton[1] = false;
    }

    private void HeavyPerfHandle(InputAction.CallbackContext context)
    {
        if (!enabled || GameController.singleton.GetPaused() || hitstun > 0 || attacking)
        {
            return;
        }

        if (DialogueManager.singleton.GetDisplaying())
        {
            return;
        }

        if (CutsceneManager.singleton.scening)
        {
            return;
        }

        if (inputs.SequenceEqual(specialRight) || inputs.SequenceEqual(specialLeft))
        {
            attacking = true;
            inputs[0] = InputDirection.none;
            attackType = Attack.special;
            animator.SetTrigger("specialattack");
            animationAttackBoolString = "specialattack";
        }
        else
        {
            AkSoundEngine.PostEvent("sfx_kick", gameObject);
            heldAttackButton[2] = true;
            BasicAttack(Attack.heavy, heavyAttackStats.hitboxActivationTime, heavyAttackStats.moveLag, heavyAttackStats.xVelocity, heavyAttackStats.yVelocity, heavyAttackStats.hitstun, heavyAttackStats.damage, heavyAttackStats.startup, "heavyattack");
        }
    }

    private void HeavyCancHandle(InputAction.CallbackContext context)
    {
        if (!enabled)
        {
            return;
        }

        heldAttackButton[2] = false;
    }

    protected override void Awake()
    {
        base.Awake();

        if (healthSlider == null)
        {
            GameObject healthBarTemp = GameObject.Find("PlayerHealthBar");
            if (healthBarTemp != null)
            {
                healthSlider = healthBarTemp.GetComponent<Slider>();
                healthSlider.gameObject.SetActive(false);
            }
        }

        if (hitstunSlider == null)
        {
            GameObject hitstunSliderTemp = GameObject.Find("PlayerHitstunBar");
            if (hitstunSliderTemp != null)
            {
                hitstunSlider = hitstunSliderTemp.GetComponent<Slider>();
                hitstunSlider.gameObject.SetActive(false);
            }
        }

        if (enemyHealthSlider == null)
        {
            GameObject healthBarTemp = GameObject.Find("EnemyHealthBar");
            if (healthBarTemp != null)
            {
                enemyHealthSlider = healthBarTemp.GetComponent<Slider>();
                enemyHealthSlider.gameObject.SetActive(false);
            }
        }

        if (enemyHitstunSlider == null)
        {
            GameObject hitstunSliderTemp = GameObject.Find("EnemyHitstunBar");
            if (hitstunSliderTemp != null)
            {
                enemyHitstunSlider = hitstunSliderTemp.GetComponent<Slider>();
                enemyHitstunSlider.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        inputs = new InputDirection[3];
        inputsHeld = new bool[5];
        heldAttackButton = new bool[3];

        for (int i = 0; i < inputs.Length - 1; i++)
        {
            inputs[i] = InputDirection.none;
        }
        for (int i = 0; i < inputsHeld.Length - 1; i++)
        {
            inputsHeld[i] = false;
        }
        attacking = false;
        hitThisFrame = false;
        animator = GetComponentInChildren<Animator>();

        hitbox = transform.Find("Hitbox").GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        #region healthBarStuff
        healthSlider.maxValue = GameController.singleton.maxHP;
        healthSlider.value = GameController.singleton.GetHP();
        hitstunSlider.value = hitstun;
        if (!CutsceneManager.singleton.scening)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length != 0)
            {
                foreach (GameObject enemy in enemies)
                {
                    if (enemy.gameObject.GetComponent<FGEnemy>().fighting == true)
                    {
                        if (!enemiesInView.Contains(enemy))
                        {
                            if (!enemy.GetComponent<FGEnemy>().dying)
                            {
                                enemiesInView.Add(enemy);
                            }
                        }
                        else
                        {
                            if (enemy.GetComponent<FGEnemy>().dying)
                            {
                                enemiesInView.Remove(enemy);
                            }
                        }
                    }
                }
                if (enemiesInView.Count != 0)
                {
                    //https://answers.unity.com/questions/790508/remove-missing-objects-from-list.html
                    for (int i = enemiesInView.Count - 1; i > -1; i--)
                    {
                        if (enemiesInView[i] == null)
                            enemiesInView.RemoveAt(i);
                    }
                    for (int i = 0; i < enemiesInView.Count; i++)
                    {
                        if (closestEnemy != null)
                        {
                            if (Mathf.Sqrt(Mathf.Pow(transform.position.x - enemiesInView[i].transform.position.x, 2)) < Mathf.Sqrt(Mathf.Pow(transform.position.x - closestEnemy.transform.position.x, 2)))
                            {
                                closestEnemy = enemiesInView[i];
                            }
                        }
                        else
                        {
                            if (enemiesInView[i] != null)
                            {
                                closestEnemy = enemiesInView[i];
                            }
                        }
                    }
                }
            }
            if (closestEnemy != null)
            {
                enemyHealthSlider.maxValue = closestEnemy.GetComponent<FGEnemy>().maxHP;
                enemyHealthSlider.value = closestEnemy.GetComponent<FGEnemy>().currHP;
                enemyHitstunSlider.value = closestEnemy.GetComponent<FGEnemy>().hitstun;
            }
            else
            {
                enemyHealthSlider.maxValue = 1.0f;
                enemyHealthSlider.value = 0.0f;
                enemyHitstunSlider.value = 0.0f;
            }
        }
        #endregion

        if (GameController.singleton.GetHP() <= 0)
        {
            if (!dead)
            {
                dead = true;
                StartCoroutine(Death());
            }
        }
        if (!dead)
        {
            if (grounded)
            {
                animator.SetBool("jumping", false);
            }
            else
            {
                animator.SetBool("jumping", true);
            }
            hitThisFrame = false;
            if (hitstun <= 0)
            {
                animator.SetBool("hit", false);
                attackEffects.SetBool("hit", false);

                Move(hRaw, vRaw);

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

                if (attacking == true)
                {
                    animator.SetBool("attacking", true);
                }
                else
                {
                    animator.SetBool("attacking", false);
                }

                if (attacking)
                {
                    if (!attackCoRoutineRunning)
                    {
                        attackCoRoutineRunning = true;
                        StartCoroutine(AttackCoRoutine(animationAttackBoolString, moveVelX, moveVelY));
                    }
                }
                else
                {
                    hitbox.gameObject.SetActive(false);
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
            else
            {
                animator.SetBool("hit", true);
                attackEffects.SetBool("hit", true);
                animator.SetBool("attacking", false);
                hitbox.gameObject.SetActive(false);
                attacking = false;
                hitstun -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameController.singleton.GetPaused())
        {
            return;
        }

        if (DialogueManager.singleton.GetDisplaying())
        {
            return;
        }

        if (CutsceneManager.singleton.scening)
        {
            return;
        }

        base.Update();
    }

    protected override void Move(float h, float v)
    {
        if (GameController.singleton.GetPaused() 
            || DialogueManager.singleton.GetDisplaying() 
            || CutsceneManager.singleton.scening)
        {
            return;
        }

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
                
                if (v > 0 && (canStickJump || !stickUp))
                {
                    Jump();
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

        animator.SetBool("jumping", true);

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
                GameController.singleton.Damage(collision.GetComponent<FightingHitbox>().damage);
                if(collision.name == "SpecialMove(Clone)" && collision.CompareTag("EnemyHitbox"))
                {
                    Debug.Log("Hit by hado");
                    Destroy(collision.gameObject);
                }
            }
        }
        else if (collision.CompareTag("Killbox"))
        {
            GameController.singleton.Die(true);
        }
    }

    //When switching Gamemodes to fighting for the first time, if you start on the ground you aren't considered grounded
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            grounded = true;
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
        hitbox.offset = new Vector2(offsetX, offsetY);
        hitbox.size = new Vector2(sizeX, sizeY);
    }

    private void BasicAttack(Attack attack, float hitboxTime, float lag, float xVelocity,  float yVelocity, float hitstunGiven, int damage, float startup, string animationAttackBool)
    {
        attackType = attack;
        startupTime = startup;
        hitBoxActivationTime = hitboxTime;
        endLagTime = lag;
        if (grounded)
        {
            rb.velocity = new Vector2(xVelocity, yVelocity);
        }
        hitbox.gameObject.GetComponent<FightingHitbox>().hitstun = hitstunGiven/60;
        hitbox.gameObject.GetComponent<FightingHitbox>().damage = damage;
        attacking = true;
        animator.SetBool(animationAttackBool, true);
        moveVelX = xVelocity;
        moveVelY = yVelocity;
//        attackEffects.SetBool(animationAttackBool, true);
        animationAttackBoolString = animationAttackBool;
        animator.SetTrigger(animationAttackBool);
    }

    private IEnumerator AttackCoRoutine(string animationAttackBool, float xVel, float yVel)
    {
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
        yield return new WaitForSeconds(startupTime/60);
        if (animationAttackBool == "specialattack")
        {
            if (dir == Direction.right)
            {
                GameObject specialMove = Instantiate(special, new Vector3(transform.position.x + hadoDistanceFromPlayer, transform.position.y, -2.0f), Quaternion.identity) as GameObject;
                specialMove.tag = "PlayerHitbox";
            }
            else
            {
                GameObject specialMove = Instantiate(special, new Vector3(transform.position.x - hadoDistanceFromPlayer, transform.position.y, -2.0f), Quaternion.Euler(0, 180, 0)) as GameObject;
                specialMove.tag = "PlayerHitbox";
            }
        }
        else
        {
            hitbox.gameObject.SetActive(true);
            attackEffects.SetBool(animationAttackBool, true);
            if (hitbox.gameObject.activeSelf == true)
            {
                Debug.Log("hitbox active");
                List<Collider2D> contacts = new List<Collider2D>();
                hitbox.GetContacts(contacts);
                Debug.Log(contacts);
                Debug.Log(contacts.Count);
                foreach (Collider2D contact in contacts)
                {
                    Debug.Log(contact.tag);
                    if (contact.CompareTag("Enemy"))
                    {
                        Debug.Log("Hit Enemy");
                        contact.GetComponent<Rigidbody2D>().velocity = new Vector2(xVel * (float)dir, yVel);
                    }
                }
            }
        }
        yield return new WaitForSeconds(hitBoxActivationTime/60);
        hitbox.gameObject.SetActive(false);
        attackEffects.SetBool(animationAttackBool, false);
        yield return new WaitForSeconds(endLagTime/60);
        attacking = false;
        attackCoRoutineRunning = false;
    }

    private IEnumerator Death()
    {
        dying = true;
        Debug.Log("Died");
        animator.SetBool("dead", true);
        GameController.singleton.SetPaused(true);
        GameObject.Find("Song").GetComponent<AudioSource>().Stop();
        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
        yield return new WaitForSeconds(animator.GetNextAnimatorStateInfo(0).length + 2);
        GameController.singleton.Die();
    }

    public bool GetDying()
    {
        return dying;
    }
}
