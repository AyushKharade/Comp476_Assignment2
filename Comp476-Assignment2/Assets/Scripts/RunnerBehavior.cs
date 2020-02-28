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

        CurrentClusterCheck();
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

        return transform;
    }
}
