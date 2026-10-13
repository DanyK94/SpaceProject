using UnityEngine;
using UnityEngine.InputSystem;

public class PilotSeat : Interactable
{
    [Header("Seat")]
    [SerializeField] private Transform seatPoint;
    [SerializeField] private Transform exitPoint;

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
    }

    private void LeaveSeat()
    {
        if(seatedPlayer == null)
        {
            return;
        }
        seatedPlayer.ExitSeat(exitPoint);
        seatedPlayer = null;
    }
}
