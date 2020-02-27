using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Mover : MonoBehaviour
{
    public static float gravity = 3;

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
    protected float hRaw, vRaw;

    protected virtual void OnEnable()
    {
        controls = new Controls();

        controls.Player.MoveH.performed += MoveHHandle;
        controls.Player.MoveH.canceled += MoveHHandle;
        controls.Player.MoveV.performed += MoveVHandle;
        controls.Player.MoveV.canceled += MoveVHandle;

        controls.Player.MoveH.Enable();
        controls.Player.MoveV.Enable();
    }

    protected virtual void OnDisable()
    {
        controls.Player.MoveH.performed -= MoveHHandle;
        controls.Player.MoveH.canceled -= MoveHHandle;
        controls.Player.MoveV.performed -= MoveVHandle;
        controls.Player.MoveV.canceled -= MoveVHandle;

        controls.Player.MoveH.Disable();
        controls.Player.MoveV.Disable();
    }

    private void MoveHHandle(InputAction.CallbackContext context)
    {
        hRaw = context.ReadValue<float>();
    }

    private void MoveVHandle(InputAction.CallbackContext context)
    {
        vRaw = context.ReadValue<float>();
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

        hor = AxisProc(hor, hRaw);
        ver = AxisProc(ver, vRaw);

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
            res = Mathf.Clamp(axis + rawAxis * gravity * Time.deltaTime, -1, 1);
        }

        return res;
    }
}
