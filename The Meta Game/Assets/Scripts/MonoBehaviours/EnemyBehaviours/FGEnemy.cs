using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGEnemy : EnemyBehaviour
{
    /// <summary>
    /// Sprite renderer to see if the enemy is visible or not so we know whether to include them in the fight.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Fighting variable to determine whether the enemy should be fighting the player ot not.
    /// </summary>
    private bool fighting;

    /// <summary>
    /// Players X position so the sprite can face towardsthe player depending on what side they are on.
    /// </summary>
    private float playerPosX;

    public enum Direction
    {
        right,
        left
    };

    [Tooltip("The direction the enemy starts facing")]
    public Direction startDir;

    [Tooltip("The max HP enemies have in gamemodes where they can take damage")]
    public int maxHP;

    [Tooltip("The time in seconds it takes for the red flash when damaged to fade")]
    public float damageFadeTime;

    /// <summary>
    /// Used to make damage flash calculations more efficient
    /// </summary>
    private float inverseDamageFadeTime;

    /// <summary>
    /// The current HP a given enemy has
    /// </summary>
    private int currHP;

    /// <summary>
    /// Used to determine current direction; 1 is right, -1 is left
    /// </summary>
    private int dir;

    /// <summary>
    /// Reference to Rigidbody2D component on object
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Reference to BoxCollider2D component on object
    /// </summary>
    private BoxCollider2D col;

    /// <summary>
    /// Main Camera to tell if enemy is within the viewport
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// This variable is to determine whether the enemy started within the view when switching to fighting mode
    /// </summary>
    public bool changedInView;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<Camera>();
        playerPosX = GameObject.FindGameObjectWithTag("Player").gameObject.transform.position.x;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        if (spriteRenderer.isVisible)
        {
            //fighting = true;
            if (transform.position.x > playerPosX)
            {
                startDir = Direction.left;
            }
            else
            {
                startDir = Direction.right;
            }

            switch (startDir)
            {
                case Direction.right:
                    dir = 1;
                    break;

                case Direction.left:
                    dir = -1;
                    break;

                default:
                    Debug.Log("ERROR: INVALID STARTING DIRECTION");
                    break;
            }
        }
        else
        {
            //fighting = false;
        }

        currHP = maxHP;

        inverseDamageFadeTime = 1.0f / damageFadeTime;
        //Debug.Log(name + "Is visible");
    }

    void Update()
    {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        if (changedInView == true) {
            fighting = true;
            if (viewPos.y < 0.0f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            fighting = false;
        }
        if (fighting == true)
        {

        }
    }
}
