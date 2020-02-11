using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMove : MonoBehaviour
{
    private float timeInWorld;
    public float maxTimeInWorld;
    public float speed;
    public float hitstun;
    public int damage;

    private void Start()
    {
        GetComponent<FightingHitbox>().hitstun = hitstun;
        GetComponent<FightingHitbox>().damage = damage;
        timeInWorld = 0.0f;
    }

    void Update()
    {
        if (timeInWorld > maxTimeInWorld)
        {
            Destroy(gameObject);
        }
        else
        {
            timeInWorld += Time.deltaTime;
        }
        transform.Translate(Time.deltaTime * speed, 0, 0);
    }

    private void OnColliderEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("Before Destroy");
            Destroy(gameObject);
            Debug.Log("After Destroy");
        }
    }
}
