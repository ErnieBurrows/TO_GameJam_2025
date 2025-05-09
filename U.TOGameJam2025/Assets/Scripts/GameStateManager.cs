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
    private InputAction lootBagOpenAction;
    private InputAction lootBagCloseAction;
    private GameState currentGameState;

    void Awake()
    {
        pauseAction = playerInput.actions["Pause"];
        unPauseAction = playerInput.actions["UnPause"];

        lootBagOpenAction = playerInput.actions["LootBagOpen"];
        lootBagCloseAction = playerInput.actions["LootBagClose"];
    }

    void Start()
    {
        currentGameState = GameState.INGAME;
        playerInput.SwitchCurrentActionMap("Player");

        OnGameStateManagerInitialized?.Invoke();
        Debug.Log("GameStateManager Initialized");
    }

    void OnEnable()
    {
        pauseAction.performed += ctx => TogglePause(true);
        unPauseAction.performed += ctx => TogglePause(false);
        lootBagOpenAction.performed += ctx => ToggleLootBag(true);
        lootBagCloseAction.performed += ctx => ToggleLootBag(false);
        

        pauseAction.Enable();
        unPauseAction.Enable();
        lootBagOpenAction.Enable();
        lootBagCloseAction.Enable();
    }

    void OnDisable()
    {
        pauseAction.performed -= ctx => TogglePause(true);
        unPauseAction.performed -= ctx => TogglePause(false);
        lootBagOpenAction.performed -= ctx => ToggleLootBag(true);
        lootBagCloseAction.performed -= ctx => ToggleLootBag(false);

        pauseAction.Disable();
        unPauseAction.Disable();
        lootBagOpenAction.Disable();
        lootBagCloseAction.Disable();
    }

    private void TogglePause(bool isPausing)
    {
        if(isPausing)
            HandleGameState(GameState.PAUSED);
        else
            HandleGameState(GameState.INGAME);
    }
    private void ToggleLootBag(bool isOpening)
    {
        if(isOpening)
            HandleGameState(GameState.LOOTBAG);
        else
            HandleGameState(GameState.INGAME);
    }

    private void HandleGameState(GameState stateToSwapTo)
    {
        switch (stateToSwapTo)
        {
            case GameState.INGAME:
                playerInput.SwitchCurrentActionMap("Player");
                currentGameState = GameState.INGAME;
                break;

            case GameState.PAUSED:
                playerInput.SwitchCurrentActionMap("UI");
                currentGameState = GameState.PAUSED;
                break;

            case GameState.LOOTBAG:
                playerInput.SwitchCurrentActionMap("UI");
                currentGameState = GameState.LOOTBAG;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(stateToSwapTo), stateToSwapTo, null);
        }

        OnGameStateChanged?.Invoke(stateToSwapTo);
    }

    
    public enum GameState{
        INGAME,
        PAUSED,
        LOOTBAG
    };
}
