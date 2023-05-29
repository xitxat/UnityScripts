using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFloater : MonoBehaviour
{
public Rigidbody rigidBody;
    public float basicDepthBeforeSubmerged = 1f;
    public float basicDisplacementAmount = 3f;

    // Update is called once per frame
    private void Update()
    {
        if (transform.position.y < 0)
        {
            float displacementMultiplier = Mathf.Clamp01(-transform.position.y / basicDepthBeforeSubmerged) * basicDisplacementAmount;
            rigidBody.AddForce(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), ForceMode.Acceleration);
        }
    }
}
