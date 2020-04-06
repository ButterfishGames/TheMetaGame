using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RaceController : Mover
{
    public float baseSpeed, maxSpeed, accelRate, maxAccel;
    public float tiltRate, minTilt, maxTilt;
    public float jumpForce;
    public float deathWait;
    public float spawnDiff;

    public GameObject enemyRacerPrefab;
    public GameObject racerCol;

    private float speed;
    private float accel;
    private float tilt;

    private float prevX;

    private bool started;

    private int dir;

    private bool grounded = true;
    private bool dying;

    private GameObject[] places = new GameObject[4];

    private TextMeshProUGUI placeText;

    protected override void Awake()
    {
        base.Awake();

        if (placeText == null)
        {
            GameObject pTemp = GameObject.Find("PlaceText");
            if (pTemp != null)
            {
                placeText = pTemp.GetComponent<TextMeshProUGUI>();

                placeText.gameObject.SetActive(false);
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        dir = transform.rotation == Quaternion.identity ? 1 : -1;

        Vector3 spawnPos = transform.position;
        spawnPos.x += (dir * spawnDiff) * 3;
        for (int i = 0; i < 3; i++)
        {
            places[i] = Instantiate(enemyRacerPrefab, spawnPos, Quaternion.identity);
            spawnPos.x -= dir * spawnDiff;
            places[i].GetComponent<RaceEnemy>().SetDir(dir);
        }
        places[3] = gameObject;

        racerCol.SetActive(true);

        StartCoroutine(StartRace());

        controls.Player.Jump.performed += JumpHandle;

        controls.Player.Jump.Enable();

        grounded = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        StopAllCoroutines();

        controls.Player.Jump.performed -= JumpHandle;

        controls.Player.Jump.Disable();

        for (int i = 3; i > System.Array.IndexOf(places, gameObject); i--)
        {
            Destroy(places[i]);
        }

        speed = 0;
        accel = 0;
        tilt = 0;
        prevX = 0;

        placeText.gameObject.SetActive(false);

        transform.rotation = dir == 1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        started = false;
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
                if (places[i] == null || (places[i] != null && places[i+1] != null) && (places[i].transform.position.x < places[i+1].transform.position.x))
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
                if (places[i] == null || (places[i] != null && places[i+1] != null) && (places[i].transform.position.x > places[i+1].transform.position.x))
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
            speed = dir == 1 ? Mathf.Clamp((speed + accel), 0, maxSpeed) :
                Mathf.Clamp((speed + accel), -maxSpeed, 0);
        }

        prevX = rb.velocity.x;
        rb.velocity = new Vector2(speed * Time.deltaTime, rb.velocity.y);
        if (Mathf.Round(rb.velocity.x) == 0)
        {
            prevX = 0;
        }

        if (grounded && (tilt < minTilt || tilt > maxTilt) && !dying)
        {
            dying = true;
            StartCoroutine(Crash());
        }

        if (tilt < -360 + maxTilt)
        {
            tilt += 360;
        }
        else if (tilt > 360 + minTilt)
        {
            tilt -= 360;
        }

        transform.rotation = dir == 1 ? Quaternion.Euler(0, 0, tilt) : Quaternion.Euler(0, 180, tilt);
    }

    private IEnumerator StartRace()
    {
        if (placeText == null)
        {
            placeText = GameObject.Find("PlaceText").GetComponent<TextMeshProUGUI>();
        }
        placeText.gameObject.SetActive(true);

        // Replace with countdown;
        Debug.Log('3');
        yield return new WaitForSeconds(1);
        Debug.Log('2');
        yield return new WaitForSeconds(1);
        Debug.Log('1');
        yield return new WaitForSeconds(1);
        Debug.Log("Go!");
        
        for (int i = 0; i < 3; i++)
        {
            places[i].GetComponent<RaceEnemy>().StartRace(i);
        }

        started = true;
    }

    protected override void Move(float h, float v)
    {
        accel = speed > baseSpeed ? Mathf.Clamp(accel + h * accelRate * (baseSpeed / speed), -maxAccel, maxAccel) : 
            Mathf.Clamp(accel + h * accelRate, -maxAccel, maxAccel);
        tilt = grounded ? tilt + v * tiltRate : tilt + v * tiltRate * 1.5f;
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
            StartCoroutine(Crash());
        }
    }

    public IEnumerator Crash()
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
