using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basics of instantiating a Prefab
/// https://docs.unity3d.com/Manual/InstantiatingPrefabs.html
/// 
/// 
/// </summary>
public class SpawnerStatic : MonoBehaviour
{

    public Queue<GameObject> QueuedCubes;


    public GameObject Origin;
    public GameObject CubePrefab;
    public float SpawnRate;
    public int MaxCubes;


    void Start()
    {
        
        QueuedCubes= new Queue<GameObject>();   // init queue

        for(int i = 0; i < MaxCubes; i++)       // instan set amout of cubes
        {
            GameObject InstantiatedCube = Instantiate(CubePrefab, Origin.transform.position, Quaternion.identity);
            QueuedCubes.Enqueue(InstantiatedCube);

        }

        InvokeRepeating("SpawnCubes", 0, 1 / SpawnRate);

    }

    private void SpawnCubes()                   // cubes to use
    {
        GameObject CurrentCube = QueuedCubes.Dequeue();

        CurrentCube.transform.position = Origin.transform.position;

        Vector3 RandomForce = new Vector3(Random.Range(-40, 40), 50f, Random.Range(-40, 40));
        CurrentCube.GetComponent<Rigidbody>().AddForce(RandomForce);

        //absorb & repeat
        QueuedCubes.Enqueue(CurrentCube);

    }


}// end class
