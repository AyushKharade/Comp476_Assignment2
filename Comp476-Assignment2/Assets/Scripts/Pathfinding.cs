using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pathfinding : MonoBehaviour
{

    public GameObject StartNode;
    public GameObject EndNode;

    Vector3 startNodePos;
    Vector3 endNodePos;

    [Header("Materials")]
    public Material GreenMat;
    public Material RedMat;
    public Material BlueMat;


    [Header("Selection Highlight Prefab")]
    public GameObject highlightPrefab;

    public Transform CurrentSelectedNode;
    public GameObject currentSelectedText;
    GameObject selectedHighlight;


    bool startedPathfinding;

    public List<GameObject> OpenSet;
    public List<GameObject> ClosedSet;

    LinkedList<Transform> Path = new LinkedList<Transform>();       // NPC Follows this path.

    void Start()
    {
        OpenSet = new List<GameObject>();
        ClosedSet = new List<GameObject>();

        //GameObject currentSelectedText=GameObject.FindGameObjectWithTag("CurrentSelected");
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
                    //Debug.Log("Clicked a node: "+hit.collider.transform.name);
                    if (selectedHighlight != null)
                        Destroy(selectedHighlight.gameObject);
                    selectedHighlight= Instantiate(highlightPrefab, hit.collider.transform.position,Quaternion.identity);

                    CurrentSelectedNode = hit.collider.transform;
                    currentSelectedText.GetComponent<Text>().text = CurrentSelectedNode.transform.name;
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

    public void SetStartNode()
    {
        if (StartNode == null)
        {
            StartNode = CurrentSelectedNode.gameObject;
            StartNode.GetComponent<MeshRenderer>().material = GreenMat;
        }
        else
        {
            //reset older node's color back
            StartNode.GetComponent<Node>().ResetMaterial();
            StartNode = CurrentSelectedNode.gameObject;
            StartNode.GetComponent<MeshRenderer>().material = GreenMat;
        }

        Destroy(selectedHighlight);
        selectedHighlight = null;
        currentSelectedText.GetComponent<Text>().text = "None";
    }
    public void SetEndNode()
    {
        if (EndNode == null)
        {
            EndNode = CurrentSelectedNode.gameObject;
            EndNode.GetComponent<MeshRenderer>().material = RedMat;
        }
        else
        {
            //reset its color back
            //reset older node's color back
            EndNode.GetComponent<Node>().ResetMaterial();
            EndNode = CurrentSelectedNode.gameObject;
            EndNode.GetComponent<MeshRenderer>().material = RedMat;
        }

        Destroy(selectedHighlight);
        selectedHighlight = null;
        currentSelectedText.GetComponent<Text>().text = "None";
    }

    
    //-------------------------------------------------------------------------------------------------------------------


    public void StartPathfinding()
    {
        startedPathfinding = true;

        OpenSet.Add(StartNode.transform.gameObject);

        startNodePos = StartNode.transform.position;
        endNodePos = EndNode.transform.position;

        GameObject curNode = null;
        bool pathFound=false;

        while (OpenSet.Count > 0)
        {
            float lowestFCost = 10000000;
            foreach (GameObject g in OpenSet)
            {
                if (curNode == null)
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
                pathFound = true;
                TracePath();
                break;
            }
            else
            {
                //ProcessingNode = curNode.transform;
                // scan all neighbours
                foreach (GameObject ng in curNode.GetComponent<Node>().neighbours)
                {
                    if (!OpenSet.Contains(ng))
                    {
                        // if its not in the closed list, add to open list
                        if (!ClosedSet.Contains(ng))
                        {
                            OpenSet.Add(ng);
                            ng.GetComponent<Node>().CalculateFCost(startNodePos, endNodePos);
                            ng.GetComponent<Node>().SetParentPath(curNode);
                        }
                    }
                }

            }
        }

        if (!pathFound)
            Debug.Log("Path not found");
    }

    void TracePath()
    {
        // we are in the end game now, keep back tracking from end node, going through parents untill you find start node.
        GameObject curNode = EndNode;
        while (curNode.GetComponent<Node>().Parent != null)
        {
            Debug.Log(">> "+curNode.transform.name);
            Path.AddFirst(curNode.transform);         
            GameObject Parent= curNode.GetComponent<Node>().Parent;
            Debug.DrawLine(curNode.transform.position, Parent.transform.position, Color.green, 5f, false);
            curNode = Parent;
        }
    }


    public void ResetPathfinding()
    {
        StartNode.GetComponent<Node>().ResetMaterial();
        EndNode.GetComponent<Node>().ResetMaterial();

        StartNode = null;
        EndNode = null;
        startedPathfinding = false;

        OpenSet.Clear();
        ClosedSet.Clear();
    }
}
