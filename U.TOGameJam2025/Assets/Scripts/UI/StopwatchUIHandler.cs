using TMPro;
using UnityEngine;

public class StopwatchUIHandler : MonoBehaviour
{
    // --------------------------------------------------
    [SerializeField] TextMeshProUGUI _elapsedTimeTMPro;
    // --------------------------------------------------
    private void Initialize()
    {
        Debug.Log("<StopwatchUI> Initialize.");
        _elapsedTimeTMPro.text = "00:00";
    }
    // --------------------------------------------------
    private void StopwatchTicked(float elapsedTime)
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        _elapsedTimeTMPro.text = $"{minutes:00}:{seconds:00}";
    }
    // --------------------------------------------------
    private void OnEnable()
    {
        GameStateManager.OnGameStateManagerInitialized += Initialize;
        GameStateManager.OnStopwatchTicked += StopwatchTicked;
    }
    // --------------------------------------------------
    private void OnDisable()
    {
        GameStateManager.OnGameStateManagerInitialized -= Initialize;
        GameStateManager.OnStopwatchTicked -= StopwatchTicked;
    }
}
