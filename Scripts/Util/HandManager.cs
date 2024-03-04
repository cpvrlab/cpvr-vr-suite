using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public enum InteractionMode
{
    Ray,
    Teleport,
    None
}

[DefaultExecutionOrder(k_UpdateOrder)]
public class HandManager : InteractorManager
{
    static InteractionMode m_interactionMode = InteractionMode.Teleport;
    public static InteractionMode InteractionMode
    {
        get => m_interactionMode;
        set => m_interactionMode = value;
    }

    [Header("Interactors")]
    [SerializeField] XRRayInteractor m_rayInteractor;
    [SerializeField] XRRayInteractor m_teleportInteractor;
    bool m_teleportBlocked;
    public bool TeleportBlocked 
    { 
        get => m_teleportBlocked;
        set
        {
            m_teleportBlocked = value;
            if (!value && m_teleportInteractor != null)
                m_teleportInteractor.gameObject.SetActive(false);
            Debug.Log($"Teleport {(value ? "blocked" : "unblocked")}.");
        }
    }
    bool m_rayBlocked;
    public bool RayBlocked 
    { 
        get => m_rayBlocked;
        set
        {
            m_rayBlocked = value;
            if (!value && m_rayInteractor != null)
                m_rayInteractor.gameObject.SetActive(false);
        }
    }

    [Header("Hand Actions")]
    [SerializeField] InputActionReference m_interactModeActivate;
    [SerializeField] InputActionReference m_interactModeCancel;
    [SerializeField] InputActionReference m_switchInteractionMode;

    [Header("Interaction Mode Changed Event")]
    [SerializeField] UnityEvent<InteractionMode> m_interactionModeChanged;

    public const int k_UpdateOrder = XRInteractionUpdateOrder.k_Controllers - 1;
    IEnumerator m_postInteractionEventsRoutine;
    bool m_postponedDeactivateInteract;

    void SetupInteractorEvents()
    {
        var interactModeActivateAction = GetInputAction(m_interactModeActivate);
        if (interactModeActivateAction != null)
        {
            interactModeActivateAction.performed += OnStartInteraction;
            interactModeActivateAction.canceled += OnCancelInteraction;
        }

        var interactionModeCancelAction = GetInputAction(m_interactModeCancel);
        if (interactionModeCancelAction != null)
        {
            interactionModeCancelAction.performed += OnCancelInteraction;
        }

        var switchInteractionModeActivateAction = GetInputAction(m_switchInteractionMode);
        if (switchInteractionModeActivateAction != null)
        {
            switchInteractionModeActivateAction.performed += OnSwitchInteractionMode;
        }
    }

    void TeardownInteractorEvents()
    {
        var interactModeActivateAction = GetInputAction(m_interactModeActivate);
        if (interactModeActivateAction != null)
        {
            interactModeActivateAction.performed -= OnStartInteraction;
            interactModeActivateAction.canceled -= OnCancelInteraction;
        }

        var interactionModeCancelAction = GetInputAction(m_interactModeCancel);
        if (interactionModeCancelAction != null)
        {
            interactionModeCancelAction.performed -= OnCancelInteraction;
        }

        var switchInteractionModeActivateAction = GetInputAction(m_switchInteractionMode);
        if (switchInteractionModeActivateAction != null)
        {
            switchInteractionModeActivateAction.performed -= OnSwitchInteractionMode;
        }
    }

    void OnStartInteraction(InputAction.CallbackContext context)
    {
        switch (InteractionMode)
        {
            case InteractionMode.Ray:
                OnStartRay(context);
                break;
            case InteractionMode.Teleport:
                OnStartTeleport(context);
                break;
            case InteractionMode.None:
                break;
            default:
                throw new NotImplementedException();
        }
    }

    void OnStartTeleport(InputAction.CallbackContext context)
    {
        if (TeleportBlocked) return;

        m_postponedDeactivateInteract = false;

        if (m_teleportInteractor != null)
            m_teleportInteractor.gameObject.SetActive(true);

        if (m_rayInteractor != null)
            m_rayInteractor.gameObject.SetActive(false);
    }

