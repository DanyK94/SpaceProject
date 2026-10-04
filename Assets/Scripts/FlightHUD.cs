using UnityEngine;
using TMPro;

public class FlightHUD : MonoBehaviour
{
    [SerializeField] private ShipController ship;
    [SerializeField] private TMP_Text flightData;

    private void Update()
    {
        Vector3 velocity = ship.LinearVelocity;
        Vector3 angularVelocity = ship.AngularVelocity;

        float speed = velocity.magnitude;

        flightData.text = 
        $"SPD {speed:F1} m/s \n" +
        $"VEL {velocity} \n" + 
        $"ANG {angularVelocity} \n" +
        $"FA {(ship.FlightAssistEnabled ? "ON" : "OFF")}";
    }
}
