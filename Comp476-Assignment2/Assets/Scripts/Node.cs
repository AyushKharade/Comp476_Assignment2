﻿using System.Collections;
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
    [HideInInspector]public Vector3 position;
    public float gCost;
    public float hCost;
    

    public bool isFinalNode;
    public bool isInitialNode;

    public List<GameObject> neighbours;

    public GameObject Parent;


    void Start()
    {
        position = transform.position;
        // check if has atleast one neighbour
        if (neighbours.Count == 0)
            Debug.Log(transform.name+" has 0 NEIGHBOURS");
    }


    public float GetFCost()
    {
        return gCost + hCost;
    }

    public void CalculateFCost(Vector3 start, Vector3 end)
    {
        gCost = Vector3.Distance(start, transform.position);
        hCost = Vector3.Distance(end, transform.position);
    }

    public void SetParentPath(GameObject g)
    {
        Parent = g;
    }
    
}
