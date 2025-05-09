using UnityEngine;
using UnityEngine.Events;

public interface IUsable
{
    public void Use(GameObject actor);
    public UnityEvent OnUse { get;}
}
