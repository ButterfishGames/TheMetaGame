using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RaceController : Mover
{
    public float baseSpeed, maxSpeed, accelRate;
    public float tiltRate, minTilt, maxTilt;
    public float jumpForce;
    public float deathWait;
    public float spawnDiff;

    public GameObject enemyRacerPrefab;

    public TextMeshProUGUI placeText;

    private float speed;
    private float accel;
    private float tilt;

    private float prevX;

    private bool started;

    private int dir;

    private bool grounded;
    private bool dying;

    private GameObject[] places = new GameObject[4];

    protected override void OnEnable()
    {
        base.OnEnable();

        Vector3 spawnPos = transform.position;
        spawnPos.x += spawnDiff * 3;
        for (int i = 0; i < 3; i++)
        {
            places[i] = Instantiate(enemyRacerPrefab, spawnPos, Quaternion.identity);
        }
        places[3] = gameObject;

        dir = transform.rotation == Quaternion.identity ? 1 : -1;
        StartCoroutine(StartRace());

        controls.Player.Jump.performed += JumpHandle;

        controls.Player.Jump.Enable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        controls.Player.Jump.performed -= JumpHandle;

        controls.Player.Jump.Disable();
    }

    private void JumpHandle(InputAction.CallbackContext context)
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

        Jump();
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = 0;
        accel = 0;
    }

    protected override void Update()
    {
        base.Update();

        if (dir == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                if (places[i].transform.position.x < places[i+1].transform.position.x)
                {
                    GameObject temp = places[i];
                    places[i] = places[i+1];
                    places[i+1] = temp;
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (places[i].transform.position.x > places[i+1].transform.position.x)
                {
                    GameObject temp = places[i];
                    places[i] = places[i+1];
                    places[i+1] = temp;
                } 
            }
        }

        placeText.text = "" + (System.Array.IndexOf(places, gameObject) + 1);
    }

    // Update is called once per frame
    void FixedUpdate()
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

        if (started && grounded)
        {
            speed = dir == 1 ? Mathf.Clamp((speed + accel) * Time.deltaTime, 0, maxSpeed) :
                Mathf.Clamp((speed + accel) * Time.deltaTime, -maxSpeed, 0);
        }

        prevX = rb.velocity.x;
        rb.velocity = new Vector2(speed, rb.velocity.y);
        if (Mathf.Round(rb.velocity.x) == 0)
        {
            prevX = 0;
        }

        transform.rotation = Quaternion.Euler(0, 0, tilt);
    }

    private IEnumerator StartRace()
    {
        // Replace with countdown;
        Debug.Log('3');
        yield return new WaitForSeconds(1);
        Debug.Log('2');
        yield return new WaitForSeconds(1);
        Debug.Log('1');
        yield return new WaitForSeconds(1);
        Debug.Log("Go!");
        
        started = true;
    }

    protected override void Move(float h, float v)
    {
        accel += h * accelRate;
        tilt = Mathf.Clamp(tilt + v * tiltRate, minTilt, maxTilt);
    }

    private void Jump()
    {
        if (!grounded)
        {
            return;
        }

        animator.SetBool("grounded", false);
        animator.SetBool("jumping", true);
        rb.velocity = new Vector2(rb.velocity.x, 0);

        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        grounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
        {
            return;
        }

        if (collision.CompareTag("Ground"))
        {
            animator.SetBool("jumping", false);
            grounded = true;
        }
        else if (collision.CompareTag("Killbox"))
        {
            if (!GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().Play();
            }

            GameController.singleton.Die();
        }
    }

    private IEnumerator Crash()
    {
        GameObject.Find("Song").GetComponent<AudioSource>().Stop();

        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }

        yield return new WaitForSeconds(deathWait);

        GameController.singleton.Die();
    }

    public int GetDir()
    {
        return dir;
    }
}
