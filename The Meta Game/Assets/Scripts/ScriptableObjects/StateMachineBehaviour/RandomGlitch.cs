using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGlitch : StateMachineBehaviour
{
    public float minTime, maxTime;

    private float time;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        animator.SetBool("glitch", false);
        time = Time.time + Random.Range(minTime, maxTime);

        animator.SetFloat("glitchTime", Random.Range(0.0f, 0.75f));
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (Time.time > time)
        {
            animator.SetBool("glitch", true);
        }
    }
}
