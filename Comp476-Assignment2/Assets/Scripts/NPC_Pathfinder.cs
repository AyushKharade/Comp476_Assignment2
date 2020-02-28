﻿using System.Collections;
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

        followPath = AStarScript.ClusterPathFind(closestNode,currentDestination.gameObject);
        hasDestination = true;
        
    }

    

    void Update()
    {
        if (hasDestination && !moving)
        {
            traverseIndex = 0;
            currentTarget = followPath[0];
            moving = true;
        }
        else if (hasDestination && moving)
            MoveToTarget();

        // anim
        if (moving)
        {
            if (animator.GetFloat("Locomotion") < 1)
                animator.SetFloat("Locomotion", animator.GetFloat("Locomotion") + 0.04f);
        }
        else
        {
            if (animator.GetFloat("Locomotion") > 0)
                animator.SetFloat("Locomotion", animator.GetFloat("Locomotion") - 0.04f);
        }
    }

    void MoveToTarget()
    {
        Vector3 FaceDir=transform.forward;
        if (Vector3.Distance(transform.position, currentTarget.position) > 0.2f)
        {
            Vector3 dir = (currentTarget.position - transform.position).normalized;
            FaceDir = dir;
            //transform.Translate(dir * mSpeed * Time.deltaTime);
            transform.parent.Translate(dir * mSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log("Reached path index: " + traverseIndex);
            if (traverseIndex < followPath.Count-1)
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
        Align(FaceDir);
    }




    //------------------------ Steering behaviors ------
    void Align(Vector3 dir)
    {
        Quaternion lookDirection;

        //set quaternion to this dir
        lookDirection = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, 4);

    }

    GameObject FindClosestNode()
    {
        //overlap sphere
        Collider[] arr = Physics.OverlapSphere(transform.position,10f);
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
