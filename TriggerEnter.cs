using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reytech How to handle collisions
/// 
/// Enable Is Trigger in Box Collider
/// On trigger Collider class is passed and returned directly rather thatn goin gthru the Collision call
/// 
/// make vars in other classes public
/// 
/// </summary>
public class TriggerEnter : MonoBehaviour
{

    private RigidBodyMovement playerMove;   //reference  RigidBodyMovement script

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);

        //  Access player stuff used in OnTriggerStay
        playerMove = other.gameObject.GetComponent<RigidBodyMovement>();


    }

    private void OnTriggerStay(Collider other)
    {
        // call other puvlic functions already available ie. speed
        // increase health
        // slow down , quicksand
        // etc. effects

        playerMove.Speed = 5f;

        //  Fancy Speed
        //playerMove.Speed -= Time.deltaTime;
        //playerMove.Speed = Mathf.Clamp(playerMove.Speed, 1f, 5f); // restrict speed to min, max


    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.gameObject.name + " has left the trigger / collider Zone.");

        playerMove = other.gameObject.GetComponent<RigidBodyMovement>(); // ? why here and not in Stay

        // reset the speed
        playerMove.Speed = 50f;
        playerMove = null;
    }
}
