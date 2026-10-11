using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance =  3f;

    [Header("HUD")]
    [SerializeField] private GameObject interactionIndicator;

    private GameControls controls;
    private Interactable currentInteractable;

    private void Awake()
    {
        controls = new GameControls();
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
        DetectInteractable();

        if (controls.Player.Interact.WasPressedThisFrame())
        {
            TryInteract();
        }
    }

    private void DetectInteractable()
    {
        currentInteractable = null;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            currentInteractable = hit.collider.GetComponentInParent<Interactable>();
        }

        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(currentInteractable != null);
        }

    }

    private void TryInteract()
    {
        if (currentInteractable == null)
        {
            return;
        }
        currentInteractable.Interact();
    }

}
