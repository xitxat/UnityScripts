using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using static LoaderController;
using TMPro;
using System;

public class AgentLoader : Agent
{
    public GameObject trophy;
    public GameObject[] goals;     // assign in inspector
    public GameObject[] plastics;
    [HideInInspector] public GoalDetectTrash goalDetectTrash; //  script on the trash object 

    WaitForSeconds printInterval = new WaitForSeconds(5f);

    GoalDetectTrash[] goalDetectTrashArray;
    GameObject axelObject; // WaterLevel

    LoaderController loaderController;
    AreaPlasticSpawnTimer areaPlasticSpawnTimer;
    PickUpPlastic pickUpPlastic;
    Rigidbody agentRigidbody;  //cache in initialization
    AreaSettings areaSettings;
    Vector3 originalPosition = Vector3.zero;
    Vector3 lastPosition = Vector3.zero;    //  move twd goal
    Vector3[] goalPositions; // stores positions of goals

    [Header("Loader Score Board")]
    [SerializeField] private TextMeshProUGUI rewardValue = null;
    [SerializeField] private TextMeshProUGUI trashItemsValue = null;
    [SerializeField] private TextMeshProUGUI floatingPlasticValue = null;
    [SerializeField] private TextMeshProUGUI recycledPlasticValue = null;
    float overallReward = 0;
    float trashItems = 0;
    float totalMass = 0f;
    float totalDestroyedMass = 0f;

    bool isHoldingObject = false; 
    bool isLookingAtGoal = false;
    bool isMovingToGoal = false;


    #region STARTS
    public override void Initialize()
    {
        loaderController = GetComponent<LoaderController>();
        pickUpPlastic = GetComponent<PickUpPlastic>();
        areaSettings = FindObjectOfType<AreaSettings>();
        areaPlasticSpawnTimer = FindObjectOfType<AreaPlasticSpawnTimer>();

        agentRigidbody = loaderController.GetComponent<Rigidbody>();
        //agentRigidbody = GetComponent<Rigidbody>();
        originalPosition = transform.localPosition;

        StartCoroutine(PrintCumulativeReward());

        goalDetectTrashArray = new GoalDetectTrash[plastics.Length];
        for (int i = 0; i < plastics.Length; i++)
        {
            goalDetectTrashArray[i] = plastics[i].GetComponent<GoalDetectTrash>();
            goalDetectTrashArray[i].InitializeAgent(this); // Pass the AgentLoader reference to GoalDetectTrash            }
        }

        axelObject = this.transform.Find("AxelWaterLevel").gameObject;
        if (axelObject == null)  Debug.Log("Axel object not found");
        
        // Initialize goalPositions array
        goalPositions = new Vector3[goals.Length];
        for (int i = 0; i < goals.Length; i++)
        {
            goalPositions[i] = goals[i].transform.position;
        }

        lastPosition = transform.position; // Initialize the last position

    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition =  originalPosition;
        transform.localRotation = Quaternion.Euler(0, -90, 0);
        agentRigidbody.velocity = Vector3.zero;

        pickUpPlastic.DropObject();
    }
    #endregion

    #region OBSERVATIONS
public override void CollectObservations(VectorSensor sensor)
{
    // 3 observations - x, y, z for agent's position
    sensor.AddObservation(transform.localPosition);

    // Add observation for whether the agent is holding an object
    sensor.AddObservation(isHoldingObject);

    // Add observation for whether the agent is looking at a goal
    sensor.AddObservation(isLookingAtGoal);

    // For each goal in goals array x 3 in Observations
    foreach (var goal in goals)
    {
        // Add 3 observations - x, y, z for each goal's position
        sensor.AddObservation(goal.transform.localPosition);
    }

    // Define the names of the material files
    string[] materialNames = { "P 2 Blue", "P Grey", "P Orange", "P Purple" };

    // For each goal in goals array, add material observation
    foreach (var goal in goals)
    {
        string goalMaterialName = goal.GetComponent<Renderer>().sharedMaterial.name;
        int goalMaterialIndex = Array.IndexOf(materialNames, goalMaterialName);
        // Add material observation as a one-hot encoded vector
        for (int i = 0; i < materialNames.Length; i++)
        {
            float value = (i == goalMaterialIndex) ? 1f : 0f;
            sensor.AddObservation(value);
        }
    }

    // For each plastic object in plastics array, add material observation
    foreach (var plastic in plastics)
    {
        string plasticMaterialName = plastic.GetComponent<Renderer>().sharedMaterial.name;
        int plasticMaterialIndex = Array.IndexOf(materialNames, plasticMaterialName);
        // Add material observation as a one-hot encoded vector
        for (int i = 0; i < materialNames.Length; i++)
        {
            float value = (i == plasticMaterialIndex) ? 1f : 0f;
            sensor.AddObservation(value);
        }
    }
} 
    #endregion


