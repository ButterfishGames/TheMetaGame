using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleTrigger : MonoBehaviour
{
    public static CastleTrigger singleton;

    public bool castle = false;

    // Start is called before the first frame update
    void Start()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Player"))
        {
            castle = true;
        }
    }
}
