using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[RequireComponent(typeof(PlaySoundOnUse))]
public class InventoryItem : MonoBehaviour, IUsable, IPointerClickHandler
{
    [field: SerializeField] public UnityEvent OnUse {get; private set;}

    private Camera _lootbagCamera;
    private RawImage _lootbagTexture;
    private ItemState _itemState = ItemState.DROPPED;

    public void Use(GameObject actor)
    {
        foreach (var component in FindObjectsByType<InteractorComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            component.DropItem(gameObject);                                 // Call the DropItem method on all InteractorComponents in the scene
        }
        OnUse?.Invoke();

        LootbagSystem.Instance.BagItem(this);

        Debug.Log("Using item: " + gameObject.name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked.");
        if (_itemState == ItemState.IN_BAG)
        {
            Debug.Log("Clicked.");
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
                LootbagSystem.Instance.DropItem(this);
            }
        }
    }

    public void Dropped()
    {
        Debug.Log(gameObject.name + " was dropped.");
        _itemState = ItemState.DROPPED;
    }

    public void Collected()
    {
        Debug.Log(gameObject.name + " was picked up.");
        _itemState = ItemState.IN_BAG;

        if (_lootbagCamera == null)
        {
            Debug.Log("Loot Camera and Texture assigned.");
            _lootbagTexture = UIHandler.Instance.LootbagTexture;
            _lootbagCamera = LootbagSystem.Instance.LootbagCamera;
        }
    }

    private enum ItemState
    {
        DROPPED,
        IN_BAG
    }
}
