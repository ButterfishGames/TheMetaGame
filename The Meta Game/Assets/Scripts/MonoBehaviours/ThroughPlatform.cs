using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.layer = LayerMask.NameToLayer("ThroughTemp");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.layer = LayerMask.NameToLayer("Player");
    }
}
