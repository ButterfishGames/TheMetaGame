using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFEnemy : MonoBehaviour
{
    public enum Direction
    {
        right,
        left
    };

    [Tooltip("The direction the enemy starts facing")]
    public Direction startDir;

    /// <summary>
    /// Used to determine current direction; 1 is right, -1 is left
    /// </summary>
    private int dir = 1;

    /// <summary>
    /// Reference to Rigidbody2D component on object
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Reference to BoxCollider2D component on object
    /// </summary>
    private BoxCollider2D col;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        switch(startDir)
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

    // Update is called once per frame
    void Update()
    {
        if (GameController.singleton.GetPaused())
        {
            return;
        }

        RaycastHit2D hit;

        Vector2 dVec = new Vector2(dir, -1).normalized;
        
        hit = Physics2D.Raycast(transform.position, dVec, 1, ~(1<<LayerMask.NameToLayer("Enemy")));
        
        if (hit.collider == null)
        {
            Turn();
        }
        else
        {
            rb.velocity = new Vector2(dir, rb.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            rb.velocity = Vector2.zero;
            PFController pfCon = collision.collider.GetComponent<PFController>();
            pfCon.StartCoroutine(pfCon.Die());
        }

        if (collision.collider.CompareTag("Enemy"))
        {
            Turn();
        }
    }

    private void Turn()
    {
        dir *= -1;
        transform.Rotate(0, 180, 0);
    }
}
