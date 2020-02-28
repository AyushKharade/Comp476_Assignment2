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

    public Transform bestCoverNode;

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
        if (!isCurrentClusterEmpty || Vector3.Distance(closestChaser.position,transform.position) < 10f)
            NPCRef.safe = false;
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
        if (!isCurrentClusterEmpty)                 // you are not alone in current cluster so move
        {
            // move to a random nearby cluster
            // get current cluster's neighbors.
            List<GameObject> neighbors = NPCRef.currentCluster.GetComponent<Node>().neighbours;
            // find neighbour who if further from closest chaser
            GameObject furthestNeighbor = null;
            //float dist = Vector3.Distance(closestChaser.transform.position, furthestNeighbor.transform.position);
            float dist = 0;//Vector3.Distance(closestChaser.transform.position, furthestNeighbor.transform.position);
            foreach (GameObject gb in neighbors)
            {
                if (Vector3.Distance(closestChaser.transform.position, gb.transform.position) > dist)
                {
                    furthestNeighbor = gb;
                    dist = Vector3.Distance(closestChaser.transform.position, gb.transform.position);
                }
            }

            GameObject node = furthestNeighbor.GetComponent<Node>().cluster;
            Debug.Log("Furthest neigbor: "+furthestNeighbor.name);

            Transform bestNeighborNode = furthestNeighbor.GetComponent<Cluster>().GetBestCoverPoint(transform).transform;
            Debug.Log("Bestneighbor's best node: "+bestNeighborNode.name);

            return bestNeighborNode;

        }
        else
        {
            // find best spot in current cluster, wait at the best node, until its not safe anymore
            if(Vector3.Distance(closestChaser.position, transform.position) > 10)
                NPCRef.safe = true;

            Transform best = NPCRef.currentCluster.GetComponent<Cluster>().GetBestCoverPoint(transform).transform;
            bestCoverNode = best;
            return best;
        }
    }
}
