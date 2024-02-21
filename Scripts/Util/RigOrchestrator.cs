using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RigOrchestrator : MonoBehaviour
{
    readonly List<InteractorManager> m_interactorManagers = new();

    void Awake()
    {
        var managers = GetComponentsInChildren<InteractorManager>();
        m_interactorManagers.AddRange(managers);
    }

    public bool TryGetInteractorManager<T>(out T interactorManager) where T : InteractorManager
    {
        interactorManager = m_interactorManagers.OfType<T>().FirstOrDefault();
        return interactorManager != null;
    }
}
