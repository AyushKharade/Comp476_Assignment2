using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshRunner : MonoBehaviour
{
    public Transform chaseTarget1;
    public Transform chaseTarget2;
    public Transform chaseTarget3;

    public Transform AllNodesParent;

    bool hasDestination;
    float distanceToTarget;

    NavMeshAgent NavigationAgent;
    Vector3 currentNavMeshTarget;
    public string currentNodeTarget;

    public float UpdateTargetFrequency;
    public float timer = 0f;

    public float velocityMagnitude;

    public Transform avgLocSphere;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        NavigationAgent = GetComponent<NavMeshAgent>();

        // hide nodes mesh renderers
        for (int i = 0; i < AllNodesParent.transform.childCount; i++)
        {
            AllNodesParent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void Update()
    {
        AnimationControls();

        if (!hasDestination)
        {
            hasDestination = true;
            Vector3 CoM = GetCenterOfMass();
            currentNavMeshTarget = GetRunToNode(CoM);
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


    Vector3 GetCenterOfMass()
    {
        Vector3 avgLocation = Vector3.zero;
        float sumX = 0, sumY = 0, sumZ = 0;

        avgLocation.x += chaseTarget1.position.x;
        avgLocation.y += chaseTarget1.position.y;
        avgLocation.z += chaseTarget1.position.z;

        avgLocation.x += chaseTarget2.position.x;
        avgLocation.y += chaseTarget2.position.y;
        avgLocation.z += chaseTarget2.position.z;

        avgLocation.x += chaseTarget3.position.x;
        avgLocation.y += chaseTarget3.position.y;
        avgLocation.z += chaseTarget3.position.z;

        //Debug.Log("AVG Loc: " + avgLocation);
        avgLocSphere.transform.position = avgLocation;

    

        return avgLocation;
    }

    Vector3 GetRunToNode(Vector3 avgLocation)
    {
        // traver through all nodes, to find node furthest away from avg location
        Transform node = AllNodesParent.GetChild(0);
        float distance = Vector3.Distance(node.position,avgLocation);

        for (int i = 1; i < AllNodesParent.childCount; i++)
        {
            if (Vector3.Distance(AllNodesParent.GetChild(i).position, avgLocation) > distance)
            {
                distance = Vector3.Distance(AllNodesParent.GetChild(i).position, avgLocation);
                node = AllNodesParent.GetChild(i);
            }
        }
        currentNodeTarget = node.name;
        return node.position;
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

   
}
