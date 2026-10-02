using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float thrust = 10000f;

    [Header("Rotation")]
    [SerializeField] private float rotationTorque = 5000f;
    [SerializeField] private float mouseRange = 300f;

    private Rigidbody rb;
    private ShipControls controls;

    private Vector2 rotationInput;
    private Vector2 rotationCenter;

    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new ShipControls();
    }

    private void OnEnable()
    {
        controls.Enable();        
    }

    private void OnDisable()
    {
        controls.Disable();        
    }

    private void Update()
    {
        if (controls.Ship.RotateShip.WasPressedThisFrame())
        {
            rotationCenter = Mouse.current.position.ReadValue();
            //Debug.Log("Rotation Center -> " + rotationCenter);
        }
        if (controls.Ship.RotateShip.IsPressed())
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 delta = (mousePosition - rotationCenter) / mouseRange;

            rotationInput = Vector2.ClampMagnitude(delta, 1f);

            //Debug.Log("Mouse Position -> " + mousePosition + "Rotation Input -> " + rotationInput);
        }
        else
        {
            rotationInput = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        HandleTranslation();
        HandleRotation();
        
    }

    private void HandleTranslation()
    {
        Vector2 move = controls.Ship.Move.ReadValue<Vector2>();

        float vertical = controls.Ship.MoveVertical.ReadValue<float>();

        Vector3 thrustVector = new Vector3( move.x, vertical, move.y);

        rb.AddRelativeForce( 
            thrustVector * thrust, 
            ForceMode.Force);
    }

    private void HandleRotation()
    {
       
        float pitch = -rotationInput.y;
        float yaw = rotationInput.x;
        float roll = controls.Ship.Roll.ReadValue<float>();

        Vector3 torque = new Vector3(pitch,yaw,roll);

        rb.AddRelativeTorque(
            torque * rotationTorque,
            ForceMode.Force
        );
    }

}
