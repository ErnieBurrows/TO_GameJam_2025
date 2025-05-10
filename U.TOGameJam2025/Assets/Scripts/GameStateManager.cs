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

    #region Events
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnGameStateManagerInitialized;
    #endregion

    [Header("Input Actions")]
    [SerializeField] private PlayerInput playerInput;
    private InputAction pauseAction;
    private InputAction unPauseAction;
    private GameState currentGameState;

    void Awake()
    {
        pauseAction = playerInput.actions["Pause"];
        unPauseAction = playerInput.actions["UnPause"];
    }

    void Start()
    {
        currentGameState = GameState.InGame;
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

    private void HandleGameState(GameState stateToSwapTo)
    {
        switch (stateToSwapTo)
        {
            case GameState.InGame:
                playerInput.SwitchCurrentActionMap("Player");
                currentGameState = GameState.InGame;
                break;

            case GameState.UIPaused:
                playerInput.SwitchCurrentActionMap("UI");
                currentGameState = GameState.UIPaused;
                break;

            case GameState.UIUnPaused:
                playerInput.SwitchCurrentActionMap("UI");
                currentGameState = GameState.UIUnPaused;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(stateToSwapTo), stateToSwapTo, null);
        }

        OnGameStateChanged?.Invoke(stateToSwapTo);
    }

    
    public enum GameState{
        InGame,
        UIPaused,
        UIUnPaused
    };
}
