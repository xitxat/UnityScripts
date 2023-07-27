using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//   https://www.youtube.com/watch?v=iasDPyC0QOg   advanced float t: 8:30
//   Switch state overrides Rigidbody drag
//      

[RequireComponent(typeof(Rigidbody))]

public class BoyancyObject : MonoBehaviour
{
    private AgentLoader agentLoader;

    [Header("FLOAT & DRAG")]
    public float underWaterDrag = 20f;
    public float underWaterAngularDrag = 2f;
    public float airDrag = 2f;
    public float airAngularDrag = 0.2f;
    public float floatingPower  =275f;
    public float waterHeight = 0f;
    public float shoreLine = 0f;
    public float maxFlightHeight = 2.5f;
    

    [Header("SPIN & BOB")]
    public float degreesPerSecond = 0.2f;


    Rigidbody rbody;
    bool underWater;
    


    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        agentLoader = FindObjectOfType<AgentLoader>();

    }


    void FixedUpdate()
    {
        // calculate rigidbody and water height. Depth Force.
        float difference = transform.position.y - waterHeight;

        if(difference < 0 && transform.position.x < shoreLine)
        {
            rbody.AddForceAtPosition (Vector3.up * floatingPower * Mathf.Abs(difference), 
                                            transform.position, ForceMode.Force);
            Spin();
            
            if(!underWater) 
            {
                underWater = true;
                SwitchState(true);
            }
        }
        else if (underWater) 
        {
            underWater = false;
            SwitchState(false);
        }

            // When object reaches the shore
        if (transform.position.x >= shoreLine)
        {
            agentLoader.ReduceFloatingMass(rbody.mass);
        }

            // Check if the object has reached the maximum flight height
        if (transform.position.y >= maxFlightHeight)
        {
            Destroy(gameObject);
        }
    }

    //  Drag
    void SwitchState(bool isUnderWater)
    {
        if(isUnderWater)
        {
        rbody.drag = underWaterDrag;
        rbody.angularDrag = underWaterAngularDrag;
        }

        else
        {
            rbody.drag = airDrag;
            rbody.angularDrag = airAngularDrag;
        }
    }

    // Bob & spin
    void Spin()
    {
        float rotationAngle = degreesPerSecond * Time.deltaTime; // Calculate the rotation angle per frame
        Vector3 rotationAxis = Vector3.up; // You can change this axis according to your needs

        Quaternion rotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);
        Quaternion targetRotation = rbody.rotation * rotation;

        // Interpolate the rotation gradually over time
        rbody.MoveRotation(Quaternion.RotateTowards(rbody.rotation, targetRotation, Time.deltaTime * Mathf.Infinity));
    }


}








