using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public sealed class HandMenuManager : CanvasManager
{
    [Header("Audio")]
    [SerializeField] SoundClip m_hoverClip;
    [SerializeField] SoundClip m_clickClip;
    [SerializeField] bool m_playHoverSound;

    [Header("Events")]
    [SerializeField] UnityEvent m_onEnable;
    [SerializeField] UnityEvent m_onDisable;

    bool m_openLastView;
    public bool OpenLastView 
    {
        get => m_openLastView; 
        set => m_openLastView = value;
    }
    HandUIController m_lastController = null;
    EventTrigger.Entry m_hover;
    EventTrigger.Entry m_click;
    EventTrigger.Entry m_deselect;

    protected override void Start()
    {
        base.Start();

        if (AudioManager.Instance != null)
        {
            SetupAudioEvents();
            foreach (var controller in controllers.OfType<HandUIController>())
                controller.AddUIElementSoundFeedback(m_hover, m_click, m_deselect);
        }
        
        if (TryGetController<MainHandUIController>(out var mainController))
            mainController.Initialize();

        m_openLastView = PlayerPrefs.GetInt("reopenView") == 1;
    }

    void OnEnable()
    {
        m_onEnable?.Invoke();
        if (m_openLastView && m_lastController != null && controllers.Contains(m_lastController))
            OpenView(m_lastController);
        else
            OpenMainView();
    }

    void OnDisable() => m_onDisable?.Invoke();

    public void OpenView(HandUIController controller)
    {
        CloseAllPanels();
        controller.SetViewActiveState(true);
        m_lastController = controller;
    }

    public void OpenView<T>() where T : HandUIController
    {
        if (TryGetController<T>(out var controller))
        {
            CloseAllPanels();
            controller.SetViewActiveState(true);
            m_lastController = controller;
        }
    }

    public void OpenMainView()
    {
        CloseAllPanels();
        if (TryGetController<MainHandUIController>(out var mainController))
            OpenView(mainController);
    }

    void CloseAllPanels()
    {
        foreach (var controller in controllers) 
            controller.SetViewActiveState(false);
    }

    void SetupAudioEvents()
    {
        if (AudioManager.Instance == null) return;

        if (m_hoverClip != null && m_playHoverSound)
        {
            m_hover = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            m_hover.callback.AddListener(_ => AudioManager.Instance.PlaySoundClip(m_hoverClip, transform));
        }

        if (m_clickClip != null)
        {
            m_click = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select
            };
            m_click.callback.AddListener(_ => AudioManager.Instance.PlaySoundClip(m_clickClip, transform));

            m_deselect = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Deselect
            };
            m_deselect.callback.AddListener(_ => AudioManager.Instance.PlaySoundClip(m_clickClip, transform));
        }
    }
}
