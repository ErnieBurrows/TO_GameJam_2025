using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlaySoundOnUse))]
public class Weapon : MonoBehaviour, IUsable
{
    [field: SerializeField] public UnityEvent OnUse {get; private set;}

    public void Use(GameObject actor)
    {
        OnUse?.Invoke();
        Debug.Log("Using weapon: " + gameObject.name);
    }

}
