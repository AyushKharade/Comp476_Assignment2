using System.Collections;
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

    void Start()
    {
        NPCRef = GetComponent<NPC_Pathfinder>();
    }

    void Update()
    {
        
    }

    public Transform RequestDestination()
    {
        // returns a proper destination for NPC to follow. Can interupt current path to take a new path.
        NPC_Pathfinder targetScriptRef = ChaseTarget.GetComponent<NPC_Pathfinder>();

        if (NPCRef.currentCluster.transform.name != targetScriptRef.currentCluster.transform.name)
        //if (NPCRef.currentCluster.transform.name != "R1Cluster")
        {
            // then go to this cluster.
            isTargetInSameCluster = false;
            int r = Random.Range(0,targetScriptRef.closestNode.GetComponent<Node>().cluster.GetComponent<Cluster>().clusterExits.Count);
            return targetScriptRef.closestNode.GetComponent<Node>().cluster.GetComponent<Cluster>().clusterExits[r].transform;
        }
        else
        {
            isTargetInSameCluster = true;
            //same cluster
            // if close and in line of sight, seek
            if (Vector3.Distance(ChaseTarget.transform.position, transform.position) < 15f)
            {
                //check line of sight
                // seek
                return ChaseTarget.transform;
            }
            else
                return targetScriptRef.FindClosestNode(ChaseTarget.transform.position).transform;
        }

    }
}
