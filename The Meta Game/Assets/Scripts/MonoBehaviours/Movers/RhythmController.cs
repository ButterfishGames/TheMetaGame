﻿using System.Collections;
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

    private float xDiff = 20;

    private bool dying = false;

    public bool inGround = false;

    protected override void OnEnable()
    {
        started = false;
        baseY = transform.position.y - 0.05f;
        startPos = transform.position;

        inGround = false;

        controls = new Controls();

        //controls.Player.StartRhythm.performed += StartRhythmHandle;
        controls.Player.UpNote.performed += UpNoteHandle;
        controls.Player.LeftNote.performed += LeftNoteHandle;
        controls.Player.RightNote.performed += RightNoteHandle;
        controls.Player.DownNote.performed += DownNoteHandle;

        //controls.Player.StartRhythm.Enable();
        controls.Player.UpNote.Enable();
        controls.Player.LeftNote.Enable();
        controls.Player.RightNote.Enable();
        controls.Player.DownNote.Enable();
    }

    protected override void OnDisable()
    {
        //controls.Player.StartRhythm.performed -= StartRhythmHandle;
        controls.Player.UpNote.performed -= UpNoteHandle;
        controls.Player.LeftNote.performed -= LeftNoteHandle;
        controls.Player.RightNote.performed -= RightNoteHandle;
        controls.Player.DownNote.performed -= DownNoteHandle;

        //controls.Player.StartRhythm.Disable();
        controls.Player.UpNote.Disable();
        controls.Player.LeftNote.Disable();
        controls.Player.RightNote.Disable();
        controls.Player.DownNote.Disable();
    }

    /*private void StartRhythmHandle(InputAction.CallbackContext context)
    {
        StartCoroutine(StartRhythm());
    }*/

    private void UpNoteHandle(InputAction.CallbackContext context)
    {
        if (!started || dying || GameController.singleton.GetPaused())
        {
            return;
        }

        StartCoroutine(UpNote());
    }

    private void LeftNoteHandle(InputAction.CallbackContext context)
    {
        if (!started || dying || GameController.singleton.GetPaused())
        {
            return;
        }

        StartCoroutine(LeftNote());
    }

    private void RightNoteHandle(InputAction.CallbackContext context)
    {
        if (!started || dying || GameController.singleton.GetPaused())
        {
            return;
        }

        StartCoroutine(RightNote());
    }

    private void DownNoteHandle(InputAction.CallbackContext context)
    {
        if (!started || dying || GameController.singleton.GetPaused())
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
        FindObjectOfType<StaffController>().StartSong();
        yield return new WaitForSeconds(startWait);
        rb.velocity = new Vector2(speed, 0);
        started = true;
    }

    private IEnumerator UpNote()
    {
        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[0] * 0.25f);

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
                        if(FindObjectOfType<StaffController>().ProcInput(0, noteNum))
                        {
                            Destroy(nTemp.gameObject);
                        }
                    }
                }
                else
                {
                    StartCoroutine(Fail());
                }
            }
        }

        if (!hit)
        {
            StartCoroutine(Fail());
        }
    }

    private IEnumerator LeftNote()
    {
        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[1] * 0.25f);

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
                        if (FindObjectOfType<StaffController>().ProcInput(1, noteNum))
                        {
                            Destroy(nTemp.gameObject);
                        }
                    }
                }
                else
                {
                    StartCoroutine(Fail());
                }
            }
        }

        if (!hit)
        {
            StartCoroutine(Fail());
        }
    }

    private IEnumerator RightNote()
    {
        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[2] * 0.25f);

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
                        if (FindObjectOfType<StaffController>().ProcInput(2, noteNum))
                        {
                            Destroy(nTemp.gameObject);
                        }
                    }
                }
                else
                {
                    StartCoroutine(Fail());
                }
            }
        }

        if (!hit)
        {
            StartCoroutine(Fail());
        }
    }

    private IEnumerator DownNote()
    {
        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[3] * 0.25f);

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
                        if (FindObjectOfType<StaffController>().ProcInput(3, noteNum))
                        {
                            Destroy(nTemp.gameObject);
                        }
                    }
                }
                else
                {
                    StartCoroutine(Fail());
                }
            }
        }

        if (!hit)
        {
            StartCoroutine(Fail());
        }
    }

    public IEnumerator Win()
    {
        yield return new WaitUntil(() => transform.position.x >= startPos.x + xDiff);
        transform.position = startPos + new Vector3(xDiff, 0, 0);
        rb.velocity = Vector2.zero;
    }

    public IEnumerator Fail()
    {
        if (!dying)
        {
            dying = true;
            rb.velocity = Vector2.zero;
            FindObjectOfType<StaffController>().source.Stop();
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(1);
            GameController.singleton.Die();
        }
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
}
