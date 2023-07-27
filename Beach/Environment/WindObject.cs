using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    //  Uses Opportunistic Assignment of  windArea in Collision & not in Start()
    //   Add TAG "windArea"
public class WindObject : MonoBehaviour
{

    public bool isInWindZone = false;
    public float torque; 

    GameObject windArea;
    WindForce windForce;
    
    Rigidbody rb;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        windForce = GetComponent<WindForce>();
    }


    void FixedUpdate()
    {

/*         if (isInWindZone && windArea != null)
        {
            rb.AddForce(windForce.direction * windForce.strength);
        } */

        if(isInWindZone)
        {
           rb.AddForce(windArea.GetComponent<WindForce>().direction * windArea.GetComponent<WindForce>().strength);
           //rb.AddTorque(transform.forward * torque );
        
        }
    }

    void OnTriggerEnter(Collider col)
    {
            //windArea = col.gameObject;


            if (col.gameObject.tag == "windArea")
            {
                windArea = col.gameObject;
                isInWindZone = true;

            }
    }

    void OnTriggerExit(Collider col)
    {
            if (col.gameObject.tag == "windArea")
            {
                isInWindZone = false;
            }    
    }


    
    }
