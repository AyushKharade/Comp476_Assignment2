using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Pathfinder : MonoBehaviour
{
    // has a referece to the A* object.
    // uses this reference to request a path to a point gotten from decision making.
    // common script for both npcs

    public float mSpeed=4;

    public enum type { Chaser, Runner};
    public type NPCType;

    public GameObject AStarRef;
    Pathfinding AStarScript;

    public GameObject AllNodesParent;

    int pathTraverseCounter=0;
    Transform currentTarget;
    public Transform currentDestination;
    public GameObject closestNode;

    public List<Transform> followPath = new List<Transform>();
    //public List<Vector3> followPath = new List<Vector3>();

    bool hasDestination;
    bool moving;
    bool orienting;

    int traverseIndex = 0;

    Animator animator;


    void Start()
    {
        AStarScript = AStarRef.GetComponent<Pathfinding>();

        animator = GetComponent<Animator>();

        int r = Random.Range(0, AllNodesParent.transform.childCount);
        currentDestination = AllNodesParent.transform.GetChild(r);
        currentDestination.GetComponent<MeshRenderer>().material.color = Color.red;
        closestNode = FindClosestNode();

        if (closestNode.transform.name != currentDestination.transform.name)
        {
            followPath = AStarScript.ClusterPathFind(closestNode, currentDestination.gameObject);
            if (followPath != null)
                hasDestination = true;
            else
                Debug.Log(transform.name + " received a null path from position: " + closestNode.transform.name + "to destination: "+currentDestination.transform.name);
        }
    }

    

    void Update()
    {
        if (hasDestination && !moving)
        {
            traverseIndex = 0;
            if (followPath==null)
            {
                hasDestination = false;
                moving = false;
            }
            else
            {
                currentTarget = followPath[0];
                moving = true;
            }
        }
        else if (hasDestination && moving)
            MoveToTarget();
        else if (!hasDestination)
        {
            GoToNewPosition();
        }

        // anim
        if (moving && !orienting)
        {
            if (animator.GetFloat("Locomotion") < 1)
                animator.SetFloat("Locomotion", animator.GetFloat("Locomotion") + 0.04f);
        }
        else
        {
            if (animator.GetFloat("Locomotion") > 0)
                animator.SetFloat("Locomotion", animator.GetFloat("Locomotion") - 0.04f);
        }

        if (!hasDestination)
        {
            if (Input.GetKeyDown(KeyCode.T))
                GoToNewPosition();
        }
    }

    void MoveToTarget()
    {

        Vector3 dir = transform.forward;
        if (Vector3.Distance(transform.position, currentTarget.position) > 0.2f)
        {
            dir = (currentTarget.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dir) < 10)
            {
                transform.parent.Translate(dir * mSpeed * Time.deltaTime);
                orienting = false;
            }
            else
                orienting = true;

        }
        else
        {
            Debug.Log("Reached path index: " + traverseIndex);
            if (traverseIndex < followPath.Count - 1)
            {
                traverseIndex++;
                currentTarget = followPath[traverseIndex];
            }
        }

        
        if (traverseIndex == followPath.Count || Vector3.Distance(transform.position, currentDestination.position) < 0.2f)
        {
            Debug.Log("Destination Reached.");
            hasDestination = false;
            moving = false;
            currentDestination.GetComponent<Node>().ResetMaterial();
        }

        // align orietation
        Align(dir);
    }




    //------------------------ Steering behaviors ------
    void Align(Vector3 dir)
    {
        Quaternion lookDirection;

        //set quaternion to this dir
        lookDirection = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, 4);

    }


    void GoToNewPosition()
    {
        hasDestination = true;
        int r = Random.Range(0, AllNodesParent.transform.childCount);
        currentDestination = AllNodesParent.transform.GetChild(r);
        followPath = AStarScript.ClusterPathFind(FindClosestNode(),currentDestination.gameObject);
    }


    GameObject FindClosestNode()
    {
        //overlap sphere
        Collider[] arr = Physics.OverlapSphere(transform.position,15f);
        GameObject ClosestNode = null;
        float closestDistance=float.MaxValue;
        foreach (Collider col in arr)
        {
            if (col.tag == "Node")
            {
                if (Vector3.Distance(transform.position, col.transform.position) < closestDistance)
                {
                    Vector3 rayOutPos = transform.position;
                    rayOutPos.y += 0.2f;
                    Vector3 dir = (col.transform.position - transform.position).normalized;

                    RaycastHit hitobj;
                    Physics.Raycast(rayOutPos,dir, out hitobj);     // Make sure its visible
                    //Debug.Log("Ray out towards "+col.name+" hit: "+hitobj.collider.name);

                    if (hitobj.collider.tag=="Node" && hitobj.collider.name == col.name)
                    {
                        ClosestNode = col.gameObject;
                        closestDistance = Vector3.Distance(transform.position, col.transform.position);
                    }
                    
                }
            }
        }
        return ClosestNode;
        //Debug.Log("Closest Node: "+closestNode.transform.name);
    }
}
