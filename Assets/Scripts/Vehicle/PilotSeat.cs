using UnityEngine;
using UnityEngine.InputSystem;

public class PilotSeat : Interactable
{
    [Header("Seat")]
    [SerializeField] private Transform seatPoint;
    [SerializeField] private Transform exitPoint;
    
    [Header("Ship")]
    [SerializeField] private ShipController shipController;

    private PlayerController seatedPlayer;
    private bool isOccupied => seatedPlayer != null;

    public override void Interact()
    {
        if (isOccupied){
            LeaveSeat();
            return;
        }

        PlayerController player = FindAnyObjectByType<PlayerController>();

        if (player == null){
            return;
        }

        SitPlayer(player);

    }

    private void SitPlayer(PlayerController player)
    {
        seatedPlayer = player;
        player.EnterSeat(seatPoint);

        if (shipController != null)
        {
            shipController.SetPilotActive(true);
        }
    }

    private void LeaveSeat()
    {
        if(seatedPlayer == null)
        {
            return;
        }
        seatedPlayer.ExitSeat(exitPoint);
        seatedPlayer = null;

        if(shipController != null)
        {
            shipController.SetPilotActive(false);
        }
    }
}
