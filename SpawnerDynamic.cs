using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basics of instantiating a Prefab
/// https://docs.unity3d.com/Manual/InstantiatingPrefabs.html
/// 
/// DYNAMIC: Better first frame
///         instanciate only what is needed
/// 
/// nNeeds RE QUEUE script attached to on destroy/ reinit object eg on floor touch
///     :: CubeRequeuer script
/// 
/// </summary>
public class SpawnerDynamic : MonoBehaviour
{

    public Queue<GameObject> QueuedCubes;


    public GameObject Origin;
    public GameObject CubePrefab;
    public float SpawnRate;



    void Start()
    {
        
        QueuedCubes= new Queue<GameObject>();   // init queue

        InvokeRepeating("SpawnCubes", 0, 1 / SpawnRate);

    }

    private void SpawnCubes()                   // cubes to use
    {
        if(QueuedCubes.Count > 0)
        {
            GameObject CurrentCube = QueuedCubes.Dequeue(); //DEQUEUE

            CurrentCube.transform.position = Origin.transform.position; // and reuse

            Vector3 RandomForce = new Vector3(Random.Range(-40, 40), 50f, Random.Range(-40, 40));
            CurrentCube.GetComponent<Rigidbody>().AddForce(RandomForce);
        }
        // if queue is empty
        else
        {
            // iNSTANCIATE
            GameObject CurrentCube = Instantiate(CubePrefab, Origin.transform.position, Quaternion.identity);
            //INITIALISE
            CurrentCube.transform.position = Origin.transform.position;

            Vector3 RandomForce = new Vector3(Random.Range(-40, 40), 50f, Random.Range(-40, 40));
            CurrentCube.GetComponent<Rigidbody>().AddForce(RandomForce);

        }


    }


}// end class
