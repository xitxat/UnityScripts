using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=AXOUgbPiugE

public class LoaderWheel : MonoBehaviour
{

    public WheelCollider targetWheel;

    Vector3 wheelPosition    = new Vector3();
    Quaternion wheelRotation = new Quaternion();



    // Update is called once per frame
    void Update()
    {
        targetWheel.GetWorldPose(out wheelPosition, out wheelRotation);
        transform.position = wheelPosition;
        transform.rotation = wheelRotation;
    }
}
