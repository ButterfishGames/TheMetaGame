using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceEnemy : EnemyBehaviour
{

    [Tooltip("Difficulty level 0 is hard, 1 is medium, 2 is easy")]
    public float[] baseSpeeds = new float[3];

    [Tooltip("Difficulty level 0 is hard, 1 is medium, 2 is easy")]
    public float[] maxSpeeds = new float[3];

    [Tooltip("Difficulty level 0 is hard, 1 is medium, 2 is easy")]
    public float[] accelRates = new float[3];

    [Tooltip("Difficulty level 0 is hard, 1 is medium, 2 is easy")]
    public float[] maxAccels = new float[3];

    [Tooltip("Difficulty level 0 is hard, 1 is medium, 2 is easy")]
    [Range(0, 1)]
    public float[] jumpChances = new float[3];

    [Tooltip("Difficulty level 0 is hard, 1 is medium, 2 is easy")]
    public float[] jumpForces = new float[3];

    private float speed;
    private float accel;
    private float prevX;

    private float baseSpeed;
    private float maxSpeed;
    private float accelRate;
    private float maxAccel;
    private float jumpChance;
    private float jumpForce;

    private int dir;

    private bool started = false;
    private bool grounded = true;
    private bool dying = false;
    private bool canJump = false;

    private Rigidbody2D rb;

    private void OnDisable()
    {
        GetComponent<CapsuleCollider2D>().enabled = true;
        transform.Find("RacerCol").GetComponent<CapsuleCollider2D>().enabled = false;

        Destroy(this);
    }

    private void Start()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        transform.Find("RacerCol").GetComponent<CapsuleCollider2D>().enabled = true;

        prevX = 0;

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (dying)
        {
            return;
        }

        if (rb.velocity.x == 0 && !(prevX == 0))
        {
            dying = true;
            StartCoroutine(Crash());
        }

        if (started)
        {
        accel = speed > baseSpeed ? Mathf.Clamp(accel + dir * accelRate * (baseSpeed / speed), -maxAccel, maxAccel)
            : Mathf.Clamp(accel + dir * accelRate, -maxAccel, maxAccel);

            if (grounded)
            {
                speed = dir == 1 ? Mathf.Clamp((speed + accel), 0, maxSpeed) :
                    Mathf.Clamp((speed + accel), -maxSpeed, 0);
            }
        }

        prevX = rb.velocity.x;
        rb.velocity = new Vector2(speed * Time.deltaTime, rb.velocity.y);
        if (Mathf.Round(rb.velocity.x) == 0)
        {
            prevX = 0;
        }

        if (grounded && canJump)
        {
            RaycastHit2D hit;
            Vector2 dVec = new Vector2(dir * rb.velocity.x, -1).normalized;
            LayerMask mask = ~((1 << LayerMask.NameToLayer("Racer")) + (1 << LayerMask.NameToLayer("Enemy2")) + (1 << LayerMask.NameToLayer("Bounds")) + (1 << LayerMask.NameToLayer("DamageFloor")) + (1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("Checkpoint")));

            hit = Physics2D.Raycast(transform.position, dVec, rb.velocity.x, mask);
            float j;

            if (hit.collider == null)
            {
                j = Random.Range(0, 1);
                if (j < jumpChance)
                {
                    Jump();
                }
            }
            else
            {
                dVec = new Vector2(dir, 0);
                hit = Physics2D.Raycast(transform.position, dVec, rb.velocity.x, mask);

                if (hit.collider != null)
                {
                    j = Random.Range(0, 1);
                    if (j < jumpChance)
                    {
                        Jump();
                    }
                }
            }
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

    public void StartRace(int diffLevel)
    {
        baseSpeed = baseSpeeds[diffLevel];
        maxSpeed = maxSpeeds[diffLevel];
        accelRate = accelRates[diffLevel];
        maxAccel = maxAccels[diffLevel];
        jumpChance = jumpChances[diffLevel];
        jumpForce = jumpForces[diffLevel];

        started = true;

        StartCoroutine(LetJump());
    }

    private IEnumerator LetJump()
    {
        yield return new WaitForSeconds(0.1f);
        canJump = true;
    }

    public void SetDir(int newDir)
    {
        dir = newDir;
        if (dir == -1)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!grounded && collision.collider.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    public IEnumerator Crash()
    {
        // TODO: Implement Crash animation
        Destroy(gameObject);
        yield return null;
    }
}