    void OnStartRay(InputAction.CallbackContext context)
    {
        if (RayBlocked) return;

        if (m_rayInteractor != null)
            m_rayInteractor.gameObject.SetActive(true);

        if (m_teleportInteractor != null)
            m_teleportInteractor.gameObject.SetActive(false);
    }

    void OnCancelInteraction(InputAction.CallbackContext context)
    {
        switch (InteractionMode)
        {
            case InteractionMode.Ray:
                OnCancelRay(context);
                break;
            case InteractionMode.Teleport:
                OnCancelTeleport(context);
                break;
            case InteractionMode.None:
                break;
            default:
                throw new NotImplementedException();
        }
    }

    void OnCancelTeleport(InputAction.CallbackContext context)
    {
        m_postponedDeactivateInteract = true;
    }

    void OnCancelRay(InputAction.CallbackContext context)
    {
        m_postponedDeactivateInteract = true;
    }

    void OnSwitchInteractionMode(InputAction.CallbackContext context)
    {
        switch (InteractionMode)
        {
            case InteractionMode.Ray:
                InteractionMode = InteractionMode.Teleport;
                m_interactionModeChanged?.Invoke(InteractionMode.Teleport);
                break;
            case InteractionMode.Teleport:
                InteractionMode = InteractionMode.None;
                m_interactionModeChanged?.Invoke(InteractionMode.None);
                break;
            case InteractionMode.None:
                InteractionMode = InteractionMode.Ray;
                m_interactionModeChanged?.Invoke(InteractionMode.Ray);
                break;
            default:
                throw new NotImplementedException();
        }
        Debug.Log($"Interaction mode set to: {InteractionMode}");
    }
    
    protected void Awake() => m_postInteractionEventsRoutine = OnPostInteractionEvents();

    protected void OnEnable()
    {
        if (m_teleportInteractor != null)
            m_teleportInteractor.gameObject.SetActive(false);

        if (m_rayInteractor != null)
            m_rayInteractor.gameObject.SetActive(false);

        SetupInteractorEvents();

        StartCoroutine(m_postInteractionEventsRoutine);
    }

    protected void OnDisable()
    {
        TeardownInteractorEvents();

        StopCoroutine(m_postInteractionEventsRoutine);
    }

    protected void Start()
    {
        RayBlocked = false;
        TeleportBlocked = false;
    }
    
    IEnumerator OnPostInteractionEvents()
    {
        while (true)
        {
            yield return null;

            if (m_postponedDeactivateInteract)
            {
                if (m_teleportInteractor != null)
                    m_teleportInteractor.gameObject.SetActive(false);
                
                if (m_rayInteractor != null)
                    m_rayInteractor.gameObject.SetActive(false);

                m_postponedDeactivateInteract = false;
            }
        }
    }

    public void ChangeInteractionMode(InteractionMode mode)
    {
        InteractionMode = mode;

        switch (InteractionMode)
        {
            case InteractionMode.Ray:
                m_rayInteractor.gameObject.SetActive(!RayBlocked);
                m_interactionModeChanged?.Invoke(InteractionMode.Ray);
                break;
            case InteractionMode.Teleport:
                m_teleportInteractor.gameObject.SetActive(!TeleportBlocked);
                m_interactionModeChanged?.Invoke(InteractionMode.Teleport);
                break;
            case InteractionMode.None:
                m_interactionModeChanged?.Invoke(InteractionMode.None);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void ToggleHandMenu(bool value)
    {
        if (value)
        {
            m_rayInteractor.gameObject.SetActive(true);
            TeleportBlocked = true;
        }
        else
        {
            if (InteractionMode != InteractionMode.Ray)
                m_rayInteractor.gameObject.SetActive(false);
            TeleportBlocked = false;
        }
    }

    static InputAction GetInputAction(InputActionReference actionReference)
    {
        if (actionReference != null)
            return actionReference.action;
        else
            return null;
    }
}
