using System;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    // --------------------------------------------------
    // [[SINGLETON]]
    public static UIHandler Instance;
    // --------------------------------------------------
    // [[REFERENCES]]
    [SerializeField] GameObject _virtualMouseCanvasPrefab;
    [SerializeField] GameObject _lootbagPrefab;
    [SerializeField] GameObject _mainHudCanvasPrefab;
    [SerializeField] GameObject _lootbagCanvasPrefab;

    private GameObject _virtualMouseCanvas;
    private GameObject _lootbag;
    private GameObject _mainHudCanvas;
    private GameObject _lootbagCanvas;
    private RawImage _lootbagTexture;

    private RectTransform _virtualCursor;

    public RawImage LootbagTexture => _lootbagTexture;
    // --------------------------------------------------
    // [[VALUES]]
    private Vector2 _defaultVirtualMousePos;
    private bool _isVirtualMouseEnabled;
    private UIStates _currentState;

    public bool IsVirtualMouseEnabled => _isVirtualMouseEnabled;
    public UIStates CurrentState => _currentState;
    // --------------------------------------------------
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }
    // --------------------------------------------------
    private void Setup()
    {
        Debug.Log("<UIHandler> Setting up.");

        // Virtual Mouse
        _virtualMouseCanvas = Instantiate(_virtualMouseCanvasPrefab, transform);
        _virtualCursor = _virtualMouseCanvas.transform.Find("VirtualMouseHandler") as RectTransform;
        _defaultVirtualMousePos = _virtualCursor.anchoredPosition;
        _virtualMouseCanvas.SetActive(false);

        // Lootbag System
        _lootbag = Instantiate(_lootbagPrefab, transform);
        LootbagSystem(false);

        // Main HUD
        _mainHudCanvas = Instantiate(_mainHudCanvasPrefab,transform);
        _mainHudCanvas.SetActive(false);

        // Lootbag View
        _lootbagCanvas = Instantiate(_lootbagCanvasPrefab,transform);
        _lootbagTexture = _lootbagCanvas.transform.Find("LootbagTexture").GetComponent<RawImage>();
        _lootbagCanvas.SetActive(false);

        GameStart();
    }
    // --------------------------------------------------
    public void GameStart()
    {
        LootbagSystem(true);
        SwitchState(UIStates.MAIN_HUD);
    }
    // --------------------------------------------------
    public void SwitchState(UIStates state)
    {
        // Disable all first
        _mainHudCanvas.SetActive(false);
        _lootbagCanvas.SetActive(false);

        switch (state)
        {
            case UIStates.MAIN_HUD:
                VirtualMouse(false);
                _mainHudCanvas.SetActive(true);
                break;
            case UIStates.LOOTBAG:
                VirtualMouse(true);
                _lootbagCanvas.SetActive(true);
                break;
        }
    }
    // --------------------------------------------------
    public void VirtualMouse(bool value)
    {
        _isVirtualMouseEnabled = value;
        _virtualMouseCanvas.SetActive(value);

        if (_isVirtualMouseEnabled)
        {
            // _virtualCursor.anchoredPosition = _defaultVirtualMousePos;
            // _virtualCursor.GetComponent<VirtualMouseInput>().cursorTransform.position = _defaultVirtualMousePos;
        }
    }
    // --------------------------------------------------
    public void LootbagSystem(bool value)
    {
        _lootbag.SetActive(value);
    }
    // --------------------------------------------------
    private void GameStateChanged(GameStateManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameStateManager.GameState.LOOTBAG:
                SwitchState(UIStates.LOOTBAG);
                break;
            case GameStateManager.GameState.INGAME:
                SwitchState(UIStates.MAIN_HUD);
                break;
        }
    }
    // --------------------------------------------------

    private void ToggleItemLabel(GameObject gameObject, bool isActive)
    {
        Debug.Log($"<UIHandler> Toggling item label for {gameObject.name} to {isActive}");
    }
    // --------------------------------------------------
    private void OnEnable()
    {
        GameStateManager.OnGameStateManagerInitialized += Setup;
        GameStateManager.OnGameStateChanged += GameStateChanged;
        InteractorComponent.OnInteractableObjectHovered += ToggleItemLabel;
    }

    // -----------------.-----------------------
    private void OnDisable()
    {
        GameStateManager.OnGameStateManagerInitialized -= Setup;
        GameStateManager.OnGameStateChanged -= GameStateChanged;
        InteractorComponent.OnInteractableObjectHovered -= ToggleItemLabel;
    }
    // --------------------------------------------------
}