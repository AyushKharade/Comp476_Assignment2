using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    GameObject AStarRef;
    Pathfinding ScriptRef;

    public Button StartButtonRef;
    public Button ResetButtonRef;

    private void Start()
    {
        AStarRef = GameObject.FindGameObjectWithTag("A*");
        ScriptRef = AStarRef.GetComponent<Pathfinding>();

        ResetButtonRef.interactable = false;
    }

    public void StartPathfinding()
    {
        // call start function on A*.
        //Debug.Log("Started Pathfinding.");
        if (ScriptRef.StartNode != null && ScriptRef.EndNode != null)
        {
            ScriptRef.StartPathfinding();
            StartButtonRef.interactable = false;
            ResetButtonRef.interactable = true;
        }
        else
            Debug.Log("Start and/or End nodes not selected.");

        
    }

    public void SetStartingNode()
    {
        if (ScriptRef.CurrentSelectedNode != null)
        {
            ScriptRef.SetStartNode();
        }
    }

    public void SetEndingNode()
    {
        if (ScriptRef.CurrentSelectedNode != null)
        {
            ScriptRef.SetEndNode();
        }
    }

    public void ResetCall()
    {
        StartButtonRef.enabled = true;
        ScriptRef.ResetPathfinding();

        ResetButtonRef.interactable = false;
        StartButtonRef.interactable = true;
    }

}
