using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private GameState gameState;
    [SerializeField] private PlayerInput playerInput;
    private InputAction pauseAction;

    void Awake()
    {
        pauseAction = playerInput.actions["Pause"];
        pauseAction.performed += ctx => TogglePause();
    }

    private void TogglePause()
    {
        HandleGameState(GameState.UIPaused);
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
                throw new System.ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }

    public void SetGameState(GameState gameState)
    {
        HandleGameState(gameState);
    }

    public GameState GetGameState()
    {
        return gameState;
    }
}
