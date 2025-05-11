using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameResultDisplayer : MonoBehaviour
{
    // --------------------------------------------------
    [SerializeField] TextMeshProUGUI _moneyCollectedTMPro;
    [SerializeField] TextMeshProUGUI _elapsedTimeTMPro;
    [SerializeField] TextMeshProUGUI _moneyYouReceiveTMPro;
    // --------------------------------------------------
    [Header("Input")]
    [SerializeField] PlayerInput _playerInput;
    private InputAction _closeAction;
    // --------------------------------------------------
    private void Start()
    {
        _moneyCollectedTMPro.text = $"Money Collected -> ${GameStateManager.MoneyCollected:0.00}";

        float elapsedTime = GameStateManager.ElapsedTime;
        int minute = Mathf.FloorToInt(elapsedTime / 60f);
        int second = Mathf.FloorToInt(elapsedTime % 60f);
        _elapsedTimeTMPro.text = $"Elapsed Time -> {minute:00}:{second:00}";

        _moneyYouReceiveTMPro.text = $"Money You Receive -> ${GameStateManager.MoneyReceived:0.00}";
    }
    // --------------------------------------------------
    private void Close()
    {
        SceneManager.LoadScene("MainMenu");
    }
    // --------------------------------------------------
    private void OnEnable()
    {
        _closeAction = _playerInput.actions.FindActionMap("GameResult").FindAction("Close");

        _closeAction.performed += ctx => Close();
    }
    // --------------------------------------------------
    private void OnDisable()
    {
        _closeAction.performed -= ctx => Close();
    }
    // --------------------------------------------------
}
