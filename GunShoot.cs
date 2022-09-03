using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///         !!! Tag main camera
///         gun script on gun
///         no collider on gun
///         no health script = instant destruction        
/// 
///             AUDIO
///             mp3 to asset folder
///             Load into AudioClip GunSound
///             Drag Gun obj into  AudioSource GunSoundSource           
/// 
///         Health ref script: HealthSub . Add this to Obj to destroy
///         Rytech How to make a gun
/// </summary>
/// 
public class GunShoot : MonoBehaviour
{

    //ray from gun intersects this layer
    [SerializeField] private LayerMask PurpleCubesLayer;
    [SerializeField] float hitDistance;

    [SerializeField] private float Damage;  // sub health

    // Gun fire rate / mode
    [SerializeField] private float FireRate;
    private float TimeBeforeShooting;
    private bool Shooting;                  //IEnumerator burst

    [SerializeField] private AudioSource GunSoundSource;
    [SerializeField] private AudioClip GunSound;

    // TODO: EXTRAS
    [SerializeField] private ParticleSystem MuzzleFlash;


    private Camera PlayerCamera;





    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerCamera = Camera.main; // check insp. tag is MainCamera

        TimeBeforeShooting = 1 / FireRate;

    }

    //~~~~~~~~~~BURST SHOTS~~~~~~~~~~~~~~~~
    IEnumerator BurstShots(int TimesToShoot)
    {   // Shoot
        // When running bool shooting is true

         for(int TimesShot = 1; TimesShot <= TimesToShoot; TimesShot++)
        {

            //extras TODO
            GunSoundSource.PlayOneShot(GunSound);
            //MuzzleFlash.Play();

            Ray gunray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
            if (Physics.Raycast(gunray, out RaycastHit hitInfo, hitDistance, PurpleCubesLayer))
            {
                Debug.Log("hit on something");
                // destruction via health sub
                // subtract health fn. HealthSub::SubtractHealth
                // return info from obj's w. HealthSub script
                if (hitInfo.collider.gameObject.TryGetComponent(out HealthSub objHit))
                {
                    objHit.SubtractHealth(Damage);
                    Debug.Log(objHit.Health);
                }
            }

            // >Then wait - coroutine
            yield return new WaitForSeconds(1f/FireRate);
        }

        Shooting = false;

    }
    // Update is called once per frame
    void Update()
    {
        #region  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~ONE SHOT + Instant Destruction~~~~~~~~~~~~~~~~~~
        // No HealthSub script on destruction objects
        // gun ray(location, direction)
        // check what is hit , distance, shoot obj on this layer
        // out: return data

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray gunray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
        //    if(Physics.Raycast(gunray, out RaycastHit hitInfo, hitDistance, PurpleCubesLayer))
        //    {
        //        Debug.Log("hit on something");
        //        // instant destruction
        //        Destroy(hitInfo.collider.gameObject);
        //    }
        //}
        #endregion


        #region  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~ONE SHOT + HEALTH~~~~~~~~~~~~~~~~~~
        // add Health Script (HealthSub) to objects to be destroyed
        // gun ray(location, direction)
        // check what is hit , distance, shoot obj on this layer
        // out: return data

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray gunray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
        //    if(Physics.Raycast(gunray, out RaycastHit hitInfo, hitDistance, PurpleCubesLayer))
        //    {
        //        Debug.Log("hit on something");
        //        // destruction via health sub
        //        // subtract health fn. HealthSub::SubtractHealth
        //        // return info from obj's w. HealthSub script
        //        if(hitInfo.collider.gameObject.TryGetComponent(out HealthSub objHit))
        //        {
        //            objHit.SubtractHealth(Damage);
        //            Debug.Log(objHit.Health);
        //        }

        //        // instant destruction
        //        //Destroy(hitInfo.collider.gameObject);
        //    }
        //}
        #endregion


        #region       //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~RAPID FIRE + HEALTH~~~~~~~~~~~~~~~~
        // add Health Script (HealthSub) to objects to be destroyed


        //if (Input.GetMouseButton(0))
        //{
        //    if (TimeBeforeShooting <= 0f)
        //    {
        //        //extras TODO
        //        GunSoundSource.PlayOneShot(GunSound);
        //        //MuzzleFlash.Play();

        //        Ray gunray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
        //        if (Physics.Raycast(gunray, out RaycastHit hitInfo, hitDistance, PurpleCubesLayer))
        //        {
        //            Debug.Log("hit on something");
        //            // destruction via health sub
        //            // subtract health fn. HealthSub::SubtractHealth
        //            // return info from obj's w. HealthSub script
        //            if (hitInfo.collider.gameObject.TryGetComponent(out HealthSub objHit))
        //            {
        //                objHit.SubtractHealth(Damage);
        //                Debug.Log(objHit.Health);
        //            }
        //        }
        //        // reset the time on shoot, allow tap fire
        //        TimeBeforeShooting = 1 / FireRate;
        //    }
        //    else
        //    {
        //        TimeBeforeShooting -= Time.deltaTime;
        //    }
        //}// end rapid w health

        //// No mouse button
        //else
        //{
        //    TimeBeforeShooting = 0f;
        //}
        #endregion


        #region       //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~BURST FIRE + HEALTH~~~~~~~~~~~~~~~~
        // add Health Script (HealthSub) to objects to be destroyed

        // if not shooting when mouse 0 click - start burst shot Coroutine...
        if (Input.GetMouseButton(0) && !Shooting)
        {
            // moved fire control to IEnumerator
            // bool Shooting false if no mouse button

            StartCoroutine(BurstShots(3));
            Shooting = true;


        }// end burst w health

        #endregion

    }// end update
}// end class
