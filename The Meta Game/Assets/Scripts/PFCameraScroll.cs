using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFCameraScroll : MonoBehaviour
{
    /// <summary>
    /// Object reference for the Player object
    /// </summary>
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.x > transform.position.x)
        {
            transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
        }
    }
}
