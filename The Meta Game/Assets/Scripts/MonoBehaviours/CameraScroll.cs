using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    [Tooltip("The difference in y position between the camera and the player")]
    public float yOffset;

    [Tooltip("The lowest x and y coordinates the camera should be able to reach")]
    public Vector2 min;

    [Tooltip("The highest x and y coordinates the camera should be able to reach")]
    public Vector2 max;

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
        float posX, posY;

        switch (GameController.singleton.equipped)
        {
            case GameController.GameMode.platformer:
                if (player.position.x > transform.position.x)
                {
                    posX = player.position.x;
                }
                else
                {
                    posX = transform.position.x;
                }
                posY = player.position.y + yOffset;
                break;

            case GameController.GameMode.rpg:
                posX = player.position.x;
                posY = player.position.y + yOffset;
                break;

            default:
                posX = transform.position.x;
                posY = transform.position.y;
                break;
        }

        posX = Mathf.Clamp(posX, min.x, max.x);
        posY = Mathf.Clamp(posY, min.y, max.y);

        float xDiff = posX - transform.position.x;
        float yDiff = posY - transform.position.y;

        Parallax[] pObjs = FindObjectsOfType<Parallax>();
        foreach (Parallax obj in pObjs)
        {
            obj.UpdatePos(xDiff, yDiff);
        }

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
}
