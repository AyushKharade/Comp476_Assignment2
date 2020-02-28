using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagCamera : MonoBehaviour
{

    public Transform Target;
    public List<Transform> ListOfTargets;
    int targetIndex;

    public float smoothValue = 0.3f;

    [Header("Camera Offsets")]
    public float height;
    public float xOffset;
    public float zOffset;

    private float xOldOffset;
    private float yOldOffset;
    private float zOldOffset;

    Vector3 velocity = Vector3.zero;

    void Start()
    {
        Target = ListOfTargets[0];
        targetIndex = 0;

        Debug.Log(" Mod: "+(-1%2));
    }

    void Update()
    {
        CamControl();
        ZoomControl();

        if (Input.GetKeyDown(KeyCode.Q))       // switch to next target
        {
            Target = ListOfTargets[GetMod((targetIndex+1),ListOfTargets.Count)];
            targetIndex=GetMod((targetIndex+1), ListOfTargets.Count);
        }

        if (Input.GetKeyDown(KeyCode.E))        // switch to previous target
        {

            Target = ListOfTargets[GetMod((targetIndex - 1), ListOfTargets.Count)];
            targetIndex = GetMod((targetIndex -1), ListOfTargets.Count);
        }
        
    }

    int GetMod(int x, int r)
    {
        if (x >= 0)
        {
            return x % r;
        }
        else
        {
            return (x + r) % r;
        }
    }

    void CamControl()
    {
        Vector3 pos = new Vector3();
        pos.x = Target.position.x + xOffset;
        pos.y = Target.position.y + height;
        pos.z = Target.position.z + zOffset;

        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothValue);

        // move on z axis
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (zOffset < -2)
                zOffset += 0.2f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (zOffset > -10)
                zOffset -= 0.2f;
        }

    }

    void ZoomControl()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            if (height > 2)
                height -= 0.2f;
            if (xOffset > 2)
                xOffset -= 0.2f;
            if (zOffset < -2)
                zOffset += 0.2f;
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            if (height < 15)
                height += 0.2f;
            if (xOffset < 15)
                xOffset += 0.2f;
            if (zOffset > -15)
                zOffset -= 0.2f;
        }

        if (Input.GetKey(KeyCode.R))
        {
            //Reset Camera
            Debug.Log("Resetting Camera Offsets to Default");
            height = yOldOffset;
            xOffset = xOldOffset;
            zOffset = zOldOffset;
        }
    }
}
