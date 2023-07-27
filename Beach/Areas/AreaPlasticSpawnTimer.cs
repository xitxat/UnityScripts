using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Spawns a prefab randomly throughout the volume of a Unity transform. Attach to a Unity cube to visually scale or rotate. For best results disable collider and renderer.
/// </summary>
public class AreaPlasticSpawnTimer : MonoBehaviour 
{

    //public GameObject ObjectToSpawn; // single
    public GameObject[] ObjectsToSpawn; // Now an array
    public float RateOfSpawn = 1;
    private float nextSpawn = 0;

    // Create a new List to hold the spawned objects
    private List<GameObject> spawnedPrefabs = new List<GameObject>();

    void Update () 
    {
        if(Time.time > nextSpawn)
        {
            nextSpawn = Time.time + RateOfSpawn;

                        // Random position within this transform
            Vector3 rndPosWithin;
            rndPosWithin = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            rndPosWithin = transform.TransformPoint(rndPosWithin * .5f);

            GameObject objectToSpawn = ObjectsToSpawn[Random.Range(0, ObjectsToSpawn.Length)];
            Quaternion randomRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            // Instantiate the object and add it to the list
            GameObject spawnedObject = Instantiate(objectToSpawn, rndPosWithin, randomRotation);
            spawnedPrefabs.Add(spawnedObject);     
    
        }
    }

    public void DestroySpawnedObjects()
    {
        foreach(GameObject obj in spawnedPrefabs)
        {
            if(obj != null) Destroy(obj);
        }
        // Clear the list
        spawnedPrefabs.Clear();
    }

    public int GetSpawnedCount()
    {
        return spawnedPrefabs.Count;
    }

    public float GetTotalSpawnedMass()
    {
        float totalMass = 0f;

        foreach(GameObject prefab in spawnedPrefabs)
        {
            if(prefab != null) 
            {
                totalMass += prefab.GetComponent<Rigidbody>().mass;
            }
        }

        return totalMass;
    }

}