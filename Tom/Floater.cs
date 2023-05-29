using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{

    public Rigidbody rigidBody;
    public float depthBeforeSubmerged = 1f;
    public float displaccementAmount = 3f;
    public int floaterCount = 1;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;


    private void FixedUpdate()
    {
        // Stablise floater's gravity (while above water)(not in if st,)
        // reduce gravioty by # of Floaters 
        // Select in INSPECTOR floaters group of a RB obj and set fCount
        rigidBody.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);


        // height @ X coord
        float waveHeight = WaveManager.instance.GetWaveHeight(transform.position.x);

        // is Y under the wave ?
        if (transform.position.y < waveHeight)
        {
            float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displaccementAmount;

            // using fm.Acceleration so mass doesnt affect boyancy
            // AtPosition: passing in floaters pos. 
            rigidBody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
            rigidBody.AddForce(displacementMultiplier * -rigidBody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rigidBody.AddTorque(displacementMultiplier * -rigidBody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
