using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    GameObject AStarRef;

    private void Start()
    {
        AStarRef = GameObject.FindGameObjectWithTag("A*");
    }

    public void StartPathfinding()
    {
        // call start function on A*.
        Debug.Log("Started Pathfinding.");
        AStarRef.GetComponent<Pathfinding>().StartPathfinding();
    }

    public void SetStartingNode() { }

    public void SetEndingNode() { }

}
