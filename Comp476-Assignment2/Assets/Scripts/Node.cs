using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // node stores
    // position
    // g cost
    // h cost
    // total cost
    // bool isEndNode.


    // variables
    Vector3 position;
    float gCost;
    float hCost;
    

    public bool isFinalNode;
    public bool isInitialNode;

    public List<GameObject> neighbours;



    void Start()
    {
        position = transform.position;
        // check if has atleast one neighbour
        if (neighbours.Count == 0)
            Debug.Log(transform.name+" has 0 NEIGHBOURS");
    }


    public float GetCost()
    {
        return gCost + hCost;
    }
    
}
