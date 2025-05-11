using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
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
    [SerializeField] GameObject _pauseCanvasPrefab;

    private GameObject _virtualMouseCanvas;
    private GameObject _lootbag;
    private GameObject _mainHudCanvas;
    private GameObject _lootbagCanvas;
    private GameObject _pauseCanvas;

    private RawImage _lootbagTexture;
    private TextMeshProUGUI _itemLabelTMPro;
    private TextMeshProUGUI _lootbagLabelTMPro;
    private TextMeshProUGUI _droppingLootbagLabelTMPro;
    private GameObject _bagItemGuide;

    private RectTransform _virtualCursor;

    public RawImage LootbagTexture => _lootbagTexture;
    // --------------------------------------------------
    // [[VALUES]]
    private Vector2 _defaultVirtualMousePos;
    private bool _isVirtualMouseEnabled;
    private UIStates _currentState = UIStates.NONE;

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
        Debug.Log("<UIHandler> Virtual Mouse.");
        _virtualMouseCanvas = Instantiate(_virtualMouseCanvasPrefab, transform);
        _virtualCursor = _virtualMouseCanvas.transform.Find("VirtualMouseHandler") as RectTransform;
        _defaultVirtualMousePos = _virtualCursor.anchoredPosition;
        VirtualMouse(false);

        // Lootbag System
        Debug.Log("<UIHandler> Lootbag System.");
        _lootbag = Instantiate(_lootbagPrefab, transform);
        LootbagSystem(false);

        // Main HUD
        Debug.Log("<UIHandler> Main HUD.");
        _mainHudCanvas = Instantiate(_mainHudCanvasPrefab,transform);
        _bagItemGuide = _mainHudCanvas.transform.Find("BagItemButtonGuide").gameObject;
        _bagItemGuide.SetActive(false);
        _mainHudCanvas.SetActive(false);

        // Lootbag View
        Debug.Log("<UIHandler> Lootbag Canvas.");
        _lootbagCanvas = Instantiate(_lootbagCanvasPrefab,transform);
        _lootbagTexture = _lootbagCanvas.transform.Find("LootbagTexture").GetComponent<RawImage>();
        _lootbagCanvas.SetActive(false);

        // Pause
        _pauseCanvas = Instantiate(_pauseCanvasPrefab,transform);
        _pauseCanvas.SetActive(false);

        // Get Item Label TMPro
        _itemLabelTMPro = _mainHudCanvas.transform.Find("ItemText").GetComponent<TextMeshProUGUI>();

        // Get Lootbag Label TMPro
        _lootbagLabelTMPro = _mainHudCanvas.transform.Find("LootbagTexture/LootbagLabel").GetComponent<TextMeshProUGUI>();

        // Get Dropping Lootbag Label TMPro
        _droppingLootbagLabelTMPro = _lootbagCanvas.transform.Find("WeightMoneyLabel").GetComponent<TextMeshProUGUI>();

        OnInventoryChanged();

        GameStart();
    }
    // --------------------------------------------------
    public void GameStart()
    {
        Debug.Log("<UIHandler> Game Start.");
        LootbagSystem(true);
        SwitchState(UIStates.MAIN_HUD);
    }
    // --------------------------------------------------
    public void SwitchState(UIStates state)
    {
        if (state == _currentState) return;

        // Disable all first
        _mainHudCanvas.SetActive(false);
        _lootbagCanvas.SetActive(false);
        _pauseCanvas.SetActive(false);

        switch (state)
        {
            case UIStates.MAIN_HUD:
                Debug.Log("<UIHandler> Switched to Main HUD.");
                _currentState = UIStates.MAIN_HUD;
                VirtualMouse(false);
                _mainHudCanvas.SetActive(true);
                break;
            case UIStates.LOOTBAG:
                Debug.Log("<UIHandler> Switched to Lootbag.");
                _currentState = UIStates.LOOTBAG;
                VirtualMouse(true);
                _lootbagCanvas.SetActive(true);
                break;
            case UIStates.GAME_PAUSED:
                Debug.Log("<UIHandler> Switched to Paused.");
                _currentState = UIStates.GAME_PAUSED;
                VirtualMouse(true);
                _pauseCanvas.SetActive(true);
                break;
        }
    }
    // --------------------------------------------------
    public void VirtualMouse(bool value)
    {
        _isVirtualMouseEnabled = value;
        _virtualMouseCanvas.SetActive(value);

        GameObject virtualMouseImage = _virtualCursor.transform.Find("Cursor").gameObject;
        virtualMouseImage.SetActive(_isVirtualMouseEnabled);

        if (_isVirtualMouseEnabled)
        {
            var currentPos = Mouse.current.position.ReadValue();
            var newPos = currentPos + new Vector2(0.1f, 0f); // tiny nudge

            InputSystem.QueueDeltaStateEvent(Mouse.current.position, newPos);
            // InputSystem.Update(); // apply the state
        }

        // VirtualMouseInput virtualMouse = _virtualMouseCanvas.transform.Find("VirtualMouseHandler").GetComponent<VirtualMouseInput>();
        // 
        // virtualMouse.enabled = _isVirtualMouseEnabled;
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
            case GameStateManager.GameState.PAUSED:
                SwitchState(UIStates.GAME_PAUSED);
                break;
        }
    }
    // --------------------------------------------------

    private void ToggleItemLabel(GameObject gameObject, bool isActive)
    {
        if (gameObject.GetComponent<InventoryItem>())
        {
            LootItem item = gameObject.GetComponent<LootItem>();
            _itemLabelTMPro.text = $"<b>{item.ItemName}</b>\n<size=25><color=#ffff00>(Valuable - ${item.Value})</color></size>";
        }
        else if (gameObject.GetComponent<Weapon>())
        {
            _itemLabelTMPro.text = $"<b>{gameObject.name}</b>\n<size=25>(Weapon)</size>";
        }

        _itemLabelTMPro.enabled = isActive;
    }
    // --------------------------------------------------
    private void OnInventoryChanged()
    {
        float currentMoney = PlayerInventory.Instance.currentMoney;
        float currentWeight = PlayerInventory.Instance.currentWeight;
        float maxWeight = PlayerInventory.Instance.maxWeight;
        _lootbagLabelTMPro.text = $"{currentWeight}/{maxWeight} lbs\n<color=#ffff00>${currentMoney}</color>";

        _droppingLootbagLabelTMPro.text = $"{currentWeight}/{maxWeight} lbs - <color=#ffff00>${currentMoney}</color>";

        _bagItemGuide.SetActive(false);
    }
    // --------------------------------------------------
    private void OnItemPickedUp(GameObject itemObject)
    {
        Debug.Log("Item Picked Up");
        if (itemObject.GetComponent<InventoryItem>())
        {
            Debug.Log("Item is Inventory Item");
            _bagItemGuide.SetActive(true);
        }
    }
    // --------------------------------------------------
    private void OnItemDropped(GameObject itemObject)
    {
        Debug.Log("Item Dropped");
        _bagItemGuide.SetActive(false);
    }
    // --------------------------------------------------
    private void OnEnable()
    {
        GameStateManager.OnGameStateManagerInitialized += Setup;
        GameStateManager.OnGameStateChanged += GameStateChanged;
        InteractorComponent.OnInteractableObjectHovered += ToggleItemLabel;
        InventoryItem.OnInventoryChanged += OnInventoryChanged;
        InteractorComponent.OnInteractableObjectPickedUp += OnItemPickedUp;
        InteractorComponent.OnInteractableObjectDropped += OnItemDropped;
    }

    // -----------------.-----------------------
    private void OnDisable()
    {
        GameStateManager.OnGameStateManagerInitialized -= Setup;
        GameStateManager.OnGameStateChanged -= GameStateChanged;
        InteractorComponent.OnInteractableObjectHovered -= ToggleItemLabel;
        InventoryItem.OnInventoryChanged -= OnInventoryChanged;
        InteractorComponent.OnInteractableObjectPickedUp -= OnItemPickedUp;
        InteractorComponent.OnInteractableObjectDropped -= OnItemDropped;
    }
    // --------------------------------------------------
}