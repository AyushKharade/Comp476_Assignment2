﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserBehavior : MonoBehaviour
{
    /*
     * This class makes decisions and sets destination for movement for every Not-it NPC.
     * 
     * 
     * 
     * 
     */
    public Transform AllNodesParent;
    public Transform AllClusterParent;

    public Transform ChaseTarget;

    NPC_Pathfinder NPCRef;

    public bool isTargetInSameCluster;
    public bool seekTarget;
    public float distanceToTarget;

    public Transform curTargetCluster=null;
    public Transform curDestinationCluster=null;

    [Header("Settings")]
    public bool useClosestORRandomClusterExit;
    [TextArea(3, 6)]
    public string ClosestOrRandom;

    void Start()
    {
        NPCRef = GetComponent<NPC_Pathfinder>();
    }

    void Update()
    {
        distanceToTarget = Vector3.Distance(transform.position,ChaseTarget.position);

        // if distance is really small, interupt path following and seek target.
        /*
        Vector3 rayOutPos = transform.position;
        rayOutPos.y += 0.2f;
        Vector3 dir = (ChaseTarget.transform.position - transform.position).normalized;

        RaycastHit hitobj;
        Physics.Raycast(rayOutPos, dir, out hitobj);     // Make sure its visible
        */

        if (distanceToTarget < 3.5f)
        {
            seekTarget = true;
            NPCRef.StopMovement();
            NPCRef.SeekTarget(ChaseTarget.transform);
        }
        else
            seekTarget = false;


        UpdateTargetCluster();
        if (curTargetCluster!=null && curDestinationCluster!=null)
        {
            if (curTargetCluster.name != curDestinationCluster.name)
            {
                //NPCRef.StopMovement();
                //NPCRef.ChangeClusterTarget();          // causes bugs
            }
        }
    }

    void UpdateTargetCluster()
    {
        curTargetCluster = ChaseTarget.GetComponent<NPC_Pathfinder>().currentCluster.transform;
    }

    public Transform RequestDestination()
    {
        // returns a proper destination for NPC to follow. Can interupt current path to take a new path.
        NPC_Pathfinder targetScriptRef = ChaseTarget.GetComponent<NPC_Pathfinder>();

        if (NPCRef.currentCluster.transform.name != targetScriptRef.currentCluster.transform.name)
        {
            // then go to this cluster.
            isTargetInSameCluster = false;
            curDestinationCluster = targetScriptRef.closestNode.GetComponent<Node>().cluster.GetComponent<Cluster>().transform;
            if (useClosestORRandomClusterExit)
                return targetScriptRef.closestNode.GetComponent<Node>().cluster.GetComponent<Cluster>().GetFastestExit(transform.position, ChaseTarget.transform.position).transform;
            else
            {
                Cluster temp = targetScriptRef.closestNode.GetComponent<Node>().cluster.GetComponent<Cluster>();
                return temp.clusterExits[Random.Range(0, temp.clusterExits.Count)].transform;
            }
        }
        else
        {
            //same cluster
            isTargetInSameCluster = true;
            return targetScriptRef.FindClosestNode(ChaseTarget.transform.position).transform;
        }

    }
}
