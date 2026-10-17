using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Character Heights")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchingHeight = 1f;

    [Header("Camera")]
    [SerializeField] private PlayerCameraController playerCameraController;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float standingCameraHeight = 1.6f;
    [SerializeField] private float crouchingCameraHeight = 0.8f;

    [Header("Look")]
    [SerializeField] private float lookSensitivity = 0.1f;


    /*
    [SerializeField] private float gravity = -20f;
    


    [Header("Look")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float lookSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 85f;
    */

    private GameControls controls;
    private CharacterController charController;
    private Collider[] playerColliders;
    private Collider[] shipColliders;

    private float verticalVelocity;
    private float cameraPitch;


    private bool isSeated;
    private Transform currentSeatPoint;

    private void Awake()
    {
        controls = new GameControls();
        charController = GetComponent<CharacterController>();
        playerColliders = GetComponentsInChildren<Collider>(true);
    }

    private void OnEnable()
    {
        controls.Player.Enable();        
    }

    private void OnDisable()
    {
        controls.Player.Disable();        
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
   

    private void Update()
    {   
        

        if (isSeated)
        {
            return;
        }
        HandleLook();
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

    private void HandleLook()
    {
        Vector2 look = controls.Player.Look.ReadValue<Vector2>();

        

        cameraPitch -= look.y * lookSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);

        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        if (!isSeated)
        {
            transform.Rotate(Vector3.up, look.x * lookSensitivity);
        }
    }


    //SEAT
    public void EnterSeat(Transform seatPoint)
    {
        if (isSeated || seatPoint == null)
        {
            return;
        }
        if (playerCameraController != null)
        {
            playerCameraController.SetSeatedMode(true);
        }

        charController.enabled = false;

        isSeated = true;
        currentSeatPoint = seatPoint;

        transform.SetParent(seatPoint,true);
        transform.SetPositionAndRotation(currentSeatPoint.position, currentSeatPoint.rotation);
    }

    public void ExitSeat(Transform exitPoint)
    {
        if (!isSeated || exitPoint == null)
        {
            return;
        }
        if (playerCameraController != null)
        {   
            playerCameraController.SetSeatedMode(false);
        }

        charController.enabled = false;
        transform.SetParent(null, true);

        transform.SetPositionAndRotation(exitPoint.position, exitPoint.rotation);

        charController.enabled = true;

        verticalVelocity = 0f;
        isSeated = false;
        currentSeatPoint = null;
    }

}
