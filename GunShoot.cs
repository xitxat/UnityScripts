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

    [SerializeField] private FiringModes CurrentFiringMode = FiringModes.Auto;
    [SerializeField] private LayerMask PurpleCubesLayer;        //ray from gun intersects this layer
    [SerializeField] float hitDistance;                  // ~100f
    [SerializeField] private float Damage;               // sub health
    [SerializeField] private float FireRate;
    [SerializeField] private AudioSource GunSoundSource;
    [SerializeField] private AudioClip GunSound;

    // TODO: EXTRAS
    [SerializeField] private ParticleSystem MuzzleFlash;


    private Camera PlayerCamera;
    private float TimeBeforeShooting;
    private bool Shooting;                  // IEnumerator burst
    private int FireModeID = 1;                 // case selector mapped to Key V


    public enum FiringModes
    {
        //case selectors
        Semi =1,
        Auto =2,
        Burst =3
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerCamera = Camera.main; // check insp. tag is MainCamera

        TimeBeforeShooting = 1 / FireRate;

    }



    IEnumerator BurstShots(int TimesToShoot)
    {   // Shoot
        // When running bool shooting is true

        for (int TimesShot = 1; TimesShot <= TimesToShoot; TimesShot++)
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
            yield return new WaitForSeconds(1f / FireRate);
        }

        Shooting = false;

    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            FireModeID++;

            if(FireModeID > 3)
            {
                FireModeID = 1;
            }

            CurrentFiringMode = (FiringModes)FireModeID;

        }
        switch (CurrentFiringMode)
        {
            case FiringModes.Semi:
                Semi();
                break;

            case FiringModes.Burst:
                Burst();
                break;

            case FiringModes.Auto:
                Auto();
                break;  

        }
    }// end update

    void Burst()
    {
        if (Input.GetMouseButtonDown(0) && !Shooting)
        {
            StartCoroutine(BurstShots(3));
            Shooting = true;
        }
    }

    void Auto()
    {
        //add Health Script(HealthSub) to objects to be destroyed


        if (Input.GetMouseButton(0))
        {
            if (TimeBeforeShooting <= 0f)
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
                // reset the time on shoot, allow tap fire
                TimeBeforeShooting = 1 / FireRate;
            }
            else
            {
                TimeBeforeShooting -= Time.deltaTime;
            }
        }// end rapid w health

        // No mouse button
        else
        {
            TimeBeforeShooting = 0f;
        }
    }

    void Semi()
    {
         //add Health Script(HealthSub) to objects to be destroyed
         //gun ray(location, direction)
         //check what is hit , distance, shoot obj on this layer
         //out: return data

        if (Input.GetMouseButtonDown(0))
        {
            GunSoundSource.PlayOneShot(GunSound);

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

                // instant destruction
                //Destroy(hitInfo.collider.gameObject);
            }
        }
    }

    void FiringLibrary() // Base Methods
    {
        // Pulled from Update() and replaced with Switch

        #region  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~SEMI + Instant Destruction~~~~~~~~~~~~~~~~~~
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


        #region  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~SEMI + HEALTH~~~~~~~~~~~~~~~~~~
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


        #region       //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AUTO FIRE + HEALTH~~~~~~~~~~~~~~~~
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

    }
}// end class
