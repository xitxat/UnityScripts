using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// https://www.youtube.com/watch?v=zgCV26yFAiU
/// 
/// Create a new layer called Pickup
///     Assign this layer to pick upable objects
///     Assign rigidbody component to these obje 
///         PIckup Range 10
/// Pickup objects  Rigidbody set interpolate to Interpolate for smoothness
/// </summary>
/// 


public class PhysicsPickup : MonoBehaviour
{


    [SerializeField] private LayerMask PickupMask;      // player can pick up objects on this layer
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private Transform PickupTarget;    // target selection ray
    [Space]
    [SerializeField] private float PickupRange;
    private Rigidbody CurrentObject;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //  E key to interact
        if (Input.GetKeyDown(KeyCode.E))
        {

            // check if currently holding an object
            if (CurrentObject)
            {
                CurrentObject.useGravity = true;
                CurrentObject = null;
                return;
            }



            // object in middle of screen
            Ray CameraRay = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            //  if ray hit an object assign its rigidbody to CurrentObject var & disable gravity
            if(Physics.Raycast(CameraRay, out RaycastHit HitInfo, PickupRange, PickupMask  ))
            {
                CurrentObject = HitInfo.rigidbody;
                CurrentObject.useGravity = false;   
            }

        }
    }

    // change velocity of rigidbody of selected object
    // FixedUpdate runs off a physics timestamp
  void FixedUpdate()
    {
        Vector3 DirectionToPoint = PickupTarget.position - CurrentObject.position;
        float DistanceToPoint = DirectionToPoint.magnitude;

        // 12f is the speed var
        CurrentObject.velocity = DirectionToPoint * 12f * DistanceToPoint;

    }


}   //  end class
