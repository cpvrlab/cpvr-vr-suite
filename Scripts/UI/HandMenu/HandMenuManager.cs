using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public sealed class HandMenuManager : CanvasManager
{
    [Header("Audio")]
    [SerializeField] SoundClip _hoverClip;
    [SerializeField] SoundClip _clickClip;
    [SerializeField] bool _playHoverSound;

    [Header("Events")]
    [SerializeField] UnityEvent m_onEnable;
    [SerializeField] UnityEvent m_onDisable;

    bool m_openLastView;
    public bool OpenLastView { get => m_openLastView; }
    Controller m_lastController = null;
    EventTrigger.Entry m_hover;
    EventTrigger.Entry m_click;
    EventTrigger.Entry m_deselect;

    void Awake()
    {
        // m_hover = new EventTrigger.Entry();
        // m_hover.eventID = EventTriggerType.PointerEnter;
        // m_hover.callback.AddListener(_ => { PlaySound(_hoverClip); });

        // m_click = new EventTrigger.Entry();
        // m_click.eventID = EventTriggerType.Select;
        // m_click.callback.AddListener(_ => { PlaySound(_clickClip); });

        // m_deselect = new EventTrigger.Entry();
        // m_deselect.eventID = EventTriggerType.Deselect;
        // m_deselect.callback.AddListener(_ => { PlaySound(_clickClip); });
    }

    protected override void Start()
    {
        base.Start();

        // TODO: Set controller sound feedback

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

    public void OpenView(Controller controller)
    {
        CloseAllPanels();
        controller.SetViewActiveState(true);
        m_lastController = controller;
    }

    public void OpenMainView()
    {
        CloseAllPanels();
        TryGetController<MainHandUIController>(out var mainController);
        OpenView(mainController);
    }

    void CloseAllPanels()
    {
        foreach (var controller in controllers) 
            controller.SetViewActiveState(false);
    }
}
