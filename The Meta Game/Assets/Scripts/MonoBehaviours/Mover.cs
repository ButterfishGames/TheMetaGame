using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Mover : MonoBehaviour
{
    public float gravity = 3;

    public Controls controls;

    [Tooltip("The layer on which normal level collision will be checked")]
    public LayerMask blockingLayer;

    /// <summary>
    /// The BoxCollider2D component attached to this object
    /// </summary>
    protected CapsuleCollider2D col;

    /// <summary>
    /// The Rigidbody2D component attached to this object
    /// </summary>
    protected Rigidbody2D rb;

    protected Animator animator;

    public float hor, ver;
    public float hRaw, vRaw;

    protected bool stickUp;

    protected virtual void OnEnable()
    {
        controls = new Controls();

        controls.Player.MoveH.performed += MoveHHandle;
        controls.Player.MoveH.canceled += MoveHHandle;
        controls.Player.MoveV.performed += MoveVHandle;
        controls.Player.MoveV.canceled += MoveVHandle;
        controls.Player.LStick.performed += JoyMoveHandle;
        controls.Player.LStick.canceled += JoyMoveHandle;
        controls.Player.DPad.performed += JoyMoveHandle;
        controls.Player.DPad.canceled += JoyMoveHandle;

        controls.Player.MoveH.Enable();
        controls.Player.MoveV.Enable();
        controls.Player.LStick.Enable();
        controls.Player.DPad.Enable();
    }

    protected virtual void OnDisable()
    {
        controls.Player.MoveH.performed -= MoveHHandle;
        controls.Player.MoveH.canceled -= MoveHHandle;
        controls.Player.MoveV.performed -= MoveVHandle;
        controls.Player.MoveV.canceled -= MoveVHandle;
        controls.Player.LStick.performed -= JoyMoveHandle;
        controls.Player.LStick.canceled -= JoyMoveHandle;
        controls.Player.DPad.performed -= JoyMoveHandle;
        controls.Player.DPad.canceled -= JoyMoveHandle;

        controls.Player.MoveH.Disable();
        controls.Player.MoveV.Disable();
        controls.Player.LStick.Disable();
        controls.Player.DPad.Disable();
    }

    private void MoveHHandle(InputAction.CallbackContext context)
    {
        hRaw = context.ReadValue<float>();
    }

    private void MoveVHandle(InputAction.CallbackContext context)
    {
        vRaw = context.ReadValue<float>();
        stickUp = false;
    }

    private void JoyMoveHandle(InputAction.CallbackContext context)
    {
        Vector2 stickRaw = context.ReadValue<Vector2>();

        float threshold = Mathf.Cos(5.0f / 18.0f * Mathf.PI); // 5/18 * pi radians = 50 degrees; cos(50 deg) = sin(40 deg)

        if (stickRaw.x >= threshold)
        {
            hRaw = 1;
        }
        else if (stickRaw.x <= -threshold)
        {
            hRaw = -1;
        }
        else
        {
            hRaw = 0;
        }

        if (stickRaw.y >= threshold)
        {
            stickUp = true;
            vRaw = 1;
        }
        else if (stickRaw.y <= -threshold)
        {
            stickUp = false;
            vRaw = -1;
        }
        else
        {
            vRaw = 0;
        }
    }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        col = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
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

        hor = AxisProc(hor, hRaw);
        ver = AxisProc(ver, vRaw);
        
        if (ver <= 0 && stickUp)
        {
            stickUp = false;
        }

        if (hor != 0 || ver != 0)
        {
            Move(hor, ver);
        }
    }

    protected abstract void Move(float h, float v);

    public virtual Animator GetAnimator()
    {
        return animator;
    }

    private float AxisProc(float axis, float rawAxis)
    {
        float res;

        if (rawAxis == 0)
        {
            if (axis > 0)
            {
                res = Mathf.Clamp01(axis - gravity * Time.deltaTime);
            }
            else if (axis < 0)
            {
                res = Mathf.Clamp(axis + gravity * Time.deltaTime, -1, 0);
            }
            else
            {
                res = 0;
            }
        }
        else
        {
            if ((rawAxis > 0 && axis < 0) || (rawAxis < 0 && axis > 0))
            {
                res = 0;
            }
            else
            {
                res = Mathf.Clamp(axis + rawAxis * gravity * Time.deltaTime, -1, 1);
            }
        }

        return res;
    }
}
