using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TEST_gravity : MonoBehaviour
{
    public static float gravity = 3;

    private float hRaw, vRaw;
    private float hor, ver;

    private Controls controls;

    private void OnEnable()
    {
        controls = new Controls();

        controls.Player.MoveH.performed += MoveHHandle;
        controls.Player.MoveH.canceled += MoveHHandle;
        controls.Player.MoveV.performed += MoveVHandle;
        controls.Player.MoveV.canceled += MoveVHandle;

        controls.Player.MoveH.Enable();
        controls.Player.MoveV.Enable();
    }

    private void OnDisable()
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
        hRaw = context.action.ReadValue<float>();
    }

    private void MoveVHandle(InputAction.CallbackContext context)
    {
        vRaw = context.action.ReadValue<float>();
    }

    private void Update()
    {
        hor = AxisProc(hor, hRaw);
        ver = AxisProc(ver, vRaw);

        if (hor != 0)
        {
            Debug.Log(hor);
        }
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

    /* IMPORTANT: TESTING SCRIPT
    
    private bool testingH, testingV;
    private int countH, countV;
    private float prevH, prevV;


    private Controls controls;

    private void OnEnable()
    {
        controls = new Controls();

        controls.Player.MoveH.performed += MoveHHandle;
        controls.Player.MoveH.canceled += MoveHHandle;
        controls.Player.MoveV.performed += MoveVHandle;
        controls.Player.MoveV.canceled += MoveVHandle;

        controls.Player.MoveH.Enable();
        controls.Player.MoveV.Enable();
    }

    private void OnDisable()
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
        testingH = true;
        countH = 0;
        Debug.Log("Action: " + context.action.ReadValue<float>());
    }

    private void MoveVHandle(InputAction.CallbackContext context)
    {
        testingV = true;
        countV = 0;
        Debug.Log("Action: " + context.action.ReadValue<float>());
    }

    private void Update()
    {
        if (testingH)
        {
            Debug.Log("Axis: " + Input.GetAxis("Horizontal") + ", frame " + countH);
            Debug.Log("Axis dif: " + (Input.GetAxis("Horizontal") - prevH));
            Debug.Log("Axis dif adj: " + ((Input.GetAxis("Horizontal") - prevH) / Time.deltaTime));
            if (Input.GetAxis("Horizontal") == 0)
            {
                testingH = false;
            }
            else
            {
                countH++;
                prevH = Input.GetAxis("Horizontal");
            }
        }

        if (testingV)
        {
            Debug.Log("Axis: " + Input.GetAxis("Vertical") + ", frame " + countV);
            if (Input.GetAxis("Vertical") == 0)
            {
                testingV = false;
            }
            else
            {
                countV++;
                prevV = Input.GetAxis("Vertical");
            }
        }
    }
    */
    // RESULT: Each frame the key is pressed, axis += rawAxis * gravity * Time.deltaTime
    // RESULT: Each frame the key is not pressed, axis movest toward 0 at rate of gravity * Time.deltaTime
}
