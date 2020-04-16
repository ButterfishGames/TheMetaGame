using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PFController : Mover
{
    [Tooltip("The rate at which the object accelerates")]
    public float moveSpeed;

    [Tooltip("The maximum speed at which the object may move")]
    public float maxVelX;

    [Tooltip("The vertical force applied to the object when it jumps (includes wall jumping)")]
    public float jumpForce;

    [Tooltip("The horizontal force applied to the object when it wall jumps")]
    public float wallJumpForce;

    [Tooltip("The force applied for death animation")]
    public float deathForce;

    [Tooltip("The time in seconds for which the game waits after death by enemy before reloading")]
    public float deathWait;

    [Range(0, 0.5f)]
    public float coyoteTime;

    /// <summary>
    /// Tracks whether the player is currently on the ground
    /// </summary>
    private bool grounded;

    private bool onWall;

    private int wallDir;

    private bool dying;

    private bool goingThrough;

    private BoxCollider2D groundTrigger;

    private bool coyote = false;

    private void OnJump(InputValue value)
    {
        if (!enabled
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
        if (!enabled
               || GameController.singleton.GetPaused()
               || DialogueManager.singleton.GetDisplaying()
               || CutsceneManager.singleton.scening
               || !SettingsController.singleton.pfUpJump)
        {
            return;
        }

        Jump();
    }

    private void Start()
    {
        if (GameController.singleton != null && !GameController.singleton.onMenu)
        {
            SaveManager.singleton.InitScene();
        }
        if (SettingsController.singleton != null)
        {
            AudioSource[] sources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource source in sources)
            {
                if (source.gameObject.name.Equals("Player") || source.gameObject.name.Equals("Song"))
                {
                    source.volume = SettingsController.singleton.musicVolume;
                }
                else
                {
                    source.volume = SettingsController.singleton.sfxVolume;
                }
            }
        }

        dying = false;

        groundTrigger = null;
        BoxCollider2D[] cols = GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D col in cols)
        {
            if (col.name.Equals("GroundTrigger"))
            {
                groundTrigger = col;
            }
        }

        onWall = false;
    }

    private void LateUpdate()
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

        if (grounded && !onWall)
        {
            GroundWallCheck();
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

    protected override void Move(float h, float v)
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

        /*if (v < 0 && !goingThrough)
        {
            StartCoroutine("GoThrough");
        }*/

        float moveX = hRaw == 0 ?
            (grounded ? h * moveSpeed * Time.deltaTime : rb.velocity.x) :
            (grounded ? Mathf.Clamp(rb.velocity.x + (h * moveSpeed * Time.deltaTime), -maxVelX, maxVelX) :
            Mathf.Clamp(rb.velocity.x + (h * (moveSpeed / 10) * Time.deltaTime), -maxVelX, maxVelX));
        float moveY = rb.velocity.y;
        if (moveX < 0 && !onWall)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            animator.SetBool("moving", true);
        }
        else if (moveX > 0 && !onWall)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
            animator.SetBool("moving", true);
        }

        rb.velocity = new Vector2(moveX, moveY);
    }

    private void Jump()
    {
        if (!grounded)
        {
            return;
        }

        animator.SetBool("grounded", false);
        animator.SetBool("walled", false);
        animator.SetBool("jumping", true);
        rb.velocity = new Vector2(rb.velocity.x, 0);

        float xForce = 0;
        if (onWall)
        {
            xForce = -wallDir * wallJumpForce;
            // TODO: Add Player Wall Jump SFX Event
        }
        else
        {
            // TODO: Add Player Jump SFX Event
        }

        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        rb.velocity = new Vector2(xForce, rb.velocity.y);
        onWall = false;
        grounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            animator.SetBool("jumping", false);
            grounded = true;
            GroundWallCheck();
        }
        else if (collision.CompareTag("Killbox"))
        {
            StartCoroutine(Die(false));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((!grounded || onWall) && collision.CompareTag("Ground"))
        {
            animator.SetBool("jumping", false);
            grounded = true;
            GroundWallCheck();
        }
    }

    public IEnumerator Die(bool hit)
    {
        if (dying)
        {
            yield return null;
        }

        dying = true;
        GameObject.Find("Song").GetComponent<AudioSource>().Stop();

        if (hit)
        {
            animator.SetBool("dying", true);
            GameController.singleton.SetPaused(true);
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            GameObject lavamap = GameObject.Find("Lavamap");
            if (lavamap != null)
            {
                lavamap.GetComponent<Renderer>().sortingOrder = 0;
            }
            yield return new WaitForSeconds(1);
            animator.SetBool("dead", true);
            rb.AddForce(Vector2.up * deathForce, ForceMode2D.Impulse);
            rb.gravityScale = GameController.singleton.GetGScale();
            col.enabled = false;
        }

        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
        /*foreach (BoxCollider2D childCol in GetComponentsInChildren<BoxCollider2D>())
        {
            childCol.enabled = false;
        }*/
        if (!CutsceneManager.singleton.scening)
        {
            yield return new WaitForSeconds(deathWait);
            GameController.singleton.Die(!hit);
        }
    }

    private IEnumerator GoThrough()
    {
        goingThrough = true;
        gameObject.layer = LayerMask.NameToLayer("ThroughTemp");
        yield return new WaitForSeconds(0.25f);
        gameObject.layer = LayerMask.NameToLayer("Player");
        goingThrough = false;
    }

    private void GroundWallCheck()
    {
        bool onGround = false;

        if (groundTrigger != null)
        {
            List<Collider2D> contacts = new List<Collider2D>();
            groundTrigger.GetContacts(contacts);
            foreach (Collider2D contact in contacts)
            {
                if (contact.CompareTag("Ground"))
                {
                    onGround = true;
                }
            }
        }
        else
        {
            grounded = true;
            return;
        }

        if (onGround)
        {
            float distX = groundTrigger.bounds.extents.x;
            float distY = groundTrigger.bounds.extents.y;
            distX -= 0.015f;
            Vector3 origin, origin2;
            origin = origin2 = transform.position;
            origin.x -= distX;
            origin2.x += distX;
            RaycastHit2D hit, hit2;

            hit = Physics2D.Raycast(origin, Vector2.down, 0.4f * transform.localScale.y,
                ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor")) 
                + (1 << LayerMask.NameToLayer("Racer")) + (1 << LayerMask.NameToLayer("Checkpoint"))
                + (1 << LayerMask.NameToLayer("Default"))));
            hit2 = Physics2D.Raycast(origin2, Vector2.down, 0.4f * transform.localScale.y,
                ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))
                + (1 << LayerMask.NameToLayer("Racer")) + (1 << LayerMask.NameToLayer("Checkpoint"))
                + (1 << LayerMask.NameToLayer("Default"))));

            if ((hit.collider == null || !hit.collider.CompareTag("Ground")) && (hit2.collider == null || !hit2.collider.CompareTag("Ground")))
            {
                onGround = false;
            }

            if (!onGround)
            {
                origin = origin2 = transform.position;
                origin.y += distY;
                origin2.y -= distY;
                RaycastHit2D hit3, hit4;
                hit = Physics2D.Raycast(origin, Vector2.right, 0.4f * transform.localScale.x,
                    ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));
                hit2 = Physics2D.Raycast(origin2, Vector2.right, 0.4f * transform.localScale.x,
                    ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));
                hit3 = Physics2D.Raycast(origin, Vector2.left, 0.4f * transform.localScale.x,
                    ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));
                hit4 = Physics2D.Raycast(origin2, Vector2.left, 0.4f * transform.localScale.x,
                    ~((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("DamageFloor"))));

                if ((hit.collider != null && hit.collider.CompareTag("Ground")) || (hit2.collider != null && hit2.collider.CompareTag("Ground")))
                {
                    onWall = true;
                    wallDir = 1;
                    transform.rotation = Quaternion.Euler(0, 90 + (wallDir * 90), 0);
                    // TODO: Add Stick to Wall SFX Event
                }
                else if ((hit3.collider != null && hit3.collider.CompareTag("Ground")) || (hit4.collider != null && hit4.collider.CompareTag("Ground")))
                {
                    onWall = true;
                    wallDir = -1;
                    transform.rotation = Quaternion.Euler(0, 90 + (wallDir * 90), 0);
                    // TODO: Add Stick to Wall SFX Event
                }
                else
                {
                    onWall = false;
                }
            }
        }
        else
        {
            onWall = false;
        }

        if (onGround)
        {
            onWall = false;
            animator.SetBool("grounded", true);
            animator.SetBool("walled", false);
            animator.SetBool("jumping", false);
            grounded = true;
        }
        else if (!onWall)
        {
            if (!coyote)
            {
                StartCoroutine(CoyoteTime());
            }
        }
        else
        {
            animator.SetBool("jumping", false);
            if (hor == wallDir)
            {
                animator.SetBool("walled", true);
            }
            else
            {
                animator.SetBool("walled", false);
            }
        }
    }

    private IEnumerator CoyoteTime()
    {
        coyote = true;
        yield return new WaitForSeconds(coyoteTime);
        animator.SetBool("grounded", false);
        animator.SetBool("walled", false);
        grounded = false;
        coyote = false;
    }

    public bool GetDying()
    {
        return dying;
    }
}
