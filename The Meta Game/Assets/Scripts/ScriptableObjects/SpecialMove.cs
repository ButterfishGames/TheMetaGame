using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMove : MonoBehaviour
{
    public float timeInWorld;
    public float addDistance;

    private void Start()
    {
        timeInWorld = 0.0f;
    }

    void Update()
    {
        if (timeInWorld > 10)
        {
            Destroy(gameObject);
        }
        else
        {
            timeInWorld += Time.deltaTime;
        }
        transform.Translate(new Vector2(transform.position.x + Time.deltaTime * addDistance, transform.position.y), Space.World);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
