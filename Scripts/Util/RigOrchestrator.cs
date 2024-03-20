using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RigOrchestrator : MonoBehaviour
{
    public GameObject Camera { get; private set; }
    readonly List<InteractorManager> m_interactorManagers = new();

    void Awake()
    {
        Camera = GetComponentInChildren<Camera>().gameObject;
        
        var managers = GetComponentsInChildren<InteractorManager>();
        m_interactorManagers.AddRange(managers);
    }

    public void SwitchToRay()
    {
        if (TryGetInteractorManager<HandManager>(out var handManager))
            handManager.InteractionMode = InteractionMode.Ray;
        else
            Debug.Log("HandManager not found.");

        if (TryGetInteractorManager<HandMenuManager>(out var handMenuManager))
            handMenuManager.Blocked = true;
        else
            Debug.LogWarning("No HandMenuManager for the right hand found.");
    }

    public void SwitchToTeleport()
    {
        if (TryGetInteractorManager<HandManager>(out var handManager))
            handManager.InteractionMode = InteractionMode.Teleport;
        else
            Debug.Log("HandManager not found.");

        if (TryGetInteractorManager<HandMenuManager>(out var handMenuManager))
            handMenuManager.Blocked = false;
        else
            Debug.LogWarning("No HandMenuManager found.");
    }

    public void ToggleHandMenu(bool value)
    {
        if (!TryGetInteractorManager<HandMenuManager>(out var handMenuManager) ||
            (handMenuManager != null && handMenuManager.Blocked)) return;

        if (TryGetInteractorManager<HandManager>(out var handManager))
            handManager.ToggleHandMenu(value);
        else
            Debug.Log("HandManager not found.");

        if (TryGetInteractorManager<GazeManager>(out var gazeManager))
            gazeManager.BlockInteractor(value);
        else
            Debug.LogWarning("No GazeManager found.");
    }

    public void BlockTeleport(bool value)
    {
        if (TryGetInteractorManager<HandManager>(out var handManager))
            handManager.TeleportBlocked = value;
        else
            Debug.LogWarning("No HandManager found.");
    }
    
    public void BlockInteraction(bool value)
    {
        if (TryGetInteractorManager<HandManager>(out var handManager))
        {
            handManager.LeftInteractionBlocked = value;
            handManager.RightInteractionBlocked = value;
        }
        else
            Debug.LogWarning("No HandManager found.");
    }
    
    public bool TryGetInteractorManager<T>(out T interactorManager) where T : InteractorManager
    {
        interactorManager = m_interactorManagers.OfType<T>().FirstOrDefault();
        return interactorManager != null;
    }
}
