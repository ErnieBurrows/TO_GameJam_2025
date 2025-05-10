using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class GameStateManager : MonoBehaviour
{
    #region Singleton
    private static GameStateManager instance;
    public static GameStateManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameStateManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameStateManager");
                    instance = obj.AddComponent<GameStateManager>();
                }
            }
            return instance;
        }
    }
    #endregion
    
    public enum GameState{
        InGame,
        UIPaused,
        UIUnPaused
    };

    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnGameStateManagerInitialized;
    [SerializeField] private PlayerInput playerInput;
    private GameState gameState;
    private InputAction pauseAction;
    private InputAction unPauseAction;

    void Awake()
    {
        pauseAction = playerInput.actions["Pause"];
        unPauseAction = playerInput.actions["UnPause"];
    }

    void Start()
    {
        gameState = GameState.InGame;
        playerInput.SwitchCurrentActionMap("Player");
        
        OnGameStateManagerInitialized?.Invoke();
    }

    void OnEnable()
    {
        pauseAction.performed += ctx => TogglePause(true);
        unPauseAction.performed += ctx => TogglePause(false);

        pauseAction.Enable();
        unPauseAction.Enable();
    }
   
    void OnDisable()
    {
        pauseAction.performed -= ctx => TogglePause(true);
        unPauseAction.performed -= ctx => TogglePause(false);

        pauseAction.Disable();
        unPauseAction.Disable();
    }

    private void TogglePause(bool isPausing)
    {
        if(isPausing)
            HandleGameState(GameState.UIPaused);
        else
            HandleGameState(GameState.InGame);
    }

    private void HandleGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.InGame:
                playerInput.SwitchCurrentActionMap("Player");
                break;
            case GameState.UIPaused:
                playerInput.SwitchCurrentActionMap("UI");
                break;
            case GameState.UIUnPaused:
                // Handle UI unpaused state
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }

        OnGameStateChanged?.Invoke(gameState);
    }
}
