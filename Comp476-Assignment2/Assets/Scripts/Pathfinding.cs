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
    public Material YellowMat;


    [Header("Selection Highlight Prefab")]
    public GameObject highlightPrefab;

    public Transform CurrentSelectedNode;
    public GameObject currentSelectedText;
    GameObject selectedHighlight;

    public GameObject AllNodesParent;
    public GameObject AllClustersParent;


    bool startedPathfinding;

    public List<GameObject> OpenSet;
    public HashSet<GameObject> ClosedSet;

    LinkedList<Transform> Path = new LinkedList<Transform>();       // NPC Follows this path.

    float distanceCovered=0;

    // auto turn of highlights while showing neighbours

    bool showingNeighbours;
    float showingTimer;

    [Header("Heuristics")]
    public bool clusterHeuristic;     // if enable, call a seperate function

    void Start()
    {
        OpenSet = new List<GameObject>();
        ClosedSet = new HashSet<GameObject>();

        //GameObject currentSelectedText=GameObject.FindGameObjectWithTag("CurrentSelected");
        InitStartCosts();
    }

    // Update is called once per frame
    void Update()
    {
        if(!startedPathfinding)
            MouseInput();

        if (showingNeighbours)
        {
            showingTimer += Time.deltaTime;
            if (showingTimer > 3f)
            {
                showingNeighbours=false;
                showingTimer = 0;

                int count = AllNodesParent.transform.childCount;
                for (int i = 0; i < count; i++)
                {
                    AllNodesParent.transform.GetChild(i).GetComponent<Node>().ResetMaterial();
                }
            }
        }
    }


    void InitStartCosts()
    {
        int count = AllNodesParent.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            AllNodesParent.transform.GetChild(i).GetComponent<Node>().CalculateFCost(startNodePos,endNodePos);
            AllNodesParent.transform.GetChild(i).GetComponent<Node>().Parent = null;
        }
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

    public void Pathfind()
    {
        if (clusterHeuristic)
        {
            StartPathfindingCluster();
        }
        else
        {
            StartPathfindingRegular();
        }
    }


    // second video i saw
    void StartPathfindingRegular()
    {
        startedPathfinding = true;
        OpenSet.Clear();
        ClosedSet.Clear();
        InitStartCosts();
        OpenSet.Add(StartNode);

        while (OpenSet.Count > 0)
        {
            GameObject curNode = OpenSet[0];
            // check for any other nodes having smaller cost that this
            foreach (GameObject g in OpenSet)
            {
                if (g.GetComponent<Node>().GetFCost() < curNode.GetComponent<Node>().GetFCost() && g.transform.name!=curNode.transform.name
                    &&
                    (g.GetComponent<Node>().hCost < curNode.GetComponent<Node>().hCost))
                {
                    curNode = g;
                }
            }
            
            OpenSet.Remove(curNode);
            ClosedSet.Add(curNode);
            if(curNode.transform.name != StartNode.transform.name && curNode.transform.name!=EndNode.transform.name)
                curNode.GetComponent<MeshRenderer>().material = YellowMat;

            //check if it is the target node
            if (curNode.transform.name == EndNode.transform.name)
            {
                Debug.Log("Found end node.");
                break;
            }
            // if not final node, traverse all neighbours
            foreach (GameObject nb in curNode.GetComponent<Node>().neighbours)
            {
                if (!ClosedSet.Contains(nb))        // skip closed list neighbours
                {
                    // find cost of moving to neighbout
                    float moveCost = curNode.GetComponent<Node>().gCost + Vector3.Distance(curNode.transform.position, nb.transform.position);

                    //if (moveCost < nb.GetComponent<Node>().GetFCost() || !OpenSet.Contains(nb))
                    if (moveCost < nb.GetComponent<Node>().gCost || !OpenSet.Contains(nb))
                    {
                        nb.GetComponent<Node>().gCost = moveCost;
                        nb.GetComponent<Node>().hCost = Vector3.Distance(nb.transform.position, endNodePos);
                        nb.GetComponent<Node>().Parent = curNode;

                        if (!OpenSet.Contains(nb))
                        {
                            OpenSet.Add(nb);
                            if(nb.transform.name!=EndNode.transform.name)
                                nb.GetComponent<MeshRenderer>().material = BlueMat;
                        }
                    }
                }
            }
        }
        TracePath2();

    }


    // overloaded method to return a list of path
    List<Transform> StartPathfindingRegular(GameObject start, GameObject end)
    {
        List<Transform> pathList = new List<Transform>();

        startedPathfinding = true;
        OpenSet.Clear();
        ClosedSet.Clear();
        InitStartCosts();
        OpenSet.Add(start);

        while (OpenSet.Count > 0)
        {
            GameObject curNode = OpenSet[0];
            // check for any other nodes having smaller cost that this
            foreach (GameObject g in OpenSet)
            {
                if (g.GetComponent<Node>().GetFCost() < curNode.GetComponent<Node>().GetFCost() && g.transform.name != curNode.transform.name
                    &&
                    (g.GetComponent<Node>().hCost < curNode.GetComponent<Node>().hCost))
                {
                    curNode = g;
                }
            }

            OpenSet.Remove(curNode);
            ClosedSet.Add(curNode);
            if (curNode.transform.name != StartNode.transform.name && curNode.transform.name != EndNode.transform.name)
                curNode.GetComponent<MeshRenderer>().material = YellowMat;

            //check if it is the target node
            if (curNode.transform.name == end.transform.name)
            {
                Debug.Log("Found end node.");
                break;
            }
            // if not final node, traverse all neighbours
            foreach (GameObject nb in curNode.GetComponent<Node>().neighbours)
            {
                if (!ClosedSet.Contains(nb))        // skip closed list neighbours
                {
                    // find cost of moving to neighbout
                    float moveCost = curNode.GetComponent<Node>().gCost + Vector3.Distance(curNode.transform.position, nb.transform.position);

                    //if (moveCost < nb.GetComponent<Node>().GetFCost() || !OpenSet.Contains(nb))
                    if (moveCost < nb.GetComponent<Node>().gCost || !OpenSet.Contains(nb))
                    {
                        nb.GetComponent<Node>().gCost = moveCost;
                        nb.GetComponent<Node>().hCost = Vector3.Distance(nb.transform.position, endNodePos);
                        nb.GetComponent<Node>().Parent = curNode;

                        if (!OpenSet.Contains(nb))
                        {
                            OpenSet.Add(nb);
                            if (nb.transform.name != end.transform.name)
                                nb.GetComponent<MeshRenderer>().material = BlueMat;
                        }
                    }
                }
            }
        }


        // add the path in list
        GameObject currNode = end;

        pathList.Add(EndNode.transform);
        while (currNode.GetComponent<Node>().Parent != null)
        {
            pathList.Add(currNode.GetComponent<Node>().Parent.transform);
            currNode = currNode.GetComponent<Node>().Parent;
        }

        pathList.Reverse();

        return pathList;

    }


    // cluster heuristics:
    void StartPathfindingCluster()
    {
        // check if start node and end nodes are both in the same cluster, if so, do regular path finding.
        GameObject clusterStart = StartNode.GetComponent<Node>().cluster;
        GameObject clusterEnd = EndNode.GetComponent<Node>().cluster;
        if (clusterStart.transform.name == clusterEnd.transform.name)
        {
            StartPathfindingRegular();
            Debug.Log("Path within the same cluster");
        }
        else
        {
            StartNode = clusterStart;
            EndNode = clusterEnd;
            StartPathfindingRegular();
            Debug.Log("Path among different clusters.");

            // store path of clusters
            List<Transform> clusterPath = StartPathfindingRegular(clusterStart,clusterEnd);

        }
    }



    void TracePath()
    {
        // we are in the end game now, keep back tracking from end node, going through parents untill you find start node.
        GameObject curNode = EndNode;
        int count = 0;
        while (curNode.GetComponent<Node>().Parent != null)
        {
            count++;
            Path.AddFirst(curNode.transform);         
            GameObject Parent= curNode.GetComponent<Node>().Parent;

            Debug.DrawLine(curNode.transform.position, Parent.transform.position, Color.green, 5f, false);
            distanceCovered += (Vector3.Distance(Parent.transform.position, curNode.transform.position));
            curNode = Parent;
        }
        

        Debug.Log("Distance Covered through this path: "+distanceCovered +"units");
        Debug.Log("Traced back"+count+" steps");
    }

    public void TracePath2()
    {
        List<Transform> path=new List<Transform>();
        GameObject curNode = EndNode;

        path.Add(EndNode.transform);
        while (curNode.GetComponent<Node>().Parent!=null)
        {
            path.Add(curNode.GetComponent<Node>().Parent.transform);
            curNode = curNode.GetComponent<Node>().Parent;
        }

        path.Reverse();

        // now draw
        for (int i = 0; i < path.Count-1; i++)
        {
            Debug.DrawLine(path[i].position,path[i+1].position,Color.green, 5f, false);
        }
    }


    public void ResetPathfinding()
    {
        StartNode.GetComponent<Node>().ResetMaterial();
        EndNode.GetComponent<Node>().ResetMaterial();

        StartNode = null;
        EndNode = null;
        startedPathfinding = false;

        CurrentSelectedNode = null;

        OpenSet.Clear();
        ClosedSet.Clear();

        Path.Clear();

        // clear parent nodes.
        for (int i = 0; i < AllNodesParent.transform.childCount;i++)
        {
            AllNodesParent.transform.GetChild(i).GetComponent<Node>().Parent = null;
            AllNodesParent.transform.GetChild(i).GetComponent<Node>().ResetMaterial();
        }
        for (int i = 0; i < AllClustersParent.transform.childCount; i++)
        {
            AllClustersParent.transform.GetChild(i).GetComponent<Node>().Parent = null;
            AllClustersParent.transform.GetChild(i).GetComponent<Node>().ResetMaterial();
        }



        distanceCovered = 0;
    }


    public void ShowNeighbours()
    {
        if (CurrentSelectedNode != null)
        {
            foreach (GameObject nb in CurrentSelectedNode.GetComponent<Node>().neighbours)
            {
                nb.GetComponent<MeshRenderer>().material = YellowMat;
                showingNeighbours = true;
            }
        }
    }
}
