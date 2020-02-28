using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Pathfinder : MonoBehaviour
{
    // has a referece to the A* object.
    // uses this reference to request a path to a point gotten from decision making.
    // common script for both npcs

    public float mSpeed=4;
    public bool allowMovement=true;

    public enum type { Chaser, Runner};
    public type NPCType;

    public GameObject AStarRef;
    Pathfinding AStarScript;

    public GameObject AllNodesParent;

    int pathTraverseCounter=0;
    Transform currentTarget;
    public Transform currentDestination;
    public GameObject closestNode;
    public GameObject currentCluster;

    public List<Transform> followPath = new List<Transform>();

    public bool hasDestination;
    bool moving;
    bool orienting;
    public bool seekingTarget;

    int traverseIndex = 0;

    Animator animator;

    RunnerBehavior RBehavior;
    ChaserBehavior CBehavior;

    // temp timer
    float RequestTimer = 0;
    float seekTimer = 0;         // incase npc got stuck seeking for too long, reset and start pathfinding again.


    private void Awake()
    {
        closestNode = FindClosestNode(transform.position);
        currentCluster = closestNode.GetComponent<Node>().cluster;
    }

    void Start()
    {
        AStarScript = AStarRef.GetComponent<Pathfinding>();
        animator = GetComponent<Animator>();

        //closestNode = FindClosestNode(transform.position);
        //currentCluster = closestNode.GetComponent<Node>().cluster;

        if (NPCType + "" == "Chaser")
        {
            CBehavior = GetComponent<ChaserBehavior>();
        }
        else if (NPCType + "" == "Runner")
        {
            RBehavior = GetComponent<RunnerBehavior>();
        }
       
    }

    

    void Update()
    {
        if (hasDestination && !moving)
        {
            traverseIndex = 0;
            if (followPath == null)
            {
                hasDestination = false;
                moving = false;
            }
            else
            {
                currentTarget = followPath[0];
                moving = true;
            }
        }
        else if (hasDestination && moving && allowMovement)
        {
            MoveToTarget();
        }
        else if (!hasDestination)
        {
            //GoToNewPosition();
            if (NPCType + "" == "Chaser")
            {
                RequestTimer += Time.deltaTime;
                if (RequestTimer > 0.025f)
                {
                    RequestTimer = 0;
                    ChaserNewDestination();
                }
            }
            else if (NPCType + "" == "Runner")
            {
                RunnerNewDestination();
            }
        }

        // anim
        if (moving && !orienting)
        {
            if (animator.GetFloat("Locomotion") < 1)
                animator.SetFloat("Locomotion", animator.GetFloat("Locomotion") + 0.04f);
        }
        else
        {
            if (animator.GetFloat("Locomotion") > 0)
                animator.SetFloat("Locomotion", animator.GetFloat("Locomotion") - 0.04f);
        }


        // if seek gets stuck.
        //seekTimer += Time.deltaTime;
        //if (seekTimer > 7.5f)
        //{
            //reset
        //}

    }

    

    void ChaserNewDestination()
    {
        currentDestination = CBehavior.RequestDestination();
        if (currentDestination.tag == "Node")
        {
            seekingTarget = false;
            closestNode = FindClosestNode(transform.position);
            followPath = AStarScript.ClusterPathFind(closestNode, currentDestination.gameObject);

            if (closestNode.transform.name != currentDestination.transform.name || followPath != null)
            {
                hasDestination = true;
            }
            else
            {
                Debug.Log("Cancelled Destination: " + currentDestination.transform.name + " for " + transform.name);
                hasDestination = false;
                currentDestination = null;
            }
        }
        else
        {
            seekingTarget = true;
        }
    }


    void RunnerNewDestination()
    {
        currentDestination = RBehavior.RequestDestination();
        // always will be a node.
        closestNode = FindClosestNode(transform.position);
        followPath = AStarScript.ClusterPathFind(closestNode, currentDestination.gameObject);

        if (closestNode.transform.name != currentDestination.transform.name || followPath != null)
        {
            hasDestination = true;
        }
        else
        {
            Debug.Log("Cancelled Destination: " + currentDestination.transform.name + " for " + transform.name);
            hasDestination = false;
            currentDestination = null;
        }

    }

    public void SeekTarget(Transform dest)
    {
        hasDestination = true;
        moving = true;
        seekingTarget = true;
        currentDestination = dest;
        currentTarget = dest;
    }


    public void StopMovement()
    {
        hasDestination = false;
        moving = false;
        currentDestination = null;
        if(followPath!=null)
            followPath.Clear();
    }

    public void ChangeClusterTarget()
    {
        ChaserNewDestination();
    }

    void MoveToTarget()
    {
        if (!seekingTarget)
        {
            currentCluster = currentTarget.GetComponent<Node>().cluster;
            Vector3 dir = transform.forward;
            if (Vector3.Distance(transform.position, currentTarget.position) > 0.2f)
            {
                dir = (currentTarget.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dir) < 10)
                {
                    transform.parent.Translate(dir * mSpeed * Time.deltaTime);
                    orienting = false;
                }
                else
                    orienting = true;

            }
            else
            {
                //Debug.Log("Reached path index: " + traverseIndex);
                if (traverseIndex < followPath.Count - 1)
                {
                    traverseIndex++;
                    currentTarget = followPath[traverseIndex];
                }
            }


            if (traverseIndex == followPath.Count || Vector3.Distance(transform.position, currentDestination.position) < 0.2f)
            {
                Debug.Log("Destination Reached.");
                hasDestination = false;
                moving = false;
                currentDestination.GetComponent<Node>().ResetMaterial();
            }

            // align orietation
            Align(dir);
        }
        else
        {
            //seeking target
            //Debug.Log("Seeking target");
            Vector3 dir = transform.forward;
            if (Vector3.Distance(transform.position, currentTarget.position) > 0.2f)
            {
                dir = (currentTarget.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dir) < 10)
                {
                    transform.parent.Translate(dir * mSpeed * Time.deltaTime);
                    orienting = false;
                }
                else
                    orienting = true;
            }
            Align(dir);
        }
    }




    //------------------------ Steering behaviors ------
    void Align(Vector3 dir)
    {
        Quaternion lookDirection;

        //set quaternion to this dir
        lookDirection = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, 4);

    }
    

    public GameObject FindClosestNode(Vector3 pos)
    {
        //overlap sphere
        Collider[] arr = Physics.OverlapSphere(pos, 20f);
        GameObject ClosestNode = null;
        float closestDistance = float.MaxValue;
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
                    Physics.Raycast(rayOutPos, dir, out hitobj);     // Make sure its visible
                    //Debug.Log("Ray out towards "+col.name+" hit: "+hitobj.collider.name);

                    if (hitobj.collider.tag == "Node" && hitobj.collider.name == col.name)
                    {
                        ClosestNode = col.gameObject;
                        closestDistance = Vector3.Distance(transform.position, col.transform.position);
                    }

                }
            }
        }
        return ClosestNode;
    }
}
