using TMPro;
using UnityEngine;

public class SpeedUI : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI velocityText;
    public PlayerMovement playerMovement;
    private string speedString;
    private string velocityString;

    void Update()
    {
        speedString = string.Format("MaxSpeed: {0} m/s", playerMovement.GetMaxSpeed());
        velocityString = string.Format("Velocity: {0} m/s", playerMovement.GetVelocity()); 
        

        speedText.text = speedString;
        velocityText.text = velocityString;
    }
}
