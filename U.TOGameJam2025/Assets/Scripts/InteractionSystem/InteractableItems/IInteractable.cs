using UnityEngine;

public interface IInteractable
{
    bool KeepWorldPosition { get; }

    GameObject PickUp();
}
