using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMove : MonoBehaviour
{
    private float timeInWorld;
    private Animator animator;
    public float maxTimeInWorld;
    public float speed;
    public float hitstun;
    public int damage;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GetComponent<FightingHitbox>().hitstun = hitstun;
        GetComponent<FightingHitbox>().damage = damage;
        timeInWorld = 0.0f;

        if (CompareTag("PlayerHitbox")){
            animator.SetBool("player", true);
        }
        else
        {
            animator.SetBool("player", false);
        }
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
}
