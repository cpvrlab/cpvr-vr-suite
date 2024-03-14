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
    public Action<GameObject> OnInteractionStarted;
    public Action<GameObject> OnInteractionEnded;
    [SerializeField] InteractionMode m_interactionMode = InteractionMode.Teleport;
    InteractionMode m_previousInteractionMode = InteractionMode.None;
    public InteractionMode InteractionMode
    {
        get => m_interactionMode;
        set => ChangeInteractionMode(value);
    }
    bool m_tempMode;

    #region Inspector references
    [Header("Left Hand")]
    [Header("Interactors")]
    [SerializeField] XRRayInteractor m_leftRayInteractor;
    [SerializeField] XRRayInteractor m_leftTeleportInteractor;
    
    bool m_leftInteractionBlocked;
    public bool LeftInteractionBlocked
    {
        get => m_leftInteractionBlocked;
        set => m_leftInteractionBlocked = value;
    }

    [Header("Actions")]
    [SerializeField] InputActionReference m_leftInteractModeActivate;
    [SerializeField] InputActionReference m_leftInteractModeCancel;

    [Space]
    [Header("Right Hand")]
    [Header("Interactors")]
    [SerializeField] XRRayInteractor m_rightRayInteractor;
    [SerializeField] XRRayInteractor m_rightTeleportInteractor;

    [Header("Actions")]
    [SerializeField] InputActionReference m_rightInteractModeActivate;
    [SerializeField] InputActionReference m_rightInteractModeCancel;
    
    bool m_rightInteractionBlocked;
    public bool RightInteractionBlocked
    {
        get => m_rightInteractionBlocked;
        set => m_rightInteractionBlocked = value;
    }

    [Space]
    [Header("Common Action")]
    [SerializeField] InputActionReference m_switchInteractionMode;

    [Header("Interaction Mode Changed Event")]
    public UnityEvent<InteractionMode> OnInteractionModeChanged;
    #endregion

    public const int k_UpdateOrder = XRInteractionUpdateOrder.k_Controllers - 1;
    IEnumerator m_postInteractionEventsRoutine;
    bool m_postponedDeactivateInteractLeft;
    bool m_postponedDeactivateInteractRight;

    void SetupInteractorEvents()
    {
        var leftInteractModeActivateAction = GetInputAction(m_leftInteractModeActivate);
        if (leftInteractModeActivateAction != null)
        {
            leftInteractModeActivateAction.performed += OnStartLeftInteraction;
            leftInteractModeActivateAction.canceled += OnCancelLeftInteraction;
        }

        var leftInteractionModeCancelAction = GetInputAction(m_leftInteractModeCancel);
        if (leftInteractionModeCancelAction != null)
        {
            leftInteractionModeCancelAction.performed += OnCancelLeftInteraction;
        }
        
        var rightInteractModeActivateAction = GetInputAction(m_rightInteractModeActivate);
        if (rightInteractModeActivateAction != null)
        {
            rightInteractModeActivateAction.performed += OnStartRightInteraction;
            rightInteractModeActivateAction.canceled += OnCancelRightInteraction;
        }

        var rightInteractionModeCancelAction = GetInputAction(m_rightInteractModeCancel);
        if (rightInteractionModeCancelAction != null)
        {
            rightInteractionModeCancelAction.performed += OnCancelRightInteraction;
        }

        var switchInteractionModeActivateAction = GetInputAction(m_switchInteractionMode);
        if (switchInteractionModeActivateAction != null)
        {
            switchInteractionModeActivateAction.performed += OnSwitchInteractionMode;
        }
    }

    void TeardownInteractorEvents()
    {
        var leftInteractModeActivateAction = GetInputAction(m_leftInteractModeActivate);
        if (leftInteractModeActivateAction != null)
        {
            leftInteractModeActivateAction.performed -= OnStartLeftInteraction;
            leftInteractModeActivateAction.canceled -= OnCancelLeftInteraction;
        }

        var leftInteractionModeCancelAction = GetInputAction(m_leftInteractModeCancel);
        if (leftInteractionModeCancelAction != null)
        {
            leftInteractionModeCancelAction.performed -= OnCancelLeftInteraction;
        }
        
        var rightInteractModeActivateAction = GetInputAction(m_rightInteractModeActivate);
        if (rightInteractModeActivateAction != null)
        {
            rightInteractModeActivateAction.performed -= OnStartRightInteraction;
            rightInteractModeActivateAction.canceled -= OnCancelRightInteraction;
        }

        var rightInteractionModeCancelAction = GetInputAction(m_rightInteractModeCancel);
        if (rightInteractionModeCancelAction != null)
        {
            rightInteractionModeCancelAction.performed -= OnCancelRightInteraction;
        }

        var switchInteractionModeActivateAction = GetInputAction(m_switchInteractionMode);
        if (switchInteractionModeActivateAction != null)
        {
            switchInteractionModeActivateAction.performed -= OnSwitchInteractionMode;
        }
    }

