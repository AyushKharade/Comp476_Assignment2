using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NavMeshChaser : MonoBehaviour
{
    public Transform chaseTarget;
    bool hasDestination;
    float distanceToTarget;

    NavMeshAgent NavigationAgent;
    Vector3 currentNavMeshTarget;

    public float UpdateTargetFrequency=5f;
    public float timer=0f;

    public float velocityMagnitude;

    Animator animator;

    public Text TaggedCount;
    public int TaggedCountInt;

    void Start()
    {
        animator = GetComponent<Animator>();
        NavigationAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        AnimationControls();

        if (!hasDestination)
        {
            hasDestination = true;
            currentNavMeshTarget = chaseTarget.transform.position;
            NavigationAgent.SetDestination(currentNavMeshTarget);
        }

        if (hasDestination)
        {
            //if (Vector3.Distance(transform.position, currentNavMeshTarget) > 15f)
                timer += Time.deltaTime;
                if (timer > UpdateTargetFrequency)
                {
                    timer = 0;
                    hasDestination = false;
                    NavigationAgent.ResetPath();
                }
        }
    }

    void AnimationControls()
    {
        float value = NavigationAgent.velocity.magnitude;
        velocityMagnitude = value;

        // cap it somehow, velocity > 1.5 is 1
        value = (velocityMagnitude / 2.0f);
        if (value > 1)
            value = 1;
        animator.SetFloat("Locomotion",value);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Runner")
        {
            TaggedCountInt++;
            TaggedCount.text = "Tagged Count:" + TaggedCountInt;
        }
    }
}
