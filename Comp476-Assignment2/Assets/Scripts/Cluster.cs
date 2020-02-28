using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    public string clusterName;

    public List<GameObject> clusterElements=new List<GameObject>();
    public List<GameObject> clusterExits=new List<GameObject>();


    public GameObject GetFastestExit(Vector3 startPos, Vector3 endPos)
    {
        //return exit closest to given position

        GameObject exitNode = null;
        float gCost = 2000;
        float hCost = 2000;

        foreach (GameObject ex in clusterExits)
        {
            float exitGCost = Vector3.Distance(startPos, ex.transform.position);
            float exitHCost = Vector3.Distance(endPos, ex.transform.position);

            if ((gCost + hCost) > (exitGCost + exitHCost))
            {
                exitNode = ex;
                gCost = exitGCost;
                hCost = exitHCost;
            }
        }

        // you have the fastest exit.
        if (exitNode == null)
        {
            Debug.Log("Didnt find shortest exit");
        }
        return exitNode;
    }
}
