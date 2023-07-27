using UnityEngine;

    //  THIS GOES ON THE TRASH OBJECT   !!!

public class GoalDetectTrash : MonoBehaviour
{

    [HideInInspector] public AgentLoader agent;  

    public Material objectMaterial; // Assign this in the inspector

    public void InitializeAgent(AgentLoader agentLoader)
    {
        agent = agentLoader;
    }

    void OnCollisionEnter(Collision col)
    {
        // Touched goal.
        if (col.gameObject.CompareTag("goal"))
        {
            float mass = gameObject.GetComponent<Rigidbody>().mass;
            agent.AddToTotalDestroyedMass(mass);

            Goal goal = col.gameObject.GetComponent<Goal>();

            if (goal != null && goal.goalMaterial.name == this.objectMaterial.name)
            {
                agent.ScoredAGoalWithBonus();
            }
            else
            {
                agent.ScoredAGoal();
            }

            Destroy(gameObject);
        }
    }
}
