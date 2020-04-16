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

    public float coyoteTime;

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

    BoxCollider2D groundTrigger;

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

        animator.SetBool("platformer", false);
        animator.SetBool("fighter", false);
        animator.SetBool("rpg", false);
        animator.SetBool("rhythm", false);
        animator.SetBool("racing", true);
        
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
        transform.Find("BikeBomb").gameObject.SetActive(true);

        StartCoroutine(StartRace());

        groundTrigger = transform.Find("GroundTrigger").GetComponent<BoxCollider2D>();
        groundTrigger.size = new Vector2(groundTrigger.size.x, 0.8f);
        groundTrigger.offset = new Vector2(0, -0.17f);

        grounded = true;

        //rb.freezeRotation = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        animator.SetBool("racing", false);

        //rb.freezeRotation = true;

        StopAllCoroutines();

        for (int i = 3; i > System.Array.IndexOf(places, gameObject); i--)
        {
            Destroy(places[i]);
        }

        speed = 0;
        accel = 0;
        tilt = 0;
        prevX = 0;

        racerCol.SetActive(false);
        GameObject bikeBomb = transform.Find("BikeBomb").gameObject;
        if (bikeBomb != null)
        {
            bikeBomb.SetActive(false);
        }

        placeText.gameObject.SetActive(false);

        groundTrigger.size = new Vector2(groundTrigger.size.x, 0.71f);
        groundTrigger.offset = new Vector2(0, -0.115f);

        transform.rotation = dir == 1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        started = false;
    }

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

        if (hRaw > 0)
        {
            animator.SetBool("decelerating", false);
            animator.SetBool("accelerating", true);
        }
        else if (hRaw < 0 && speed != 0)
        {
            animator.SetBool("accelerating", false);
            animator.SetBool("decelerating", true);
        }
        else
        {
            animator.SetBool("accelerating", false);
            animator.SetBool("decelerating", false);
            accel = 0;
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
            animator.SetBool("moving", false);
        }
        else
        {
            animator.SetBool("moving", true);
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

        if (!dying)
        {
            transform.rotation = dir == 1 ? Quaternion.Euler(0, 0, tilt) : Quaternion.Euler(0, 180, tilt);
        }
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
        accel = speed > baseSpeed ? Mathf.Clamp(h * accelRate * (baseSpeed / speed), -maxAccel, maxAccel) : 
            Mathf.Clamp(h * accelRate, -maxAccel, maxAccel);
        if (v != 0)
        {
            tilt = grounded ? tilt + v * tiltRate : tilt + v * tiltRate * 1.5f;
        }
        else if (grounded)
        {
            tilt = tilt > 0 ? Mathf.Clamp(tilt - tiltRate, 0, tilt) : Mathf.Clamp(tilt + tiltRate, tilt, 0);
        }
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            StartCoroutine(CoyoteTime());
        }
    }

    public IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(coyoteTime);
        grounded = false;
    }

    public IEnumerator Crash()
    {
        dying = true;

        animator.SetBool("dying", true);
        transform.Find("BikeBomb").GetComponent<BikeBoom>().enabled = true;
        transform.Find("BikeBomb").transform.SetParent(null);
        GameObject.Find("Song").GetComponent<AudioSource>().Stop();

        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }

        float t = 0;
        float timer = 0;
        while (Mathf.Round(transform.rotation.eulerAngles.z) != 0)
        {
            t += Time.deltaTime / deathWait;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(transform.rotation.eulerAngles.z, transform.rotation.y, t));
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        yield return new WaitForSeconds(deathWait - timer);

        GameController.singleton.Die();
    }

    public int GetDir()
    {
        return dir;
    }

    public bool GetDying()
    {
        return dying;
    }
}
