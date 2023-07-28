using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

    //  mlagents-learn config/ppo/Beach.yaml --run-id=Heli_00

public class AgentHelo : Agent
{
    public GameObject goal;

    Rigidbody rb_Heli;  //cache in initialization
    HeloControl heloControl;
    Vector3 originalPosition;
    Vector3 goalPosition;

    int goalCount = 0; 
    float goalCountTimer = 10.0f; // Timer to log goal count 
    float agentThrottle; // percentage
    float throttleChange;
    float agentThrottleDebugTimer = 0.5f;
    float lastHoverPrintTime = 0f;
    float hoverMinRPM = 29.0f;
    float hoverMaxRPM = 40.0f;
    float hoverTimer = 0.0f;
    float hoverLimit = 2.0f;
    float agentThrottleRedLine = 75f;
    float takeOffThreshold = 0.03f;
    float initialYPos;
    float rewardTimer = 0.0f;       // decrementing from rewards~ OnTriggerStay
    float logRewardTimer = 0.0f;    // In goal cummulative rewards
    const float logInterval = 0.25f;
    
    bool hasTakenOff;
    bool isHovering = false;
    bool throttleUsed = false;
    bool hasEnteredGoal = false;
    bool isInGoal = false;

    #region STARTS
    public override void Initialize()
    {
        heloControl = GetComponent<HeloControl>();
        rb_Heli = heloControl.GetComponent<Rigidbody>();
        originalPosition = transform.localPosition;
        hasTakenOff = false;
        hasEnteredGoal = false;
        rewardTimer = 0;
        InvokeRepeating("AgentThrottleDebugLog", 0f, agentThrottleDebugTimer);
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition =  originalPosition;
        transform.localRotation = Quaternion.Euler(0, -90, 0);
        agentThrottle = 0f;
        heloControl.ResetThrottle();
        rb_Heli.velocity = Vector3.zero;
        isHovering = false;
        hoverTimer = 0.0f;
        throttleUsed = false;
        initialYPos = transform.position.y;
        hasTakenOff = false;
        hasEnteredGoal = false;
        rewardTimer = 0;

        int randomRotation = UnityEngine.Random.Range(0, 4);
        // Rotate the agent based on the random integer
        switch (randomRotation)
        {
            case 0:
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case 1:
                transform.localRotation = Quaternion.Euler(0, 90, 0);
                break;
            case 2:
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                break;
            case 3:
                transform.localRotation = Quaternion.Euler(0, 270, 0);
                break;
        }
            
    }
    #endregion

    #region OBSERVATIONS
    public override void CollectObservations(VectorSensor sensor)
    {
        // 20 observations 
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(rb_Heli.velocity);
        sensor.AddObservation(goalPosition);
        sensor.AddObservation(Vector3.Distance(transform.localPosition, goalPosition));
        sensor.AddObservation(agentThrottle);
        sensor.AddObservation(throttleChange);
        sensor.AddObservation(isHovering);
        sensor.AddObservation(hasEnteredGoal);
        sensor.AddObservation(isInGoal);
        sensor.AddObservation(heloControl.Throttle());

    }
    #endregion


    #region REWARDS

        void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "wall":
            Debug.Log("Hit Wall");
            AddReward(-0.05f);
            break;

