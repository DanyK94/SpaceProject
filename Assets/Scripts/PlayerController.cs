using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Character Heights")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchingHeight = 1f;

    [Header("Character Camera Heights")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float standingCameraHeight = 1.6f;
    [SerializeField] private float crouchingCameraHeight = 0.8f;


    /*
    [SerializeField] private float gravity = -20f;
    


    [Header("Look")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float lookSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 85f;
    */

    private GameControls controls;
    private CharacterController charController;

    private float verticalVelocity;

    private void Awake()
    {
        controls = new GameControls();
        charController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        controls.Player.Enable();        
    }

    private void OnDisable()
    {
        controls.Player.Disable();        
    }
   

    private void Update()
    {
        HandleMovement();
        HandleCrouch();
        //IF IN GRAVITY

        //IF GRAVITY 0
        // HANDLE VERTICAL WITH DownCrouch key and UpJump key
    }

    private void HandleMovement()
    {
        Vector2 move = controls.Player.Move.ReadValue<Vector2>();
        Vector3 movement = transform.right * move.x + transform.forward * move.y;

        movement *= moveSpeed;

        //NOTE: TO MANAGE IF ZERO GRAVITY      
        if (charController.isGrounded)
        {
            if(verticalVelocity < 0f)
            {
                verticalVelocity = -1f;
            }
            if (controls.Player.Up.WasPressedThisFrame())
            {
                verticalVelocity = jumpForce;
            }
        } else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        movement.y = verticalVelocity;

        charController.Move(movement * Time.deltaTime);



    }

    private void HandleCrouch()
    {
        if (controls.Player.Down.IsPressed())
        {
            charController.height = crouchingHeight;
            charController.center = new Vector3(0f, crouchingHeight / 2f, 0f);

            playerCamera.localPosition = new Vector3( 0f, crouchingCameraHeight , 0f);
        }
        else
        {
            charController.height = standingHeight;
            charController.center = new Vector3(0f, standingHeight / 2f, 0f);

            playerCamera.localPosition = new Vector3( 0f, standingCameraHeight , 0f);
        }
    }

}
