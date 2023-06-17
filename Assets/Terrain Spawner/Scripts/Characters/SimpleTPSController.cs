using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class SimpleTPSController : MonoBehaviour
{

    public float forwardWalkingSpeed = 7.5f;
    public float backwardWalkingSpeed = 5.0f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    Camera characterCamera;
    CharacterController characterController;
    public Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    public float vertical ;
    public float horizontal ;
    public bool isRunning ;

    // Start is called before the first frame update
    void Start()
    {
        //characterCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();

        
    }

    // Update is called once per frame
    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);

        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        float curSpeedX = 0;
        float curSpeedY = 0;

        if (vertical > 0.0f)
        {
            curSpeedX = canMove ? (isRunning ? runningSpeed : forwardWalkingSpeed) * vertical : 0;
            curSpeedY = canMove ? (isRunning ? runningSpeed : forwardWalkingSpeed) * horizontal : 0;
        }
        else
        {
            curSpeedX = canMove ? backwardWalkingSpeed * vertical : 0;
            curSpeedY = canMove ? backwardWalkingSpeed * horizontal : 0;
        }

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            //rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            //rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            //characterCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
