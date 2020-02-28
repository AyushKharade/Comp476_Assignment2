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

        // random for now
        int r = Random.Range(0, AllNodesParent.transform.childCount);
        NPCRef.hasDestination = true;
        return AllNodesParent.transform.GetChild(r);

    }
}
