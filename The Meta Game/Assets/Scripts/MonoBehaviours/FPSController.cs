using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [Tooltip("Determines the rate at which the mouse moves the camera")]
    public float sensitivity;

    [Tooltip("The factor by which FOV is divided when zooming")]
    public float zoomFactor;

    [Tooltip("Inverts Y axis for mouselook controls")]
    public bool invertY;

    [Tooltip("The damage dealt when shot connects")]
    public int damage;

    [Tooltip("Which layers can be shot by FPS mode")]
    public LayerMask mask;

    /// <summary>
    /// Unzoomed camera field of view
    /// </summary>
    private float fovNormal;

    /// <summary>
    /// Zoomed camera field of view
    /// </summary>
    private float fovZoomed;

    /// <summary>
    /// Object reference to GameObject holding image of zoom reticle
    /// </summary>
    private GameObject zoomRet;

    /// <summary>
    /// Stores aspect ratio to make zoom more efficient
    /// </summary>
    private float aspect;

    private float x, y;

    private Controls controls;

    private void OnEnable()
    {
        controls = new Controls();

        controls.Player.LookX.performed += LookXHandle;
        controls.Player.LookX.canceled += LookXHandle;
        controls.Player.LookY.performed += LookYHandle;
        controls.Player.LookY.canceled += LookYHandle;
        controls.Player.Zoom.started += ZoomPerfHandle;
        controls.Player.Zoom.canceled += ZoomCancHandle;
        controls.Player.Fire.started += FireHandle;

        controls.Player.LookX.Enable();
        controls.Player.LookY.Enable();
        controls.Player.Zoom.Enable();
        controls.Player.Fire.Enable();
    }

    private void OnDisable()
    {
        controls.Player.LookX.performed -= LookXHandle;
        controls.Player.LookX.canceled -= LookXHandle;
        controls.Player.LookY.performed -= LookYHandle;
        controls.Player.LookY.canceled -= LookYHandle;
        controls.Player.Zoom.started -= ZoomPerfHandle;
        controls.Player.Zoom.canceled -= ZoomCancHandle;
        controls.Player.Fire.started -= FireHandle;

        controls.Player.LookX.Disable();
        controls.Player.LookY.Disable();
        controls.Player.Zoom.Disable();
        controls.Player.Fire.Disable();
    }

    private void LookXHandle (InputAction.CallbackContext context)
    {
        x = context.action.ReadValue<float>();
    }

    private void LookYHandle (InputAction.CallbackContext context)
    {
        y = invertY ? context.action.ReadValue<float>() : context.action.ReadValue<float>() * -1;
    }

    private void ZoomPerfHandle (InputAction.CallbackContext context)
    {
        zoomRet.SetActive(true);
        Camera.main.projectionMatrix = Matrix4x4.Perspective(fovZoomed, aspect, 0.3f, 1000.0f);
    }

    private void ZoomCancHandle (InputAction.CallbackContext context)
    {
        zoomRet.SetActive(false);
        Camera.main.projectionMatrix = Matrix4x4.Perspective(fovNormal, aspect, 0.3f, 1000.0f);
    }

    private void FireHandle (InputAction.CallbackContext context)
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, mask);

        if (hit.collider == null)
        {
            return;
        }
        else if (hit.collider.CompareTag("Player"))
        {
            GameController.singleton.Hit(damage);
        }
        else
        {
            hit.collider.GetComponentInParent<PFEnemy>().Hit(damage);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        fovNormal = Camera.main.fieldOfView;
        fovZoomed = fovNormal / zoomFactor;

        RectTransform[] rects = GameObject.Find("Canvas_Scene").GetComponentsInChildren<RectTransform>(true);
        foreach (RectTransform rect in rects)
        {
            if (rect.name.Equals("ZoomRet"))
            {
                zoomRet = rect.gameObject;
            }
        }

        aspect = (float)Screen.width / (float)Screen.height;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(y * sensitivity * Time.deltaTime, x * sensitivity * Time.deltaTime, 0);
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, 0);
    }
}
