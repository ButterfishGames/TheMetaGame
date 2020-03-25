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

    protected override void OnEnable()
    {
        started = false;
        baseY = transform.position.y - 0.05f;

        controls = new Controls();

        controls.Player.StartRhythm.performed += StartRhythmHandle;
        controls.Player.UpNote.performed += UpNoteHandle;
        controls.Player.LeftNote.performed += LeftNoteHandle;
        controls.Player.RightNote.performed += RightNoteHandle;
        controls.Player.DownNote.performed += DownNoteHandle;

        controls.Player.StartRhythm.Enable();
        controls.Player.UpNote.Enable();
        controls.Player.LeftNote.Enable();
        controls.Player.RightNote.Enable();
        controls.Player.DownNote.Enable();
    }

    protected override void OnDisable()
    {
        controls.Player.StartRhythm.performed -= StartRhythmHandle;
        controls.Player.UpNote.performed -= UpNoteHandle;
        controls.Player.LeftNote.performed -= LeftNoteHandle;
        controls.Player.RightNote.performed -= RightNoteHandle;
        controls.Player.DownNote.performed -= DownNoteHandle;

        controls.Player.StartRhythm.Disable();
        controls.Player.UpNote.Disable();
        controls.Player.LeftNote.Disable();
        controls.Player.RightNote.Disable();
        controls.Player.DownNote.Disable();
    }

    private void StartRhythmHandle(InputAction.CallbackContext context)
    {
        StartCoroutine(StartRhythm());
    }

    private void UpNoteHandle(InputAction.CallbackContext context)
    {
        if (!started)
        {
            return;
        }

        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[0] * 0.25f);
    }

    private void LeftNoteHandle(InputAction.CallbackContext context)
    {
        if (!started)
        {
            return;
        }

        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[1] * 0.25f);
    }

    private void RightNoteHandle(InputAction.CallbackContext context)
    {
        if (!started)
        {
            return;
        }

        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[2] * 0.25f);
    }

    private void DownNoteHandle(InputAction.CallbackContext context)
    {
        if (!started)
        {
            return;
        }

        transform.position = new Vector3(transform.position.x, baseY + StaffController.NOTE_HEIGHTS[3] * 0.25f);
    }

    protected override void Move(float h, float v)
    {
        // Nothing to see here
    }

    private IEnumerator StartRhythm()
    {
        FindObjectOfType<StaffController>().StartSong();
        yield return new WaitForSeconds(startWait);
        rb.velocity = new Vector2(speed, 0);
        started = true;
    }
}
