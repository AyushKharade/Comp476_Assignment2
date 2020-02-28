using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerBehavior : MonoBehaviour
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
    NPC_Pathfinder NPCRef;

    public Transform chaser1Ref;
    public Transform chaser2Ref;
    public Transform chaser3Ref;
    public Transform closestChaser;

    public float c1Distance;
    public float c2Distance;
    public float c3Distance;

    [Header("States")]
    public bool isCurrentClusterEmpty;    // if true, stand at the highest cover value node.
    public bool isBeingSeeked;            // if anyone is actively seeking, run faster.

    void Start()
    {
        NPCRef = GetComponent<NPC_Pathfinder>();
    }

    void Update()
    {
        GetDistances();
        closestChaser = GetClosestChaser();

        //CurrentClusterCheck();
        isCurrentClusterEmpty= ClusterCheck(transform.GetComponent<NPC_Pathfinder>().currentCluster.transform);
        SeekCheck();

    }

    
    void SeekCheck()
    {
        if (chaser1Ref.GetComponent<NPC_Pathfinder>().seekingTarget
            ||
            chaser2Ref.GetComponent<NPC_Pathfinder>().seekingTarget
            ||
            chaser3Ref.GetComponent<NPC_Pathfinder>().seekingTarget
            )
        {
            isBeingSeeked = true;
        }
        else
            isBeingSeeked = false;
    }
    /*
    void CurrentClusterCheck()
    {
        if (chaser1Ref.GetComponent<NPC_Pathfinder>().currentCluster.name != transform.GetComponent<NPC_Pathfinder>().currentCluster.name
            &&
            chaser2Ref.GetComponent<NPC_Pathfinder>().currentCluster.name != transform.GetComponent<NPC_Pathfinder>().currentCluster.name
            &&
            chaser3Ref.GetComponent<NPC_Pathfinder>().currentCluster.name != transform.GetComponent<NPC_Pathfinder>().currentCluster.name)
        {
            isCurrentClusterEmpty = true;
        }
        else
            isCurrentClusterEmpty = false;
    }
    */

    bool ClusterCheck(Transform cluster)
    {
        if (chaser1Ref.GetComponent<NPC_Pathfinder>().currentCluster.name != cluster.name
            &&
            chaser2Ref.GetComponent<NPC_Pathfinder>().currentCluster.name != cluster.name
            &&
            chaser3Ref.GetComponent<NPC_Pathfinder>().currentCluster.name != cluster.name)
        {
            return true;
        }
        else
            return false;
    }

    void GetDistances()
    {
        c1Distance = Vector3.Distance(transform.position,chaser1Ref.position);
        c2Distance = Vector3.Distance(transform.position,chaser2Ref.position);
        c3Distance = Vector3.Distance(transform.position,chaser3Ref.position);
    }

    Transform GetClosestChaser()
    {
        if (c1Distance < c2Distance && c1Distance < c3Distance)
            return chaser1Ref;
        else if (c2Distance < c3Distance)
            return chaser2Ref;
        else
            return chaser3Ref;
    }
    public Transform RequestDestination()
    {
        // acts as an FSM, tells what to do.
        if (!isCurrentClusterEmpty)
        {
            // move to a random nearby cluster
            // check which cluster neighbour is empty, move there
            List<GameObject> neighbors = NPCRef.currentCluster.GetComponent<Node>().neighbours;
            GameObject emptyCluster = null;
            foreach (GameObject gb in neighbors)
            {
                if (ClusterCheck(gb.transform))
                {
                    emptyCluster = gb;
                    break;
                }
            }
            if (emptyCluster = null)
                emptyCluster = neighbors[Random.Range(0,neighbors.Count)];

            // you have a cluster to move to, move to its fastest exit
            return emptyCluster.GetComponent<Cluster>().GetFastestExit(transform.position, emptyCluster.transform.position).transform;
        }
        else
        {
            // find best spot in current cluster
            return NPCRef.currentCluster.GetComponent<Cluster>().GetBestCoverPoint(transform).transform;
        }
    }
}
