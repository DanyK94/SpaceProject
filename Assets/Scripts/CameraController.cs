using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera secondaryCamera;


    [Header("FPV")]
    [SerializeField] private float fpvSensitivity = 0.1f;
    [SerializeField] private float fpvMaxPitch = 85f;

    [Header("TPV")]
    [SerializeField] private float tpvDistance = 6f;
    [SerializeField] private float tpvHeight = 2f;
    [SerializeField] private float tpvSensitivity = 0.1f;

    private bool isFPV = true;
    private GameControls controls;

    //VIEW VECTORS DATA
    //FPV
    private float fpvYaw;
    private float fpvPitch;

    //TPV
    private float tpvYaw;
    private float tpvPitch;


    public Transform ActiveCameraTransform => isFPV ? mainCamera.transform : secondaryCamera.transform;

    private void Awake()
    {
        controls = new GameControls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleCameraToggle();

        if (controls.Ship.EnableLook.IsPressed())
        {
            HandleCameraLook();
        }
        

    }

    private void LateUpdate()
    {
        UpdateCamera();
    }


    private void HandleCameraToggle()
    {
        if (controls.Ship.CameraToggle.WasPressedThisFrame())
        {
            isFPV = !isFPV;
            Debug.Log("CAMERA TOGGLE: " + isFPV);
        }
    }

    private void HandleCameraLook()
    {
        Vector2 look = controls.Ship.Look.ReadValue<Vector2>();

        if (isFPV)
        {
            fpvYaw += look.x * fpvSensitivity;
            fpvPitch -= look.y * fpvSensitivity;

            fpvPitch = Mathf.Clamp(
                fpvPitch, -fpvMaxPitch, fpvMaxPitch
            );
        }
        else
        {
            tpvYaw += look.x * tpvSensitivity;
            tpvPitch -= look.y * tpvSensitivity;

            tpvPitch = Mathf.Clamp(
                tpvPitch, -80f, 80f
            );
        }
    }

    private void UpdateCamera()
    {
        mainCamera.enabled = isFPV;
        secondaryCamera.enabled = !isFPV;

        if (isFPV)
        {
            UpdateFPV();
        }
        else
        {
            UpdateTPV();
        }
    }

    private void UpdateFPV()
    {
        mainCamera.transform.localRotation = Quaternion.Euler(fpvPitch, fpvYaw, 0f);
    }

    private void UpdateTPV()
    {
        Quaternion rotation = Quaternion.Euler(tpvPitch, tpvYaw, 0f);

        Vector3 offset = rotation * new Vector3(0f, tpvHeight, -tpvDistance);

        secondaryCamera.transform.position = transform.position + offset;
        secondaryCamera.transform.LookAt(transform);
    }
}
