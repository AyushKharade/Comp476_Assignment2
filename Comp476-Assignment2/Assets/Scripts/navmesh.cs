using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class navmesh : MonoBehaviour
{
    public GameObject character;
    public bool hasDestination;

    void Start()
    {
        
    }

    void Update()
    {
        ReceiveRaycast();
    }

    void ReceiveRaycast()
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
                    if (hasDestination)
                    {
                        character.GetComponent<NavMeshAgent>().isStopped = true;
                        character.GetComponent<NavMeshAgent>().ResetPath();
                    }
                    character.GetComponent<NavMeshAgent>().SetDestination(hit.transform.position);
                    Debug.Log(hit.transform.position);
                    hasDestination = true;

                }

            }
        }
    }
}
