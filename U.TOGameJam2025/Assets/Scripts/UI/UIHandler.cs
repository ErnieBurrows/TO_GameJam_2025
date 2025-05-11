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
    [SerializeField] InputActionAsset _inputActionAsset;

    private GameObject _virtualMouseCanvas;
    private GameObject _lootbag;
    private GameObject _mainHudCanvas;
    private GameObject _lootbagCanvas;
    private RawImage _lootbagTexture;
    private TextMeshProUGUI _itemLabelTMPro;
    private TextMeshProUGUI _lootbagLabelTMPro;
    private TextMeshProUGUI _droppingLootbagLabelTMPro;

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
    private void Start()
    {
        var virtualMouse = InputSystem.GetDevice<Mouse>("VirtualMouse");
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("Mouse");
        }

        InputUser user = InputUser.CreateUserWithoutPairedDevices();
        InputUser.PerformPairingWithDevice(virtualMouse, user);
        user.AssociateActionsWithUser(_inputActionAsset);

        InputSystem.QueueStateEvent(virtualMouse, new MouseState
        {
            position = new Vector2(Screen.width / 2, Screen.height / 2)
        });

        InputSystem.Update();
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
        _mainHudCanvas.SetActive(false);

        // Lootbag View
        Debug.Log("<UIHandler> Lootbag Canvas.");
        _lootbagCanvas = Instantiate(_lootbagCanvasPrefab,transform);
        _lootbagTexture = _lootbagCanvas.transform.Find("LootbagTexture").GetComponent<RawImage>();
        _lootbagCanvas.SetActive(false);

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
        // Disable all first
        _mainHudCanvas.SetActive(false);
        _lootbagCanvas.SetActive(false);

        switch (state)
        {
            case UIStates.MAIN_HUD:
                Debug.Log("<UIHandler> Switched to Main HUD.");
                VirtualMouse(false);
                _mainHudCanvas.SetActive(true);
                break;
            case UIStates.LOOTBAG:
                Debug.Log("<UIHandler> Switched to Lootbag.");
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

        GameObject virtualMouseImage = _virtualCursor.transform.Find("Cursor").gameObject;
        virtualMouseImage.SetActive(_isVirtualMouseEnabled);

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
        _lootbagLabelTMPro.text = $"?/?\n<color=#ffff00>${currentMoney}</color>";

        _droppingLootbagLabelTMPro.text = $"?/? - <color=#ffff00>${currentMoney}</color>";
    }
    // --------------------------------------------------
    private void OnEnable()
    {
        GameStateManager.OnGameStateManagerInitialized += Setup;
        GameStateManager.OnGameStateChanged += GameStateChanged;
        InteractorComponent.OnInteractableObjectHovered += ToggleItemLabel;
        InventoryItem.OnInventoryChanged += OnInventoryChanged;
    }

    // -----------------.-----------------------
    private void OnDisable()
    {
        GameStateManager.OnGameStateManagerInitialized -= Setup;
        GameStateManager.OnGameStateChanged -= GameStateChanged;
        InteractorComponent.OnInteractableObjectHovered -= ToggleItemLabel;
        InventoryItem.OnInventoryChanged -= OnInventoryChanged;
    }
    // --------------------------------------------------
}