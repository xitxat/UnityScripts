using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

    //  This Box Collider Is Trigger
    //  CreateSpawn() originally run from Agent

public class AreaPlasticSpawn : MonoBehaviour
{
    public GameObject obstacleToSpawn;
    public int numObstacles;
    public float minDistance;

    private Vector3 cubeSize;
    private Vector3 spawnPosition;
    public GameObject[] spawnedPrefabs;

    public void Awake()
    {
        // Get the size of the cube collider
        BoxCollider cubeCollider = GetComponent<BoxCollider>();
        cubeSize = cubeCollider.size;
        spawnedPrefabs = new GameObject[numObstacles];
    }

    void Start()
    {
        CreateObstacle();
    }

    public void CreateObstacle()
    {
        for (int i = 0; i < numObstacles; i++)
        {
            // Randomly position the spawned object inside the cube with Y position 0
            //  Random.Range(-cubeSize.y, cubeSize.y)
            spawnPosition = transform.position + new Vector3(
                                        Random.Range(-cubeSize.x, cubeSize.x),
                                        Random.Range(-cubeSize.y, cubeSize.y),
                                        Random.Range(-100f, 600f));
            

            // Check if the spawned object is too close to existing objects
            Collider[] colliders = Physics.OverlapSphere(spawnPosition, minDistance);
            bool isValidSpawn = true;
            foreach (Collider collider in colliders)
            {
                if (collider.attachedRigidbody != null)
                {
                    isValidSpawn = false;
                    break;
                }
            }

            if (isValidSpawn)
                {
                    //Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);
                    spawnedPrefabs[i] = Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);
                }
        } 
    }

    public void RemoveSpawnedObstacles()
        {
            Debug.Log("RemoveSpawnedObjects()");
            // Destroy all the spawned prefabs
            for (int i = 0; i < numObstacles; i++)
            {
                if (spawnedPrefabs[i] != null)
                {
                    Destroy(spawnedPrefabs[i]);
                }
            }
        }
    
    }
