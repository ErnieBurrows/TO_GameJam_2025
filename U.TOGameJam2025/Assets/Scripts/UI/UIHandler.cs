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
    [SerializeField] GameObject _dropLootCanvasPrefab;

    private GameObject _virtualMouseCanvas;
    private GameObject _lootbag;
    private GameObject _mainHudCanvas;
    private GameObject _dropLootCanvas;

    private RectTransform _virtualCursor;
    // --------------------------------------------------
    // [[VALUES]]
    private Vector2 _defaultVirtualMousePos;
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

        // Drop Loot
        _dropLootCanvas = Instantiate(_dropLootCanvasPrefab,transform);
        _dropLootCanvas.SetActive(false);
    }
    // --------------------------------------------------
    public void GameStart()
    {
        Lootbag(true);
        SwitchState(UIStates.MAIN_HUD);
    }
    // --------------------------------------------------
    public void SwitchState(UIStates state)
    {
        // Disable all first
        Debug.Log(_mainHudCanvas == null ? "mainhud null" : "mainhud not null");
        _mainHudCanvas.SetActive(false);
        _dropLootCanvas.SetActive(false);

        switch (state)
        {
            case UIStates.MAIN_HUD:
                VirtualMouse(false);
                _mainHudCanvas.SetActive(true);
                break;
            case UIStates.BAG_THE_LOOT:
                break;
            case UIStates.DROP_THE_LOOT:
                VirtualMouse(true);
                _dropLootCanvas.SetActive(true);
                break;
            case UIStates.SELL_THE_LOOT:
                break;
        }
    }
    // --------------------------------------------------
    public void VirtualMouse(bool value)
    {
        _virtualMouseCanvas.SetActive(value);

        if (_virtualMouseCanvas)
            _virtualCursor.anchoredPosition = _defaultVirtualMousePos;
    }
    // --------------------------------------------------
    public void Lootbag(bool value)
    {
        _lootbag.SetActive(value);
    }// --------------------------------------------------
}
