using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeloControl : MonoBehaviour

{

    Rigidbody rb_Heli;

    [SerializeField] public float responsiveness = 1.5f;
    [SerializeField] public float throttleAmt = 10f;
    float throttle; // percentage
    float throttleDebugTimer = 0.5f;
    float roll;
    float pitch;
    float yaw;

    [SerializeField] float rotorSpeedModifier = 2f;
    [SerializeField] Transform rotorsTransform;


    private void Awake()
    {
        rb_Heli = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Start the repeating timer to call ThrottleDebugLog() method every 0.5 seconds
        InvokeRepeating("ThrottleDebugLog", 0f, throttleDebugTimer);
    }

    private void Update()
    {
        HandelInputs();

        rotorsTransform.Rotate(Vector3.up * throttle * rotorSpeedModifier);

    }

    private void FixedUpdate()
    {
        rb_Heli.AddForce(transform.up * throttle, ForceMode.Impulse);

        rb_Heli.AddTorque(transform.right * pitch * responsiveness);
        rb_Heli.AddTorque(-transform.forward * roll * responsiveness);
        rb_Heli.AddTorque(transform.up * yaw * responsiveness);
    }

    private void HandelInputs()
    {
        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");
        //yaw = Input.GetAxis("Yaw");

    float yawInput = Input.GetAxis("Yaw");
        if (Mathf.Abs(yawInput) > 0.01f)
        {
            yaw = yawInput;
        }
        else
        {
            yaw = 0f; // Reset yaw to 0 if the yawInput is close to 0.
        }

        if (Input.GetKey(KeyCode.Space))
        {
            throttle += Time.deltaTime * throttleAmt;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            throttle -= Time.deltaTime * throttleAmt;
        }

        throttle = Mathf.Clamp(throttle, 0f, 60f);
    }

    #region METH
    public void ResetThrottle()
    {
        throttle = 0f;
    }

    public float Throttle()
    {
        return throttle ;
    }

    public float GetRotorSpeedModifier()
    {
        return rotorSpeedModifier;
    }

    public Transform GetRotorsTransform()
    {
        return rotorsTransform;
    }

    private void ThrottleDebugLog()
    {
        Debug.Log("THROTTLE: " + throttle);
    }

    #endregion
}
