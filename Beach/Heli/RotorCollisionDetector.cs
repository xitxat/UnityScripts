using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 // put on rotor
 
public class RotorCollisionDetector : MonoBehaviour
{
    // Reference to the ML Agents script or the component responsible for ending the episode.
    AgentHelo agentHelo;

    // Duration for which rotor is in contact with wall
    float wallCollisionDuration = 0f;

    // Threshold duration of collision with wall that will end the episode
    float wallCollisionDurationThreshold = 1f; // 1 second

    // Boolean flag to check if currently in collision with wall
    bool isCollidingWithWall = false;

    public void Awake()
    {
        // Get the AgentHeli component from the parent object.
        agentHelo = GetComponentInParent<AgentHelo>();

        if (agentHelo == null)
        {
            Debug.LogError("AgentHeli component not found on the parent object!");
        }
    }

    private void Update()
    {
        if (isCollidingWithWall)
        {
            wallCollisionDuration += Time.deltaTime;
            if (wallCollisionDuration >= wallCollisionDurationThreshold)
            {
                Debug.Log("COLLISION WITH WALL LASTED TOO LONG: Ending episode");
                agentHelo.AddReward(-1f);
                agentHelo.EndEpisode();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ground"))
        {
            Debug.Log("<color=orange>HIT: ...GROUND...</color>");
            agentHelo.AddReward(-1f);
            agentHelo.EndEpisode();
        }
        else if (other.CompareTag("wall"))
        {
            Debug.Log("COLLIDED WITH WALL: Starting to count time");
            wallCollisionDuration = 0f;
            isCollidingWithWall = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("wall"))
        {
            Debug.Log("ENDED COLLISION WITH WALL: Resetting timer");
            wallCollisionDuration = 0f;
            isCollidingWithWall = false;
        }
    }
}
