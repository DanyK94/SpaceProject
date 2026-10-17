using UnityEditor.Build;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
   
    [Header("Look")]
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float maxPitch = 75f;
    [SerializeField] private float minPitch = -90f;
    [SerializeField] private float maxYaw = 80f;
    [SerializeField] private float minYaw = -80f;

    private GameControls controls;

    private float yaw;
    private float pitch;
    private bool seatedMode;

    public Quaternion AimRotation => transform.rotation;
    public Quaternion LocalAimRotation => transform.localRotation;

    public void Awake()
    {
        controls = new GameControls();
    }
    public void OnEnable()
    {
        controls.Player.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    }

    public void SetSeatedMode(bool seated)
    {
        seatedMode = seated;

        if (seatedMode)
        {
            yaw = 0f;
            pitch = 0f;
            transform.localRotation = Quaternion.identity;
        }
    }

    private void Update()
    {
        if (!seatedMode) return;

        HandleLook();
    }

    private void HandleLook()
    {
        Vector2 look = controls.Player.Look.ReadValue<Vector2>();
        yaw += look.x * sensitivity;
        pitch -= look.y * sensitivity;

        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    public Vector2 GetPointerOffsetPixels()
    {
        if (playerCamera == null) return Vector2.zero;

        float halfHeight = playerCamera.pixelHeight * 0.5f;
        float halfWidth = playerCamera.pixelWidth * 0.5f;

        float verticalHalfFov = playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float horizontalHalfFov = Mathf.Atan( Mathf.Tan( verticalHalfFov ) * playerCamera.aspect );

        float yawRadians = yaw * Mathf.Deg2Rad;
        float pitchRadians = pitch * Mathf.Deg2Rad;

        float x = halfWidth * Mathf.Tan(yawRadians) / Mathf.Tan(horizontalHalfFov);
        float y = -halfHeight * Mathf.Tan(pitchRadians) / Mathf.Tan(verticalHalfFov);

        return new Vector2(x,y);
    }   


    public Vector2 GetAimReferenceScreenPosition(Transform aimReference)
    {
        if (playerCamera == null || aimReference == null)
        {
            return Vector2.zero;
        }
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(aimReference.position);

        if (screenPosition.z <= 0f)
{
            Debug.Log("AimReference dietro la camera");
        }
        
        return new Vector2(screenPosition.x, screenPosition.y);
    }
    public Vector2 GetLookScreenPosition()
    {
        if (playerCamera == null) return Vector2.zero;

        Vector3 lookDirection = transform.forward;
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(playerCamera.transform.position + lookDirection * 100f);
        

        return new Vector2(screenPosition.x, screenPosition.y);
    }
}
