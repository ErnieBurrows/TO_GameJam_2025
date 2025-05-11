using System;
using UnityEngine;

public class ExitPlatform : MonoBehaviour
{
    // --------------------------------------------------
    private bool _isPlayerInside = false;
    // --------------------------------------------------
    public static event Action<bool> OnExitPlatform;
    // --------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerObject" && !_isPlayerInside)        // Is Player
        {
            _isPlayerInside = true;
            OnExitPlatform?.Invoke(true);
        }
    }
    // --------------------------------------------------
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PlayerObject" && _isPlayerInside)        // Is Player
        {
            _isPlayerInside = false;
            OnExitPlatform?.Invoke(false);
        }
    }
    // --------------------------------------------------
    public static void StayClicked()
    {
        OnExitPlatform?.Invoke(false);
    }
}
