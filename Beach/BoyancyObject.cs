using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// YT: BOYANCY WITH UNITY RIGIDBODIES
// Auto aadd  this component
[RequireComponent(typeof(Rigidbody ))]


public class BoyancyObject : MonoBehaviour
{

                                        // add forces t different parts of the rigid body
public Transform[] floaters;            // eg for floating obj's to be walked on

public float underWaterDrag = 3f;
public float underWaterAngularDrag = 1f;
public float airDrag = 0f;
public float airAngularDrag = 0.05f;

public float    floatingPower = 150f;
public float waterHeight = 0f;
bool underWater;
Rigidbody mRigidbody;

// if all floaters are above the water then the rigidbody is above the water
// if 1 floater is below Apply WATERDRAG
int floatersUnderwater;


    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();

    }

    // Update Physics
    void FixedUpdate()
    {
         floatersUnderwater = 0;

        // for all FLOATETRS obj's. Check the floater position
        for(int i = 0; i < floaters.Length; i++ )
        { 
            float difference = floaters[i].position.y - waterHeight;

            // if underwter & the further underwater add more force
            // to the position of the rigidBody
            if(difference < 0)
            {
                mRigidbody.AddForceAtPosition(Vector3.up*floatingPower*Mathf.Abs(difference), floaters[i].position, ForceMode.Force);
                floatersUnderwater += 1;
                
                if(!underWater)
                {
                    underWater = true;
                    // if Switched in this frame
                    SwitchState(true); 
                }
            }
        }

        
         if (underWater && floatersUnderwater == 0)
        {
            underWater=false;
            SwitchState(false);
        }
    }// end F.Update

    // change between underWaterDrag & airDrag
    void SwitchState(bool isUnderWater)
    {
        if(isUnderWater)
        {
            mRigidbody.drag = underWaterDrag;
            mRigidbody.angularDrag = underWaterAngularDrag;
        }
        else
        {
            mRigidbody.drag = airDrag;
            mRigidbody.angularDrag = airAngularDrag;
        }
    }
}
