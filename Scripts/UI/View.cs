using UnityEngine;

public abstract class View : MonoBehaviour
{
    public Controller Controller { get; protected set; }
    public abstract void Initialize();
}
