﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMove : MonoBehaviour
{
    private float timeInWorld;
    public float maxTimeInWorld;
    public float speed;

    private void Start()
    {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
