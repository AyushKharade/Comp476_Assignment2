using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public GameObject StartNode;
    public GameObject EndNode;

    Vector3 startNodePos;
    Vector3 endNodePos;

    public Transform NodesParent;

    bool startedPathfinding;

    List<GameObject> OpenSet;
    List<GameObject> ClosedSet;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!startedPathfinding)
            MouseInput();
    }



    void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // raycast
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.tag == "Node")
                {
                    Debug.Log("Clicked a node: "+hit.collider.transform.name);
                }
            }
        }
    }
    /*
     * Algorithm
     * 
     * Add current node to open list
     * 
     * Loop:
     * Pick currentNode from openList with the lowest fcost.
     * remove currentNode from Open and add to closed
     * 
     */



    public void StartPathfinding()
    {
        startedPathfinding = true;

        startNodePos = StartNode.transform.position;
        endNodePos = EndNode.transform.position;

        OpenSet.Add(StartNode);

        GameObject curNode = null;


        while (OpenSet.Count > 0)
        {
            float lowestFCost = 10000000;
            foreach (GameObject g in OpenSet)
            {
                if(curNode==null)
                    g.GetComponent<Node>().CalculateFCost(startNodePos, endNodePos);
                else
                    g.GetComponent<Node>().CalculateFCost(curNode.transform.position, endNodePos);


                if (g.GetComponent<Node>().GetFCost() < lowestFCost)
                {
                    lowestFCost = g.GetComponent<Node>().GetFCost();
                    curNode = g.gameObject;
                }
            }

            // you have the node with the lowest fcost.  place it in closed.
            OpenSet.Remove(curNode);
            ClosedSet.Add(curNode);

            // is this the final node?
            if (Vector3.Distance(curNode.transform.position, endNodePos) == 0)
            {
                Debug.Log("Found end target node.");
            }
            else
            {
                // scan all neighbours
                foreach (GameObject ng in curNode.GetComponent<Node>().neighbours)
                {
                    if (!ClosedSet.Contains(ng))
                    {
                        // if its not in the closed list, add to open list
                        OpenSet.Add(ng);
                        ng.GetComponent<Node>().CalculateFCost(startNodePos, endNodePos);
                        ng.GetComponent<Node>().SetParentPath(curNode);
                    }
                }
            }
        }
    }

    
}
