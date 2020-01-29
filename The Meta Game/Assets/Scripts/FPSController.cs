using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        float x = Input.GetAxis("Mouse X");
        float y = invertY ? Input.GetAxis("Mouse Y") : -1 * Input.GetAxis("Mouse Y");

        transform.Rotate(y * sensitivity * Time.deltaTime, x * sensitivity * Time.deltaTime, 0);
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, 0);

        if (Input.GetButtonDown("Zoom"))
        {
            zoomRet.SetActive(true);
            Camera.main.projectionMatrix = Matrix4x4.Perspective(fovZoomed, aspect, 0.3f, 1000.0f);
        }

        if (Input.GetButtonUp("Zoom"))
        {
            zoomRet.SetActive(false);
            Camera.main.fieldOfView = fovNormal;
            Camera.main.projectionMatrix = Matrix4x4.Perspective(fovNormal, aspect, 0.3f, 1000.0f);
        }

        if (Input.GetButtonDown("Fire"))
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
    }
}