    #region REWARDS
    void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "wall":
            Debug.Log("Hit Wall");
            AddReward(-10f / MaxStep);
            UpdateStats();
            break;

            case "obstacle":
            Debug.Log("Hit obstacle");
            AddReward(-10f / MaxStep);
            UpdateStats();
            break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("deathZone"))
        {
            Debug.Log("DeathZone");
            AddReward(-50f / MaxStep);
            UpdateStats();
            //StartCoroutine(EndEpisodeAfterDelay(2f));
            EndEpisode();
        }
    }


    public void ScoredAGoal()
    {
        Debug.Log("GOAL, recycle kg ++");
        AddReward(0.5f);

        UpdateStats();

        pickUpPlastic.isHolding = false;
        
        //EndEpisode();
        // Swap ground material for a bit to indicate we scored.
        StartCoroutine(areaSettings.SwapGroundMaterial(areaSettings.successMaterial, 1f));
    }

    public void ScoredAGoalWithBonus()
    {
        Debug.Log("BONUS GOAL , recycle kg ++ ");
        AddReward(1f);
        UpdateStats();
        pickUpPlastic.isHolding = false;
        StartCoroutine(areaSettings.SwapGroundMaterial(areaSettings.bonusMaterial, 1f));    
    }

    public void UpdateStats()
    {
        overallReward = this.GetCumulativeReward();
        trashItems = areaPlasticSpawnTimer.GetSpawnedCount();
        totalMass = areaPlasticSpawnTimer.GetTotalSpawnedMass();
        totalDestroyedMass = GetTotalDestroyedMass();

        rewardValue.text = $"{overallReward.ToString("F2")}";
        trashItemsValue.text = $"{trashItems}";
        floatingPlasticValue.text = $"{totalMass}";
        recycledPlasticValue.text = $"{totalDestroyedMass}";


    }   
    #endregion

    #region MOVEMENT + rewards. FIXEDUPDATE
    // the method responsible for managing actions and rewards.
    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        // Calculate the distance to each goal in the previous step
        float[] lastDistancesToGoals = new float[goals.Length];
        for (int i = 0; i < goals.Length; i++)
        {
            lastDistancesToGoals[i] = Vector3.Distance(lastPosition, goalPositions[i]);
        }

        // MOVE the agent using the action.
        MoveAgent(actionBuffers.DiscreteActions);



        if (IsAxelUnderwater())
        {
            Debug.Log("Axel Underwater");
            AddReward(-10f / MaxStep);
            UpdateStats();
        }

        // Calculate the distance to each goal in the current step and compare
        // with the distance in the previous step
        isMovingToGoal = false; // Assume that the agent is not moving towards a goal
        for (int i = 0; i < goals.Length; i++)
        {
            float currentDistanceToGoal = Vector3.Distance(transform.position, goalPositions[i]);
            if (currentDistanceToGoal < lastDistancesToGoals[i])
            {
                // The agent is closer to the goal than in the previous step
                isMovingToGoal = true;
                break;
            }
        }

        // Update the last position
        lastPosition = transform.position;


        if (isHoldingObject && isLookingAtGoal && isMovingToGoal)
        {
            AddReward(100f / MaxStep);
            UpdateStats();
        }

        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-1f / MaxStep);
        UpdateStats();
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];

        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                break;
        }
        // ACTUALLY MOVING THE RIGIDBODY    
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 20f);
        agentRigidbody.AddForce(dirToGo * areaSettings.agentRunSpeed, ForceMode.VelocityChange);

    }

    private bool IsAxelUnderwater() => axelObject.transform.position.y < 0 && axelObject.transform.position.x <= 0;

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.RightArrow))    //  D
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.UpArrow))   //  W
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))   //  A
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.DownArrow))   //  S
        {
            discreteActionsOut[0] = 2;
        }


    }
    #endregion

    IEnumerator PrintCumulativeReward()
    {
        while (true)
        {
            yield return printInterval;

            // Print the cumulative reward to the console
            Debug.Log("from agent PrintCumulativeReward() Cumulative Reward: " + GetCumulativeReward());
        }
    }

    IEnumerator EndEpisodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndEpisode();
    }

    public void AddToTotalDestroyedMass(float mass)
    {
        totalDestroyedMass += mass;
    }

    public float GetTotalDestroyedMass()
    {
        return totalDestroyedMass;
    }

    public void ReduceFloatingMass(float mass)
    {
        totalMass -= mass;
        // Ensure totalMass does not go below zero
        totalMass = Mathf.Max(0, totalMass);
    }

    public void SetIsHoldingObject(bool holding)
    {
        isHoldingObject = holding;
    }

    public void SetIsLookingAtGoal(bool looking)
    {
        isLookingAtGoal = looking;
    }


}
