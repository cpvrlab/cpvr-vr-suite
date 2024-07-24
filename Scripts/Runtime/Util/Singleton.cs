using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static event Action InstanceReady;
    public static T Instance { get; private set; }
    [SerializeField] private bool _isPersistent;

    protected virtual void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
        {
            Instance = this as T;
            if (_isPersistent)
                DontDestroyOnLoad(this);
            InstanceReady?.Invoke();
        }
    }

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
