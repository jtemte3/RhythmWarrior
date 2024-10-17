using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    public float speed;
    Animator animator;
    private float gripValue;
    private float triggerValue;
    private float gripCurrent;
    private float triggerCurrent;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animateHand();
    }

    internal void setGrip(float v)
    {
        gripValue = v;
    }

    internal void setTrigger(float v)
    {
        triggerValue = v;
    }

    void animateHand()
    {
        if (gripCurrent != gripValue)
        {
            gripCurrent = Mathf.MoveTowards(gripCurrent, gripValue, Time.deltaTime * speed);
            animator.SetFloat("Grip", gripCurrent);
        }

        if (triggerCurrent != triggerValue)
        {
            triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerValue, Time.deltaTime * speed);
            animator.SetFloat("Trigger", triggerCurrent);
        }
    }
}
