using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    protected Animator animator;

    protected virtual void Start()
    { 
        animator = GetComponentInChildren<Animator>();
    }

    public Animator GetAnimator()
    {
        return animator;
    }
}
