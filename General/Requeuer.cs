using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Requeuer : MonoBehaviour
{
    // if attached to floor
    // despawn on floor touch
    // and requeue

    // reference SpawnerDynamic script
    public SpawnerDynamic CubeSpawner;


    private void OnCollisionEnter(Collision collision)
    {
        //not checking for other objects
         CubeSpawner.QueuedCubes.Enqueue(collision.gameObject);
        
        //CubeSpawner.QueuedCubes.Enqueue(collision.spa);

        //playerMove = other.gameObject.GetComponent<RigidBodyMovement>();

    }
}
