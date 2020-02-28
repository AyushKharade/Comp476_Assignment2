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

    public GameObject GetBestCoverPoint(Transform pos)            // get best cover point relative to position
    {
        GameObject best = null;
        float coverValue = 0;
        float distance = float.MaxValue;
        foreach (GameObject gb in clusterElements)
        {
            if (gb.GetComponent<Node>().coverValue > coverValue) 
                //&&
                //Vector3.Distance(pos.position, gb.transform.position) < distance)
            {
                coverValue = gb.GetComponent<Node>().coverValue;
                best = gb;
            }
        }
        return best;
    }

    public GameObject GetRandomNode()
    {
        int r = Random.Range(0, clusterExits.Count);
        /*
        if (clusterElements[r] == null)
        {
            return clusterElements[0].gameObject;
        }
        return clusterElements[r].gameObject;
        */
        return clusterExits[r];
    }
}
