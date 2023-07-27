using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

    //   ADD TO AGENT


public class PickUpPlastic : MonoBehaviour
{
 
    [Header("PickUp Settings")]
    [SerializeField] Transform holdArea;
    private GameObject heldObj;
    private Rigidbody heldObjRB;
 
    public bool isHolding = false; // accessed from AgentLoader Scored Goal()
    public float countDown = 5f;

    private float holdingTimer = 0f;
  
    AreaSettings areaSettings;
    //Agent agent; // the actual agent class
    AgentLoader agentLoader;

    void Awake()
    {
        areaSettings = FindObjectOfType<AreaSettings>();
        agentLoader = GetComponent<AgentLoader>();
    }

    void FixedUpdate()
    {
        // Check if we are holding an object
        if (heldObj != null && heldObjRB != null)
        {
            // Set the position of the held object to the position of the holdArea
            heldObjRB.MovePosition(holdArea.position);
        }

        if (isHolding)
        {
            holdingTimer += Time.fixedDeltaTime; // Update the holding timer

/*             Debug.Log("Plastic HOLDING + reward");
            agentLoader.AddReward(2f / agentLoader.MaxStep); // Use AddReward() from the Agent component
            agentLoader.UpdateStats(); */


            if (holdingTimer >= countDown)
            {
                DropObject(); // Drop the object if the timer exceeds the threshold
            }
        }
    }

    void Update()
    {
        // Cast a ray forward to detect if the agent is looking at a goal
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider.CompareTag("goal"))
            {
                agentLoader.SetIsLookingAtGoal(true);
            }
            else
            {
                agentLoader.SetIsLookingAtGoal(false);
            }
        }
        else
        {
            agentLoader.SetIsLookingAtGoal(false);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        // By adding the check if (isHolding) at the beginning of the Trigger method, 
        // you ensure that the agent can only pick up a new object if it is not already holding one.
        if (isHolding)
        {
            // Agent is already holding an object, handle accordingly
            Debug.Log("Agent is already holding an object.");
            return;
        }

        if (other.CompareTag("plastic"))
        {
            Debug.Log("Plastic Pick up + reward");

            agentLoader.AddReward(0.1f); // Use AddReward() from the Agent component
            agentLoader.UpdateStats();
            PickUpCubes(other.gameObject);
        }
    }

    void PickUpCubes(GameObject pickObj)
    {
        if (pickObj.GetComponent<Rigidbody>()) // if we have obj, use it
        {
            heldObjRB = pickObj.GetComponent<Rigidbody>();
            heldObjRB.useGravity = false;
            heldObjRB.drag = 5;
                        //heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;
            heldObjRB.transform.parent = holdArea;  // set placement
            heldObj = pickObj;      // this being passed in
        }
        
        // Start holding timer
        holdingTimer = 0f;
        isHolding = true;
        agentLoader.SetIsHoldingObject(true); // Notify AgentLoader that the agent is holding an object
    }

    public void DropObject()
    {
        if (heldObj != null)
        {
            heldObjRB.useGravity = true;
            heldObjRB.drag = 1;
            heldObjRB.transform.parent = null;

            heldObj = null;
        }
        isHolding = false;
        holdingTimer = 0f;
        agentLoader.SetIsHoldingObject(false); // Notify AgentLoader that the agent is no longer holding an object
        StartCoroutine(areaSettings.SwapGroundMaterial(areaSettings.failureMaterial, 0.5f));
    }
}
