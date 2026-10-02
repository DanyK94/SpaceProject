using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float thrust = 10000f;
    [SerializeField] private float reverseThrust = 5000f;

    [Header("Rotation")]
    [SerializeField] private float rotationTorque = 5000f;

    private Rigidbody rb;
    private ShipControls controls;

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

    private void FixedUpdate()
    {
        HandleTranslation();
        
    }

    private void HandleTranslation()
    {
        Vector2 move = controls.Ship.Move.ReadValue<Vector2>();
        /*
        float forward = move.y;

        if(forward > 0f)
        {
            rb.AddForce(
                transform.forward * thrust * forward,
                ForceMode.Force
            );
        }
        else if(forward < 0f)
        {
            rb.AddForce(
                transform.forward * reverseThrust * forward,
                ForceMode.Force
            );
        }
        */

        float vertical = controls.Ship.MoveVertical.ReadValue<float>();

        Vector3 thrustVector = new Vector3( move.x, vertical, move.y);

        rb.AddRelativeForce( thrustVector * thrust, ForceMode.Force);
    }

    private void HandleRotation()
    {
        Vector2 look = controls.Ship.Look.ReadValue<Vector2>();

        float pitch = -look.y;
        float yaw = look.x;

        float roll = controls.Ship.Roll.ReadValue<float>();

        Vector3 torque = transform.right * pitch + transform.up * yaw + transform.forward * roll;

        rb.AddTorque(
            torque * rotationTorque,
            ForceMode.Force
        );
    }

}
