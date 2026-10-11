using UnityEngine;

public class SlidingDoor : Interactable
{
    [Header("Door Panels")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Movement")]
    [SerializeField] private Vector3 slideAxis = Vector3.right;
    [SerializeField] private float slideDistance = 1f;
    [SerializeField] private float slideSpeed =  2f;

    private Vector3 leftClosedPosition;
    private Vector3 rightClosedPosition;

    private bool isOpen;

    private void Awake()
    {
        leftClosedPosition = leftDoor.localPosition;
        rightClosedPosition = rightDoor.localPosition;

        slideAxis = slideAxis.normalized;
    }

    public override void Interact()
    {
        isOpen = !isOpen;
    }

    private void Update()
    {
        Vector3 leftTarget = leftClosedPosition;
        Vector3 rightTarget = rightClosedPosition;

        if (isOpen)
        {
            leftTarget -= slideAxis * slideDistance;
            rightTarget += slideAxis * slideDistance;
        }

        leftDoor.localPosition = Vector3.MoveTowards(leftDoor.localPosition, leftTarget, slideSpeed * Time.deltaTime);
        rightDoor.localPosition = Vector3.MoveTowards(rightDoor.localPosition, rightTarget, slideSpeed * Time.deltaTime);
    }



}
