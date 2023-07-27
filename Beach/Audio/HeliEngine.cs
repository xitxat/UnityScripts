using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliEngine : MonoBehaviour
{
    public AudioSource EngineLowLoad;
    public AudioSource EngineHighLoad;

    //const float RPMScale = 100;

    float rpm;
    float power;

    bool isIdle = false;

    // Constants for RPM limits
    const float maxRPMIdle = 0.1f;
    const float maxRPMNonIdle = 3f;

    void Start()
    {
        EngineLowLoad.loop = true;
        EngineLowLoad.Play();

        EngineHighLoad.loop = true;
        EngineHighLoad.Play();

        rpm = 0.5f;
    }

    void Update()
    {
        float gasPedal = 0.0f;

        if (Input.GetKey(KeyCode.Space))
        {
            gasPedal = 1.0f;
            isIdle = false;
        }
        else if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            gasPedal = 0.5f;
            isIdle = false;
        }
        else
        {
            // Idle state
            gasPedal = 0.1f;
            isIdle = true;
        }

        // Power tweens towards the gasPedal setting
        power = Mathf.MoveTowards(power, gasPedal, 2.0f * Time.deltaTime);

        // Calculate the volume for the EngineLowLoad audio source
        float volLowLoad;
        if (isIdle)
        {
            // Idle state, set a constant volume
            volLowLoad = 0.5f;
        }
        else
        {
            // Non-idle state, interpolate between 0.8f and 0.4f volume
            volLowLoad = Mathf.Lerp(0.8f, 0.4f, power);
        }

        // Calculate the volume for the EngineHighLoad audio source
        float volHighLoad = Mathf.Lerp(0.0f, 1.0f, power);

        // Set the volume for both audio sources
        EngineLowLoad.volume = volLowLoad;
        EngineHighLoad.volume = volHighLoad;


        // Calculate the desired RPM based on gasPedal value and state
        float maxRPM = isIdle ? maxRPMIdle : maxRPMNonIdle;
        float desiredRPM = Mathf.Clamp(0.1f + 2.0f * gasPedal, 0f, maxRPM);

        // Gradually adjust RPM towards desired RPM
        rpm = Mathf.Lerp(rpm, desiredRPM, 0.15f * Time.deltaTime);
        rpm = Mathf.MoveTowards(rpm, desiredRPM, 0.05f * Time.deltaTime);

        // Calculate the pitch based on RPM
        float pitch = Mathf.Pow(2, rpm);

        // Set the pitch for both audio sources
        EngineLowLoad.pitch = pitch;
        EngineHighLoad.pitch = pitch;

        int revs = (int)(rpm);
        //Debug.Log("RPM: " + revs);
    }
}
