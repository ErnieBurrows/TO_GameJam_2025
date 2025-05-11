using System;
using System.Collections;
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
    private PlayerInput playerInput;
    private InputAction pauseAction;
    private InputAction unPauseAction;
    private InputAction lootBagOpenAction;
    private InputAction lootBagCloseAction;
    private GameState currentGameState;

    public static event Action<float> OnStopwatchTicked;

    private static float _moneyCollected;
    private static float _elapsedTime;
    private static float _moneyReceived;
    private static bool _stopwatchPaused = false;

    [Header("Game Settings")]
    [SerializeField, Range(0f, 1f)] float _moneyLostPerSeconds = 0.5f;
    

    void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        
        pauseAction = playerInput.actions["Pause"];
        unPauseAction = playerInput.actions["UnPause"];

        lootBagOpenAction = playerInput.actions["LootBagOpen"];
        lootBagCloseAction = playerInput.actions["LootBagClose"];
    }

    void Start()
    {
        currentGameState = GameState.INGAME;
        playerInput.SwitchCurrentActionMap("Player");

        GameStart();
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
                ContinueStopwatch();
                break;

            case GameState.PAUSED:
                playerInput.SwitchCurrentActionMap("UI");
                currentGameState = GameState.PAUSED;
                PauseStopwatch();
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

    private void GameStart()
    {
        OnGameStateManagerInitialized?.Invoke();

        StartStopwatch();
    }

    private void GameEnd()
    {
        StopStopwatch();

        _moneyCollected = SellPlatform.Instance.CurrentValue;
        _moneyReceived = _moneyCollected - (_elapsedTime * _moneyLostPerSeconds);
    }
    
    public void StartStopwatch()
    {
        _elapsedTime = 0f;
        StartCoroutine(StopwatchCoroutine());
    }

    public void StopStopwatch()
    {
        StopCoroutine(StopwatchCoroutine());
    }

    public void PauseStopwatch()
    {
        _stopwatchPaused = true;
    }

    public void ContinueStopwatch()
    {
        _stopwatchPaused = false;
    }

    private IEnumerator StopwatchCoroutine()
    {
        yield return new WaitUntil(() => !_stopwatchPaused); // Stop when stopwatch is paused

        _elapsedTime += Time.deltaTime;
        OnStopwatchTicked?.Invoke(_elapsedTime);

        StartCoroutine(StopwatchCoroutine());
    }
    
    public enum GameState{
        INGAME,
        PAUSED,
        LOOTBAG
    };
}
