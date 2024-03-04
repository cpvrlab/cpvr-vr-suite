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

    public void SwitchToRay()
    {
        if (TryGetInteractorManager<LeftHandManager>(out var leftHandManager))
            leftHandManager.InteractionMode = InteractionMode.Ray;
        else
            Debug.LogWarning("No HandManager for the left hand found.");
        
        if (TryGetInteractorManager<RightHandManager>(out var rightHandManager))
            rightHandManager.InteractionMode = InteractionMode.Ray;
        else
            Debug.LogWarning("No HandManager for the right hand found.");

        if (TryGetInteractorManager<HandMenuManager>(out var handMenuManager))
            handMenuManager.Blocked = true;
        else
            Debug.LogWarning("No HandMenuManager for the right hand found.");
    }

    public void SwitchToTeleport()
    {
        if (TryGetInteractorManager<LeftHandManager>(out var leftHandManager))
            leftHandManager.InteractionMode = InteractionMode.Teleport;
        else
            Debug.LogWarning("No HandManager for the left hand found.");
        
        if (TryGetInteractorManager<RightHandManager>(out var rightHandManager))
            rightHandManager.InteractionMode = InteractionMode.Teleport;
        else
            Debug.LogWarning("No HandManager for the right hand found.");

        if (TryGetInteractorManager<HandMenuManager>(out var handMenuManager))
            handMenuManager.Blocked = false;
        else
            Debug.LogWarning("No HandMenuManager found.");
    }

    public void ToggleHandMenu(bool value)
    {
        if (TryGetInteractorManager<HandMenuManager>(out var handMenuManager) &&
            handMenuManager.Blocked) return;

        if (TryGetInteractorManager<LeftHandManager>(out var leftHandManager))
            leftHandManager.InteractionMode = value ? InteractionMode.Ray : InteractionMode.Teleport;
        else
            Debug.LogWarning("No HandManager for the left hand found.");
        
        if (TryGetInteractorManager<RightHandManager>(out var rightHandManager))
            rightHandManager.InteractionMode = value ? InteractionMode.Ray : InteractionMode.Teleport;
        else
            Debug.LogWarning("No HandManager for the right hand found.");

        if (TryGetInteractorManager<GazeManager>(out var gazeManager))
            gazeManager.BlockInteractor(value);
        else
            Debug.LogWarning("No GazeManager found.");
    }

    public void BlockTeleport(bool value)
    {
        if (TryGetInteractorManager<LeftHandManager>(out var leftHandManager))
            leftHandManager.TeleportBlocked = true;
        else
            Debug.LogWarning("No HandManager for the left hand found.");
        
        if (TryGetInteractorManager<RightHandManager>(out var rightHandManager))
            rightHandManager.TeleportBlocked = true;
        else
            Debug.LogWarning("No HandManager for the right hand found.");
    }
    
    public bool TryGetInteractorManager<T>(out T interactorManager) where T : InteractorManager
    {
        interactorManager = m_interactorManagers.OfType<T>().FirstOrDefault();
        return interactorManager != null;
    }
}
