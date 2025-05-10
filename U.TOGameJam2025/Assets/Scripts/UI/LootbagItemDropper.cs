using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LootbagItemDropper : MonoBehaviour, IPointerClickHandler
{
    // --------------------------------------------------
    private Camera _lootbagCamera;
    private RawImage _lootbagTexture;
    // --------------------------------------------------
    private void Start()
    {
        _lootbagCamera = LootbagSystem.Instance.LootbagCamera;
        _lootbagTexture = GetComponent<RawImage>();

        Debug.Log(_lootbagCamera.name + "/" + _lootbagTexture.name);
    }
    // --------------------------------------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _lootbagTexture.rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPos);

        Rect rect = _lootbagTexture.rectTransform.rect;
        float normalizedX = (localPos.x - rect.x) / rect.width;
        float normalizedY = (localPos.y - rect.y) / rect.height;

        Ray ray = _lootbagCamera.ViewportPointToRay(new Vector3(normalizedX, normalizedY, 0));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            InventoryItem item = hit.collider.gameObject.GetComponent<InventoryItem>();
            if (item)
            {
                LootbagSystem.Instance.DropItem(item);
            }
            Debug.Log(hit.collider.gameObject.name + " was clicked.");
        }
    }
}
