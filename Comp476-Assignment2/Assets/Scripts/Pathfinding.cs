using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public GameObject StartNode;
    public GameObject EndNode;


    List<Node> OpenSet;
    List<Node> ClosedSet;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
}
