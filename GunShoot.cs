using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///         !!! Tag main camera
///         gun script on gun
///         no collider on gun
///         Health ref script: HealthSub . Add this to Obj to destroy
///         Rytech How to make a gun
/// </summary>
/// 
public class GunShoot : MonoBehaviour
{

    //ray from gun intersects this layer
    [SerializeField] private LayerMask PurpleCubesLayer;
    [SerializeField] float hitDistance;

    [SerializeField] private float Damage; // sub health
    private Camera PlayerCamera;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerCamera = Camera.main; // check insp. tag is MainCamera

    }

    // Update is called once per frame
    void Update()
    {
        // gun ray(location, direction)
        // check what is hit , distance, shoot obj on this layer
        // out: return data
        if (Input.GetMouseButtonDown(0))
        {
            Ray gunray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
            if(Physics.Raycast(gunray, out RaycastHit hitInfo, hitDistance, PurpleCubesLayer))
            {
                Debug.Log("hit on something");
                // destruction via health sub
                // subtract health fn. HealthSub::SubtractHealth
                // return info from obj's w. HealthSub script
                if(hitInfo.collider.gameObject.TryGetComponent(out HealthSub objHit))
                {
                    objHit.SubtractHealth(Damage);
                    Debug.Log(objHit.Health);
                }

                // instant destruction
                //Destroy(hitInfo.collider.gameObject);
            }
        }
    }
}
