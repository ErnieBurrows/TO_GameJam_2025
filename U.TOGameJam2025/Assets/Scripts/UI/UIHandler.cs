using UnityEngine;
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

    private RectTransform _virtualCursor;
    // --------------------------------------------------
    // [[VALUES]]
    private Vector2 _defaultVirtualMousePos;
    private bool _isVirtualMouseEnabled;

    public bool IsVirtualMouseEnabled => _isVirtualMouseEnabled;
    // --------------------------------------------------
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;

        Setup();

        GameStart();
    }
    // --------------------------------------------------
    private void Setup()
    {
        // Virtual Mouse
        _virtualMouseCanvas = Instantiate(_virtualMouseCanvasPrefab, transform);
        _virtualCursor = _virtualMouseCanvas.transform.Find("VirtualMouseHandler/Cursor") as RectTransform;
        _defaultVirtualMousePos = _virtualCursor.anchoredPosition;
        _virtualMouseCanvas.SetActive(false);

        // Lootbag
        _lootbag = Instantiate(_lootbagPrefab, transform);
        _lootbag.SetActive(false);

        // Main HUD
        _mainHudCanvas = Instantiate(_mainHudCanvasPrefab,transform);
        _mainHudCanvas.SetActive(false);
        Debug.Log(_mainHudCanvas == null ? "mainhud null" : "mainhud not null");

        // Lootbag
        _lootbagCanvas = Instantiate(_lootbagCanvasPrefab,transform);
        _lootbagCanvas.SetActive(false);
    }
    // --------------------------------------------------
    public void GameStart()
    {
        LootbagSystem(true);
        SwitchState(UIStates.MAIN_HUD);
    }
    // --------------------------------------------------
    // --------------------------------------------------
    public void SwitchState(UIStates state)
    {
        // Disable all first
        Debug.Log(_mainHudCanvas == null ? "mainhud null" : "mainhud not null");
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
            case UIStates.SELL_THE_LOOT:
                break;
        }
    }
    // --------------------------------------------------
    public void VirtualMouse(bool value)
    {
        _isVirtualMouseEnabled = value;
        _virtualMouseCanvas.SetActive(value);

        if (_isVirtualMouseEnabled)
            _virtualCursor.anchoredPosition = _defaultVirtualMousePos;
    }
    // --------------------------------------------------
    public void LootbagSystem(bool value)
    {
        _lootbag.SetActive(value);
    }
    // --------------------------------------------------
}
