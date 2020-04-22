using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadKnightBehavior : MonoBehaviour
{
    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public Animator GetAnimator()
    {
        return animator;
    }
}
