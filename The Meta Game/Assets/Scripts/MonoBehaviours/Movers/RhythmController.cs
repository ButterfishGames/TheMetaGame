using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RhythmController : Mover
{
    public float speed;

    public float startWait;

    private bool started;

    private float baseY;

    public Vector3 startPos;

    public AudioClip hitClip, deathClip;

    private float xDiff = 20;

    private bool dying = false;

    public bool inGround = false;

    private int misses;

    private StaffController sCon;

    private AudioSource source;

    protected override void OnEnable()
    {
        started = false;
        source = GetComponent<AudioSource>();
        misses = 0;
        baseY = transform.position.y - 0.05f;
        startPos = transform.position;

        inGround = false;
    }

    protected override void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnUpNote(InputValue value)
    {
        if (!started
            || !enabled
            || dying
            || GameController.singleton.GetPaused())
        {
            return;
        }

        StartCoroutine(UpNote());
    }

    private void OnDUpNote(InputValue value)
    {
        if (!started
            || !enabled
            || dying
            || GameController.singleton.GetPaused())
        {
            return;
        }

        if (!SettingsController.singleton.dInput)
        {
            return;
        }

        StartCoroutine(UpNote());
    }

    private void OnLeftNote(InputValue value)
    {
        if (!started
            || !enabled
            || dying
            || GameController.singleton.GetPaused())
        {
            return;
        }

        StartCoroutine(LeftNote());
    }

    private void OnDLeftNote(InputValue value)
    {
        if (!started
            || !enabled
            || dying
            || GameController.singleton.GetPaused())
        {
            return;
        }

        if (!SettingsController.singleton.dInput)
        {
            return;
        }

        StartCoroutine(LeftNote());
    }

    private void OnRightNote(InputValue value)
    {
        if (!started
            || !enabled
            || dying
            || GameController.singleton.GetPaused())
        {
            return;
        }

        StartCoroutine(RightNote());
    }

    private void OnDRightNote(InputValue value)
    {
        if (!started
            || !enabled
            || dying
            || GameController.singleton.GetPaused())
        {
            return;
        }

        if (!SettingsController.singleton.dInput)
        {
            return;
        }

        StartCoroutine(RightNote());
    }

    private void OnDownNote(InputValue value)
    {
        if (!started
            || !enabled
            || dying
            || GameController.singleton.GetPaused())
        {
            return;
        }

        StartCoroutine(DownNote());
    }

    private void dDownNoteHandle(InputValue value)
    {
        if (!started 
            || !enabled 
            || dying 
            || GameController.singleton.GetPaused())
        {
            return;
        }

        if (!SettingsController.singleton.dInput)
        {
            return;
        }

        StartCoroutine(DownNote());
    }

    protected override void Move(float h, float v)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator StartRhythm()
    {
        yield return new WaitUntil(() => FindObjectOfType<StaffController>() != null);
        sCon = FindObjectOfType<StaffController>();
        sCon.StartSong();
        yield return new WaitForSeconds(startWait);
        rb.velocity = new Vector2(speed, 0);
        started = true;
        StartCoroutine(EndProcessor());
    }

    private IEnumerator UpNote()
    {
        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[0] * 0.25f);
        animator.SetInteger("dance", 0);
        animator.SetBool("dancing", true);

        yield return new WaitForSeconds(0.033333f);

        List<Collider2D> contacts = new List<Collider2D>();
        GetComponentInChildren<BoxCollider2D>().GetContacts(contacts);

        bool hit = false;
        for (int i = 0; i < contacts.ToArray().Length && !hit; i++)
        {
            NoteController nTemp = contacts[i].GetComponent<NoteController>();
            if (nTemp != null)
            {
                hit = true;
                if (nTemp.GetDir() == 0)
                {
                    int noteNum;
                    if (int.TryParse(nTemp.gameObject.name.Substring(12), out noteNum))
                    {
                        sCon.ProcInput(0, noteNum);
                        Destroy(nTemp.gameObject);
                    }
                }
                else
                {
                    StartCoroutine(Miss());
                }
            }
        }

        if (!hit)
        {
            StartCoroutine(Miss());
        }
    }

    private IEnumerator LeftNote()
    {
        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[1] * 0.25f);
        animator.SetInteger("dance", 1);
        animator.SetBool("dancing", true);

        yield return new WaitForSeconds(0.033333f);

        List<Collider2D> contacts = new List<Collider2D>();
        GetComponentInChildren<BoxCollider2D>().GetContacts(contacts);

        bool hit = false;
        for (int i = 0; i < contacts.ToArray().Length && !hit; i++)
        {
            NoteController nTemp = contacts[i].GetComponent<NoteController>();
            if (nTemp != null)
            {
                hit = true;
                if (nTemp.GetDir() == 1)
                {
                    int noteNum;
                    if (int.TryParse(nTemp.gameObject.name.Substring(12), out noteNum))
                    {
                        sCon.ProcInput(1, noteNum);
                        Destroy(nTemp.gameObject);
                    }
                }
                else
                {
                    StartCoroutine(Miss());
                }
            }
        }

        if (!hit)
        {
            StartCoroutine(Miss());
        }
    }

    private IEnumerator RightNote()
    {
        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[2] * 0.25f);
        animator.SetInteger("dance", 2);
        animator.SetBool("dancing", true);

        yield return new WaitForSeconds(0.033333f);

        List<Collider2D> contacts = new List<Collider2D>();
        GetComponentInChildren<BoxCollider2D>().GetContacts(contacts);

        bool hit = false;
        for (int i = 0; i < contacts.ToArray().Length && !hit; i++)
        {
            NoteController nTemp = contacts[i].GetComponent<NoteController>();
            if (nTemp != null)
            {
                hit = true;
                if (nTemp.GetDir() == 2)
                {
                    int noteNum;
                    if (int.TryParse(nTemp.gameObject.name.Substring(12), out noteNum))
                    {
                        sCon.ProcInput(2, noteNum);
                        Destroy(nTemp.gameObject);
                    }
                }
                else
                {
                    StartCoroutine(Miss());
                }
            }
        }

        if (!hit)
        {
            StartCoroutine(Miss());
        }
    }

    private IEnumerator DownNote()
    {
        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[3] * 0.25f);
        animator.SetInteger("dance", 3);
        animator.SetBool("dancing", true);

        yield return new WaitForSeconds(0.033333f);

        List<Collider2D> contacts = new List<Collider2D>();
        GetComponentInChildren<BoxCollider2D>().GetContacts(contacts);

        bool hit = false;
        for (int i = 0; i < contacts.ToArray().Length && !hit; i++)
        {
            NoteController nTemp = contacts[i].GetComponent<NoteController>();
            if (nTemp != null)
            {
                hit = true;
                if (nTemp.GetDir() == 3)
                {
                    int noteNum;
                    if (int.TryParse(nTemp.gameObject.name.Substring(12), out noteNum))
                    {
                        sCon.ProcInput(3, noteNum);
                        Destroy(nTemp.gameObject);
                    }
                }
                else
                {
                    Miss(nTemp);
                }
            }
        }

        if (!hit)
        {
            StartCoroutine(Miss());
        }
    }

    public IEnumerator EndProcessor()
    {
        yield return new WaitUntil(() => transform.position.x >= startPos.x + xDiff);
        animator.SetBool("dancing", false);
        transform.position = startPos + new Vector3(xDiff, 0, 0);
        rb.velocity = Vector2.zero;
    }

    public void Miss(NoteController note)
    {
        sCon.FixPlay(int.Parse(note.gameObject.name.Substring(12)));
        StartCoroutine(Miss());
    }

    public IEnumerator Miss()
    {
        if (!dying)
        {
            misses++;
            if (misses >= 3)
            {
                dying = true;
                animator.SetBool("dying", true);
            }
            animator.SetTrigger("miss");
            if (!dying)
            {
                source.clip = hitClip;
                source.Play();
                yield return new WaitUntil(() => !source.isPlaying);
            }
            source.clip = deathClip;
            if (dying)
            {
                StartCoroutine(Fail());
            }
            else
            {
                yield return new WaitForSeconds(0.45f);
                animator.ResetTrigger("miss");
            }
        }
    }

    public IEnumerator Fail()
    {
        dying = true;
        rb.velocity = Vector2.zero;
        //AkSoundEngine.PostEvent("Death_Jingle_MuteMusic", gameObject);
        //AkSoundEngine.PostEvent("Death_Jingle", gameObject);
        yield return new WaitForSeconds(1);
        GameController.singleton.Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
        {
            return;
        }

        if (collision.CompareTag("Ground"))
        {
            inGround = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!enabled)
        {
            return;
        }

        if (collision.CompareTag("Ground"))
        {
            inGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled)
        {
            return;
        }

        if (collision.CompareTag("Ground"))
        {
            inGround = false;
        }
    }

    public void SetStarted(bool val)
    {
        started = val;
    }

    public bool GetDying()
    {
        return dying;
    }
}