#region Left hand interaction methods
    void OnStartLeftInteraction(InputAction.CallbackContext context)
    {
        if (LeftInteractionBlocked) return;

        RightInteractionBlocked = true;

        switch (InteractionMode)
        {
            case InteractionMode.Ray:
                OnStartLeftRay(context);
                break;
            case InteractionMode.Teleport:
                OnStartLeftTeleport(context);
                break;
            case InteractionMode.None:
                break;
            default:
                throw new NotImplementedException();
        }
    }

    void OnStartLeftTeleport(InputAction.CallbackContext context)
    {
        m_postponedDeactivateInteractLeft = false;

        if (m_leftTeleportInteractor != null)
        {
            m_leftTeleportInteractor.gameObject.SetActive(true);
            OnInteractionStarted?.Invoke(m_leftTeleportInteractor.gameObject);
        }

        if (m_leftRayInteractor != null)
            m_leftRayInteractor.gameObject.SetActive(false);
    }

    void OnStartLeftRay(InputAction.CallbackContext context)
    {
        if (m_leftRayInteractor != null)
        {
            m_leftRayInteractor.gameObject.SetActive(true);
            OnInteractionStarted?.Invoke(m_leftRayInteractor.gameObject);
        }

        if (m_leftTeleportInteractor != null)
            m_leftTeleportInteractor.gameObject.SetActive(false);
    }

    void OnCancelLeftInteraction(InputAction.CallbackContext context)
    {
        if (InteractionMode == InteractionMode.Ray)
            OnInteractionEnded?.Invoke(m_leftRayInteractor.gameObject);
        else if (InteractionMode == InteractionMode.Teleport)
            OnInteractionEnded?.Invoke(m_leftTeleportInteractor.gameObject);
        
        RightInteractionBlocked = false;
        m_postponedDeactivateInteractLeft = true;
    }

    #endregion

#region Right hand interaction methods

    void OnStartRightInteraction(InputAction.CallbackContext context)
    {
        if (RightInteractionBlocked) return;

        LeftInteractionBlocked = true;

        switch (InteractionMode)
        {
            case InteractionMode.Ray:
                OnStartRightRay(context);
                break;
            case InteractionMode.Teleport:
                OnStartRightTeleport(context);
                break;
            case InteractionMode.None:
                break;
            default:
                throw new NotImplementedException();
        }
    }

    void OnStartRightTeleport(InputAction.CallbackContext context)
    {
        m_postponedDeactivateInteractRight = false;

        if (m_rightTeleportInteractor != null)
        {
            m_rightTeleportInteractor.gameObject.SetActive(true);
            OnInteractionStarted?.Invoke(m_rightTeleportInteractor.gameObject);
        }

        if (m_rightRayInteractor != null)
            m_rightRayInteractor.gameObject.SetActive(false);
    }

    void OnStartRightRay(InputAction.CallbackContext context)
    {
        if (m_rightRayInteractor != null)
        {
            m_rightRayInteractor.gameObject.SetActive(true);
            OnInteractionStarted?.Invoke(m_rightRayInteractor.gameObject);
        }

        if (m_rightTeleportInteractor != null)
            m_rightTeleportInteractor.gameObject.SetActive(false);
    }

    void OnCancelRightInteraction(InputAction.CallbackContext context)
    {
        if (InteractionMode == InteractionMode.Ray)
            OnInteractionEnded?.Invoke(m_rightRayInteractor.gameObject);
        else if (InteractionMode == InteractionMode.Teleport)
            OnInteractionEnded?.Invoke(m_rightTeleportInteractor.gameObject);
        
        LeftInteractionBlocked = false;
        m_postponedDeactivateInteractRight = true;
    }

    #endregion

    void OnSwitchInteractionMode(InputAction.CallbackContext context)
    {
        if (m_tempMode) return;
        InteractionMode = m_interactionMode switch
        {
            InteractionMode.Ray => InteractionMode.Teleport,
            InteractionMode.Teleport => InteractionMode.None,
            InteractionMode.None => InteractionMode.Ray,
            _ => throw new NotImplementedException(),
        };
    }

    protected void Awake() => m_postInteractionEventsRoutine = OnPostInteractionEvents();

    protected void OnEnable()
    {
        if (m_leftTeleportInteractor != null)
            m_leftTeleportInteractor.gameObject.SetActive(false);

        if (m_leftRayInteractor != null)
            m_leftRayInteractor.gameObject.SetActive(false);
        
        if (m_rightTeleportInteractor != null)
            m_rightTeleportInteractor.gameObject.SetActive(false);

        if (m_rightRayInteractor != null)
            m_rightRayInteractor.gameObject.SetActive(false);

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
        RightInteractionBlocked = false;
        LeftInteractionBlocked = false;
        OnInteractionModeChanged?.Invoke(m_interactionMode);
    }

    IEnumerator OnPostInteractionEvents()
    {
        while (true)
        {
            yield return null;

            if (m_postponedDeactivateInteractLeft)
            {
                if (m_leftTeleportInteractor != null)
                    m_leftTeleportInteractor.gameObject.SetActive(false);

                if (m_leftRayInteractor != null)
                    m_leftRayInteractor.gameObject.SetActive(false);
                
                m_postponedDeactivateInteractLeft = false;
            }

            if (m_postponedDeactivateInteractRight)
            {
                if (m_rightTeleportInteractor != null)
                    m_rightTeleportInteractor.gameObject.SetActive(false);

                if (m_rightRayInteractor != null)
                    m_rightRayInteractor.gameObject.SetActive(false);

                m_postponedDeactivateInteractRight = false;
            }

        }
    }

    void ChangeInteractionMode(InteractionMode mode)
    {
        if (m_tempMode)
        {
            m_previousInteractionMode = mode;
        }
        else
        {
            m_previousInteractionMode = m_interactionMode;
            m_interactionMode = mode;
        }

        m_leftTeleportInteractor.gameObject.SetActive(false);
        m_leftRayInteractor.gameObject.SetActive(false);
        m_rightTeleportInteractor.gameObject.SetActive(false);
        m_rightRayInteractor.gameObject.SetActive(false);

        Debug.Log($"Interaction mode set to: {mode}");
        OnInteractionModeChanged?.Invoke(mode);
    }

    public void ToggleHandMenu(bool value)
    {
        if (value)
        {
            InteractionMode = InteractionMode.Ray;
            m_tempMode = true;
        }
        else
        {
            m_tempMode = false;
            InteractionMode = m_previousInteractionMode;
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
