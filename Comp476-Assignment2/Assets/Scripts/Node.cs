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
    float totalcost;

    bool isFinalNode;
    bool isInitialNode;

    List<GameObject> neighbours;



    void Start()
    {
        position = transform.position;
    }

    
}
