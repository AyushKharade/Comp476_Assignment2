using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Pathfinder : MonoBehaviour
{
    // has a referece to the A* object.
    // uses this reference to request a path to a point gotten from decision making.
    // common script for both npcs

    public float mSpeed=4;

    public enum type { Chaser, Runner};
    public type NPCType;

    public GameObject AStarRef;
    Pathfinding AStarScript;

    public GameObject AllNodesParent;

    int pathTraverseCounter=0;
    Transform currentTarget;
    public Transform currentDestination;
    public Transform closestNode;

    //public List<Transform> followPath = new List<Transform>();
    public List<Vector3> followPath = new List<Vector3>();
    public List<int> numbers = new List<int>();

    bool hasDestination;


    void Start()
    {

        //Debug.Log("" + AStarScript.scriptfunctionaccessTest);

        //AStarRef = GameObject.FindGameObjectWithTag("A*");
        /*
        if (AStarRef != null)
            Debug.Log("AStarRef is not null");
        //AStarScript = AStarRef.GetComponent<Pathfinding>();

        int r = Random.Range(0, AllNodesParent.transform.childCount);
        currentDestination = AllNodesParent.transform.GetChild(r);
        Debug.Log("Destination Set: " + currentDestination.name);

        closestNode = FindClosestNode().transform;
        //followPath = AStarRef.GetComponent<Pathfinding>().ClusterPathFind(closestNode.gameObject, currentTarget.gameObject);
        numbers = AStarScript.ReturnTest();
        Debug.Log(AStarRef.GetComponent<Pathfinding>().scriptfunctionaccessTest);
    */
    }

    

    void Update()
    {
        /*
        if (!hasDestination)
        {
            if (followPath != null)
            {
                hasDestination = true;
                closestNode = FindClosestNode().transform;
                followPath = AStarScript.ClusterPathFind(closestNode.gameObject, currentTarget.gameObject);

                currentTarget = followPath[0];
                Debug.Log("Received Path");
            }
            else
            {
                Debug.Log("Path not received.");
            }
        }
        else
        {
            // one by one follow path
            if (Vector3.Distance(currentTarget.position, transform.position) == 0 && pathTraverseCounter != followPath.Count)
            {
                pathTraverseCounter++;
                currentTarget = followPath[pathTraverseCounter];


                Vector3 moveDir = (currentTarget.position - transform.position).normalized;
                transform.Translate(moveDir * mSpeed * Time.deltaTime);
            }
        }
        */

        if (Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("" + AStarScript.scriptfunctionaccessTest);
            //AStarRef = GameObject.FindGameObjectWithTag("A*");
            //AStarScript = AStarRef.GetComponent<Pathfinding>();

            int r = Random.Range(0, AllNodesParent.transform.childCount);
            currentDestination = AllNodesParent.transform.GetChild(r);
            Debug.Log("Destination Set: " + currentDestination.name);

            closestNode = FindClosestNode().transform;
            numbers = AStarRef.GetComponent<Pathfinding>().ReturnTest();
            int n=AStarRef.GetComponent<Pathfinding>().ReturnTestJustNumber();
            Debug.Log(""+n);
            Debug.Log(AStarRef.GetComponent<Pathfinding>().scriptfunctionaccessTest);
            followPath= AStarRef.GetComponent<Pathfinding>().ClusterPathFind(closestNode.gameObject, currentDestination.gameObject);

        }
    }

    GameObject FindClosestNode()
    {
        //overlap sphere
        Collider[] arr = Physics.OverlapSphere(transform.position,10f);
        GameObject ClosestNode = null;
        float closestDistance=float.MaxValue;
        foreach (Collider col in arr)
        {
            if (col.tag == "Node")
            {
                if (Vector3.Distance(transform.position, col.transform.position) < closestDistance)
                {
                    Vector3 rayOutPos = transform.position;
                    rayOutPos.y += 0.2f;
                    Vector3 dir = (col.transform.position - transform.position).normalized;

                    RaycastHit hitobj;
                    Physics.Raycast(rayOutPos,dir, out hitobj);     // Make sure its visible
                    //Debug.Log("Ray out towards "+col.name+" hit: "+hitobj.collider.name);

                    if (hitobj.collider.tag=="Node" && hitobj.collider.name == col.name)
                    {
                        ClosestNode = col.gameObject;
                        closestDistance = Vector3.Distance(transform.position, col.transform.position);
                    }
                    
                }
            }
        }
        return ClosestNode;
        //Debug.Log("Closest Node: "+closestNode.transform.name);
    }
}