            case "obstacle":
            Debug.Log("Hit obstacle");
            AddReward(-0.05f);
            break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("goal"))
        {
            Debug.Log("<color=blue>...GOAL...</color>");
            CheckFirstGoalEntry();

            rewardTimer = 1.5f; // countdown when entering goal
            isInGoal = true;
            goalCount++;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("goal"))
        {
            // Only reward for the first 1.5 seconds in the goal
            if(rewardTimer > 0)
            {
                // This reward will be added each step while in the goal for the first 1.5 seconds
                // Timer decremented in OnActionReceived
                AddReward(100.0f / MaxStep);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("goal"))
        {
            isInGoal = false;
        }
    }


    #endregion

    private void Update()
    {
        LogGoalCount();
        LogRewardInterval();
        HasLeftPad();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
        CheckHovering();
        CheckRedLineThrottle();
        AddReward(-1.0f / MaxStep);
        GoalRewardTimer();


    }

    #region MOVEMENT  FIXEDUPDATE
        public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        // Roll: 'a' (left), 'd' (right)    Z axis
        discreteActionsOut[0] = Input.GetKey(KeyCode.Q) ? 1 : Input.GetKey(KeyCode.E) ? 2 : 0;

        // Pitch: 's' (down), 'w' (up)  X axis
        discreteActionsOut[1] = Input.GetKey(KeyCode.S) ? 1 : Input.GetKey(KeyCode.W) ? 2 : 0;

        // Yaw: 'q' (left), 'e' (right) Y axis
        discreteActionsOut[2] = Input.GetKey(KeyCode.A) ? 1 : Input.GetKey(KeyCode.D) ? 2 : 0;

        // Throttle control: 'Space' (increase), 'LeftShift' (decrease)
        discreteActionsOut[3] = Input.GetKey(KeyCode.Space) ? 1 : Input.GetKey(KeyCode.LeftShift) ? 2 : 0;
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var rollAction = act[0];
        var pitchAction = act[1];
        var yawAction = act[2];
        var throttleAction = act[3];

        float roll = 0f;
        float pitch = 0f;
        float yaw = 0f;
        float throttleChange = 0f;

        // Roll
        switch (rollAction)
        {
            case 1:
                roll = -1f;  // roll left
                break;
            case 2:
                roll = 1f;  // roll right
                break;
        }

        // Pitch
        switch (pitchAction)
        {
            case 1:
                pitch = -1f;  // pitch down
                break;
            case 2:
                pitch = 1f;  // pitch up
                break;
        }

        // Yaw Rotate
        switch (yawAction)
        {
            case 1:
                yaw = -1f;  // yaw left
                break;
            case 2:
                yaw = 1f;  // yaw right
                break;
        }

        // Throttle
        switch (throttleAction)
        {
            case 1:
                throttleChange = 1f;  // increase throttle
                break;
            case 2:
                throttleChange = -1f;  // decrease throttle
                break;
        }

        CheckThrottleFirstUse();


        agentThrottle += Time.deltaTime * heloControl.throttleAmt * throttleChange;
        agentThrottle = Mathf.Clamp(agentThrottle, 0f, 100f);

        rb_Heli.AddForce(transform.up * agentThrottle, ForceMode.Impulse);
        rb_Heli.AddTorque(transform.right * pitch * heloControl.responsiveness);
        rb_Heli.AddTorque(-transform.forward * roll * heloControl.responsiveness);
        rb_Heli.AddTorque(transform.up * yaw * heloControl.responsiveness);

        heloControl.GetRotorsTransform().Rotate(Vector3.up * agentThrottle * heloControl.GetRotorSpeedModifier());

    }
    #endregion

    #region METH

    private void AgentThrottleDebugLog()
    {
        Debug.Log("AGENT THROTTLE: " + agentThrottle);
    }

    private void CheckHovering()
    {
        if (agentThrottle >= hoverMinRPM && agentThrottle <= hoverMaxRPM)
        {
            isHovering = true;
            if (Time.time - lastHoverPrintTime >= 0.5f) // Print timer
            {
                Debug.Log("<color=orange>...HOVERING...</color>");
                lastHoverPrintTime = Time.time; // Update the last print time
            }
        }
        else
        {
            isHovering = false;
        }
        // Increment hover timer if hovering and in goal
        if (isHovering && isInGoal)
        {
            hoverTimer += Time.fixedDeltaTime;
            if (hoverTimer >= hoverLimit)
            {
                AddReward(1.0f);
                hoverTimer = 0.0f; 
                EndEpisode(); 
            }
        }
    }

    private void CheckThrottleFirstUse()
    {
        if (!throttleUsed && agentThrottle > 1)
        {
            AddReward(0.25f);
            throttleUsed = true;
        }
    }

    private void CheckRedLineThrottle()
    {
        if(agentThrottle > agentThrottleRedLine)
        {
            AddReward(-1.0f / MaxStep);
        }
    }

    private void HasLeftPad()
    {
                if (transform.position.y > initialYPos + takeOffThreshold)
        {
            // Log agentThrottle first time  Y position increases by takeOffThreshold
            if (!hasTakenOff)
            {
                Debug.Log("<color=green>AGENT THROTTLE AT TAKEOFF: </color>" + agentThrottle);
                AddReward(0.25f);
                hasTakenOff = true; 
            }
        }
    }

    private void CheckFirstGoalEntry()
    {
        if (!hasEnteredGoal)
        {
            AddReward(0.25f);
            hasEnteredGoal = true;
        }
    }

    void LogGoalCount()
    {
        goalCountTimer -= Time.deltaTime;
        if (goalCountTimer <= 0.0f)
        {
            Debug.Log("Goal Count: " + goalCount);
            goalCountTimer = 10.0f; // Reset the timer
        }
    }

    void GoalRewardTimer()
    {
            // Decrement reward timer at each step
        if (rewardTimer > 0)
        {
            rewardTimer -= Time.fixedDeltaTime;
        }
    }

    void LogRewardInterval()
    {
        // Only log if in goal and during reward period
        if (isInGoal && rewardTimer > 0) {
            // Decrement log timer
            logRewardTimer -= Time.deltaTime;

            // If log interval has elapsed, log the reward and reset timer
            if (logRewardTimer <= 0.0f) {
                Debug.Log("Cumulative reward: " + GetCumulativeReward());
                logRewardTimer = logInterval;
            }
        }
    }
    #endregion
}
