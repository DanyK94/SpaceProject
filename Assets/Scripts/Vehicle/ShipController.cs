using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float thrust = 50f;

    [Header("Rotation Tracking")]
    [SerializeField] private float rotationTorque = 10f;
    [SerializeField] private float maxTrackingTorque = 2000f;

    [Header("Fligh Assist Forces")]
    [SerializeField] private float breakingAcceleration = 10f;
    [SerializeField] private float rotationBrakingTorque = 25f;

    [Header("Aim Control")]
    [SerializeField] private float deadzoneRadius = 100f;
    [SerializeField] private float maxControlRadius = 300f;
    [SerializeField] private float maxAimRotationSpeed = 90f;

    private Rigidbody rb;
    private GameControls controls;
    private bool pilotActive;

    //FLIGHT CONTROL MODE
    private bool manoeuvringEnabled = false;
    private bool flightAssistEnabled = true;

    //MOVE TRACKING
    private Quaternion desiredRotation;
    private Quaternion previousLocalAimRotation;
    private bool hasPreviousAimRotation;

    //FUNCT
    public Vector3 LinearVelocity => rb.linearVelocity;
    public Vector3 AngularVelocity => rb.angularVelocity;
    public bool FlightAssistEnabled => flightAssistEnabled;
    public bool PilotActive  => pilotActive;
    public bool ManoeuvringEnabled => manoeuvringEnabled;


    // **** //
    //  V2.0 Movement 
    [Header("References")]
    [SerializeField] private PlayerCameraController  cameraController;
    [SerializeField] private Transform aimReference;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new GameControls();
        desiredRotation = rb.rotation;

    }

    private void OnEnable()
    {
        controls.Ship.Disable();        
    }

    private void OnDisable()
    {
        controls.Disable();        
    }

    public void SetPilotActive(bool active)
    {
        pilotActive = active;

        if (pilotActive){
            controls.Ship.Enable();
        } else {
            controls.Ship.Disable();
            manoeuvringEnabled = false;
        }
    }

    private void Update()
    {
        if (!pilotActive)
        {
            return;
        }

        HandleModeToggle();
        if (manoeuvringEnabled)
        {
            HandleAimInput();

        }

        HandleFlightAssistInput();
    }

    private void FixedUpdate()
    {
        if (!pilotActive)
        {
            return;
        }

        HandleTranslation();
        HandleRoll();
        if (manoeuvringEnabled)
        {
            HandleRotation();
        }

        //FLIGT ASSISTAN BRAKE
        if (flightAssistEnabled || controls.Ship.Brake.IsPressed())
        {
            HandleFlightAssist();
        }
        
    }

    private void HandleModeToggle()
    {
        if (!controls.Ship.ToggleManoeuvring.WasPressedThisFrame())
        {
            return;
        }

        manoeuvringEnabled = !manoeuvringEnabled;

        if (manoeuvringEnabled && cameraController != null)
        {
            desiredRotation = cameraController.AimRotation;
            previousLocalAimRotation = cameraController.LocalAimRotation;
            hasPreviousAimRotation = true;
        }
        else
        {
            hasPreviousAimRotation = false;
        }


        Debug.Log("Manoeuvring: " + manoeuvringEnabled);
    }
    
    private void HandleAimInput()
    {
        if (cameraController == null)
        {
            return;
        }

        Quaternion currentLocalAimRotation = cameraController.LocalAimRotation;

        if (!hasPreviousAimRotation)
        {
            previousLocalAimRotation = currentLocalAimRotation;
            hasPreviousAimRotation = true;
            return;
        }

        Quaternion localDelta = Quaternion.Inverse(previousLocalAimRotation) * currentLocalAimRotation;
        desiredRotation *= localDelta;
        previousLocalAimRotation = currentLocalAimRotation;

    }

    private void HandleFlightAssistInput()
    {
        if (controls.Ship.FlightAssist.WasPressedThisFrame())
        {
            flightAssistEnabled = !flightAssistEnabled;
            Debug.Log("Flight Assist: " + flightAssistEnabled);
        }
    }

    private void HandleTranslation()
    {
        Vector2 move = controls.Ship.Move.ReadValue<Vector2>();
        float vertical = controls.Ship.MoveVertical.ReadValue<float>();

        Vector3 thrustVector = new Vector3( move.x, vertical, move.y);

        if (flightAssistEnabled || controls.Ship.Brake.IsPressed())
        {
            Vector3 velocity = rb.linearVelocity;
            Vector3 breakingForce = -velocity.normalized * breakingAcceleration * rb.mass;
            rb.AddForce(breakingForce, ForceMode.Force);            
        }

        rb.AddRelativeForce( thrustVector * thrust, ForceMode.Force);
    }

    private void HandleFlightAssist()
    {
        Vector3 velocity = rb.linearVelocity;
        
        if (velocity.sqrMagnitude > 0.01f)
        {
            Vector3 breakingForce = -velocity.normalized * breakingAcceleration * rb.mass;
            rb.AddForce(breakingForce, ForceMode.Force);
        }
   
    }


    // *********************************** //
    // ROLL CONTROL
    private void HandleRoll()
    {
        float rollInput = - controls.Ship.Roll.ReadValue<float>();

        Vector3 rollAxis = transform.forward;
        Vector3 rollTorque = rollAxis * rollInput * rotationTorque;

        Vector3 brakingTorque = Vector3.zero ;

        if ((flightAssistEnabled || controls.Ship.Brake.IsPressed()) && rollInput == 0)
        {
           Vector3 rollAngularvelocity = Vector3.Project(rb.angularVelocity, rollAxis);
           brakingTorque = - rollAngularvelocity * rotationBrakingTorque;
        }
        
        Debug.Log(
            $"Roll input: {rollInput:F2} | " +
            $"Command: {rollTorque} | " +
            $"Braking: {rotationBrakingTorque} | " +
            $"Angular velocity: {rb.angularVelocity} | " +
            $"FA: {flightAssistEnabled}"
        );
        

        rb.AddTorque(rollTorque + brakingTorque, ForceMode.Force);
        
    }

    // *********************************** //
    // YAW PITCH MOVEMENT CONTROL SYSTEM

    private Vector2 GetHudOffset()
    {
        if (cameraController == null || aimReference == null)
        {
            return Vector2.zero;
        }

        Vector2 hudPosition = cameraController.GetAimReferenceScreenPosition(aimReference);
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        return screenCenter - hudPosition;
    }

    private float GetAimInputStrength()
    {
        float distance = GetHudOffset().magnitude;
        if (distance <= deadzoneRadius) return 0f;
        return Mathf.Clamp01( (distance - deadzoneRadius) / (maxControlRadius - deadzoneRadius) );
    }

    private Vector2 GetAimInput()
    {
        Vector2 offset = GetHudOffset();
        if (offset.magnitude <= deadzoneRadius) return Vector2.zero;
        Vector2 direction = offset.normalized;
        float strength = GetAimInputStrength();
        //Debug.Log("Dir: " + direction + " \n Strenght: " + strength);
        return direction * strength;
    }

    private Vector3 GetAimAngularVelocity() //VEDERE SE CANCELLARE E BYPASSARE
    {
        Vector2 aimInput = GetAimInput(); // (0,1)
        float pitchSpeed = aimInput.y * maxAimRotationSpeed;
        float yawSpeed = aimInput.x * maxAimRotationSpeed;
        //Debug.Log($"Pitch Speed: {aimInput.y} * " + $"Max Angular: {maxAimRotationSpeed} = " +  pitchSpeed);
        return new Vector3(-pitchSpeed, yawSpeed, 0f) * Mathf.Deg2Rad;
    }
    
    private void HandleRotation()
    {
        Vector3 aimAngularVelocity = GetAimAngularVelocity(); //(0,1) valori normalizzati vedere se cambiare
        Vector3 localAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity);
        Vector3 angualarVelocityDelta = new Vector3(aimAngularVelocity.x - localAngularVelocity.x, aimAngularVelocity.y - localAngularVelocity.y, 0f);

        Vector3 trackingTorque = transform.TransformDirection(angualarVelocityDelta * rotationTorque);

        if (flightAssistEnabled || controls.Ship.Brake.IsPressed())
        {
            Vector3 localBrackingVelocity = new Vector3(localAngularVelocity.x, localAngularVelocity.y, 0f);
            trackingTorque -= transform.TransformDirection(localBrackingVelocity * rotationBrakingTorque);
        }

        trackingTorque = Vector3.ClampMagnitude(trackingTorque, maxTrackingTorque);
        /*
        Debug.Log(
            $"Offset: {GetHudOffset()} | " +
            $"Distance: {GetHudOffset().magnitude:F2} | " +
            $"Strength: {GetAimInputStrength():F3} | " +
            $"AimInput: {GetAimInput()} | " +
            $"Angular Target: {aimAngularVelocity} | " +
            $"Angular Delta: {angualarVelocityDelta} | " +
            $"Rotation Torque {rotationTorque} |" +
            $"Command Torque: {trackingTorque} | " +
            $"Braking: {rotationBrakingTorque} | " +
            $"Angular velocity: {rb.angularVelocity} |--| " +
            $"Manouvre Mode: {manoeuvringEnabled} | "+
            $"FA: {flightAssistEnabled}"
        );
        */     

        rb.AddTorque(trackingTorque, ForceMode.Force);
    }

}
