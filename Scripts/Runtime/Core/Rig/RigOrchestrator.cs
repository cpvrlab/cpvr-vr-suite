using System.Collections.Generic;
using System.Linq;
using Network;
using UnityEngine;
using UnityEngine.XR.Hands.Samples.VisualizerSample;

public class RigOrchestrator : MonoBehaviour
{
    public Transform Origin { get; private set; }

    [field: SerializeField] public GameObject Camera { get; private set; }
    [field: SerializeField] public SkinnedMeshRenderer LeftHand { get; private set; }
    [field: SerializeField] public HandInteractorManager LeftHandInteractorManager { get; private set; }
    [field: SerializeField] public SkinnedMeshRenderer RightHand { get; private set; }
    [field: SerializeField] public HandInteractorManager RightHandInteractorManager { get; private set; }
    [field: SerializeField] public HandVisualizer Visualizer { get; private set; }
    [field: SerializeField] public NetworkTeleportationProvider NetworkTeleportationProvider { get; private set; }

    readonly List<InteractorManager> m_interactorManagers = new();

    void Awake()
    {
        Origin = transform;

        var managers = GetComponentsInChildren<InteractorManager>();
        m_interactorManagers.AddRange(managers);
    }

    public void SwitchToRay()
    {
        if (TryGetInteractorManager<HandMenuManager>(out var handMenuManager))
            handMenuManager.Blocked = true;
        else
            Debug.LogWarning("No HandMenuManager for the right hand found.");
    }

    public void ToggleHandMenu(bool value)
    {
        if (!TryGetInteractorManager<HandMenuManager>(out var handMenuManager) ||
            (handMenuManager != null && handMenuManager.Blocked)) return;

        if (TryGetInteractorManager<GazeManager>(out var gazeManager))
            gazeManager.BlockInteractor(value);
        else
            Debug.LogWarning("No GazeManager found.");
    }
    
    public bool TryGetInteractorManager<T>(out T interactorManager) where T : InteractorManager
    {
        interactorManager = m_interactorManagers.OfType<T>().FirstOrDefault();
        return interactorManager != null;
    }

    public void LockTeleport(bool value)
    {
        LeftHandInteractorManager.BlockTeleport(value);
        RightHandInteractorManager.BlockTeleport(value);
    }
}
