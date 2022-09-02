using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// FROM: RYTECH https://www.youtube.com/watch?v=b1uoLBp2I1w&t=730s
/// drag this players inspector rigidbody tab to script/ playerbody field
/// No mid air jump - volume check on jump. <movepl() checkSphere
///     create empty cont."Feet" , place under players feet -1( inside plr. obj.) , 
///     sel floor plane, add layer
///     code check sphere
///     Assign all ground jump planes to floor plane!!!
///     assign   FloorLayer to  [SerializeField] FloorMask 
///     assign    Feet transform obj to [SerializeField] FeetTransform  


/// </summary>
public class RigidBodyMovement : MonoBehaviour
{

    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;
    private float xRot; // camera control

    //No air jump
    [SerializeField] private LayerMask FloorMask;
    [SerializeField] private Transform FeetTransform;

    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private Rigidbody PlayerBody;
    [Space]
    [SerializeField] private float Speed;
    [SerializeField] private float Sensitivity;
    [SerializeField] private float Jumpforce;


    // Hide ingame mouse
    //private void Start()
    //{
    //    Cursor.lockState= CursorLockMode.Locked;
    //}




    // Update is called once per frame
    void Update()
    {
        // Hor = A & D keys, ws. GetAxisRaw has no smoothing filter
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        MovePlayerCamera();
    }

    private void FixedUpdate()
    {

    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput) * Speed;

        // not multiplying by Speed bc PlayerBody.velocity.y, when falling will be * too fast. Only want gravity
        // speed only on the wasd
        PlayerBody.velocity = new Vector3( MoveVector.x, PlayerBody.velocity.y, MoveVector.z);

        //JUMPING

        if (Input.GetKeyDown(KeyCode.Space))
        {

            //  no air jump
            if (Physics.CheckSphere(FeetTransform.position, 0.5f, FloorMask))
            {
                PlayerBody.AddForce(Vector3.up * Jumpforce, ForceMode.Impulse);         // impluse = like explosion, instantanious
            }
        }
    }

    //  bind cam also to mouse  Y axis
    private void MovePlayerCamera()
    {
        // X rotation - around the X axis for looking up / down
        xRot -= PlayerMouseInput.y * Sensitivity;

        //  restrict look behind player
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.Rotate(0f, PlayerMouseInput.x * Sensitivity, 0f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);  
    }


} // end class
