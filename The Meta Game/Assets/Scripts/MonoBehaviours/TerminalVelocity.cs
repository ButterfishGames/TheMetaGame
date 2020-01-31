using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalVelocity : MonoBehaviour
{
    [Tooltip("Maximum velocity which the attached Rigidbody2D will be able to reach")]
    [Range(0, 30)]
    public float terminalVelocity;

    private Rigidbody2D rb;

    private BoxCollider2D col;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (rb.velocity.magnitude > terminalVelocity)
        {
            rb.velocity *= terminalVelocity / rb.velocity.magnitude;
        }
    }
}