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
        return AllNodesParent.transform.GetChild(r);

    }
}
