using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectClickTest : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Camera _lootbagCamera;
    [SerializeField] RawImage _lootbagTexture;

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
            Debug.Log(hit.collider.gameObject.name + " was clicked.");
        }
    }
}
