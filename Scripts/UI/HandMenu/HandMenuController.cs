using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandMenuController : MonoBehaviour
{
    [SerializeField] SoundClip _hoverClip;
    [SerializeField] SoundClip _clickClip;
    [SerializeField] bool _playHoverSound;

    [Header("Events")]
    [SerializeField] UnityEvent _onEnable;
    [SerializeField] UnityEvent _onDisable;

    [Header("UI Panels")] 
    [SerializeField] List<MenuPanel> _panels;
    
    [HideInInspector] public bool openLastPanel;
    MenuPanel m_lastPanel = null;
    bool m_initialized = false;
    EventTrigger.Entry m_hover;
    EventTrigger.Entry m_click;
    EventTrigger.Entry m_deselect;

    void Awake()
    {
        m_hover = new EventTrigger.Entry();
        m_hover.eventID = EventTriggerType.PointerEnter;
        m_hover.callback.AddListener(_ => { PlaySound(_hoverClip); });

        m_click = new EventTrigger.Entry();
        m_click.eventID = EventTriggerType.Select;
        m_click.callback.AddListener(_ => { PlaySound(_clickClip); });

        m_deselect = new EventTrigger.Entry();
        m_deselect.eventID = EventTriggerType.Deselect;
        m_deselect.callback.AddListener(_ => { PlaySound(_clickClip); });

        SceneManager.activeSceneChanged += (_,_) => UnregisterDynamicPanels();
    }

    void Start() => openLastPanel = PlayerPrefs.GetInt("reopenPanel") == 1;

    void OnEnable()
    {
        _onEnable?.Invoke();
        if (openLastPanel && m_lastPanel != null && _panels.Contains(m_lastPanel))
            OpenPanel(m_lastPanel);
        else
            OpenMainPanel();

        if (RigManager.Instance != null &&
            RigManager.Instance.RigOrchestrator.TryGetInteractorManager<GazeManager>(out var gazeManager))
        {
            gazeManager.BlockInteractor(true);
        }
    }

    void OnDisable()
    {
        _onDisable?.Invoke();

        if (RigManager.Instance != null &&
            RigManager.Instance.RigOrchestrator.TryGetInteractorManager<GazeManager>(out var gazeManager))
        {
            gazeManager.BlockInteractor(false);
        }
    }

    public bool TryGetMenuPanel<T>(out T panel) where T : MenuPanel
    {
        panel = _panels.OfType<T>().FirstOrDefault();
        return panel != null;
    }
    
    public void OpenPanel(MenuPanel panel)
    {
        CloseAllPanels();
        panel.gameObject.SetActive(true);
        m_lastPanel = panel;
    }

    public void OpenMainPanel()
    {
        CloseAllPanels();
        OpenPanel(_panels.First());
    }

    public void RegisterPanel(MenuPanel panel)
    {
        // if (panel.TryGetComponent<MainPanel>(out var _)) return;
        if (!m_initialized)
            InitializeMainPanel();

        panel.transform.SetParent(transform);
        panel.transform.SetPositionAndRotation(_panels.First().transform.position, _panels.First().transform.rotation);
        panel.transform.localScale = Vector3.one;

        if (!gameObject.activeInHierarchy)
            panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 50, 0);

        AddUiElementSoundFeedback(panel);
        
        if (!_panels.Contains(panel))
        {
            _panels.First().GetComponent<MainPanel>().AddPanelButton(panel);
            _panels.Add(panel);
            panel.gameObject.SetActive(false);
        } 
    }

    public void UnregisterDynamicPanels()
    {
        var dynamicPanels = _panels.Where(item => item.PanelType == PanelType.Dynamic).ToList();
        if (!_panels.First().TryGetComponent<MainPanel>(out var mainPanel))
        {
            Debug.LogError("First panel does not have a MainPanel Component.");
            return;
        }

        foreach (var panel in dynamicPanels)
        {
            mainPanel.RemovePanelButton(panel);
            _panels.Remove(panel);
            panel.transform.SetParent(null);
            panel.gameObject.SetActive(false);
            Destroy(panel.gameObject);
        }
    }

    void InitializeMainPanel()
    {
        if (_panels.First().TryGetComponent<MainPanel>(out var mainPanel))
        {
            m_initialized = true;
            RegisterPanel(_panels.First());
            mainPanel.SetHandMenuController(this);
        }
    }

    void CloseAllPanels()
    {
        foreach (var panel in _panels) panel.gameObject.SetActive(false);
    }

    void PlaySound(SoundClip clip)
    {
        var go = Instantiate(new GameObject("SoundFX"), transform);
        var source = go.AddComponent<AudioSource>();
        source.clip = clip.clip;
        source.volume = clip.volume;
        source.pitch = clip.pitch + Random.Range(-0.025f, 0.025f);
        source.Play();
        Destroy(go, clip.clip.length * 1.1f);
    }

    void AddUiElementSoundFeedback(MenuPanel panel)
    {        
        var buttons = panel.GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            AddButtonSoundFeedback(button);
        }

        var toggles = panel.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            AddToggleSoundFeedback(toggle);        
        }

        var inputFields = panel.GetComponentsInChildren<TMP_InputField>();
        foreach (var inputField in inputFields)
        {
            AddInputFieldSoundFeedback(inputField);
        }
    }

    public void AddButtonSoundFeedback(Button button)
    {
        var uiInteraction = button.gameObject.AddComponent<EventTrigger>();
        if (_playHoverSound) uiInteraction.triggers.Add(m_hover);
        uiInteraction.triggers.Add(m_click);
    }

    public void AddToggleSoundFeedback(Toggle toggle)
    {
        var uiInteraction = toggle.gameObject.AddComponent<EventTrigger>();
        if (_playHoverSound) uiInteraction.triggers.Add(m_hover);
        uiInteraction.triggers.Add(m_click); 
    }

    public void AddInputFieldSoundFeedback(TMP_InputField inputField)
    {
        var uiInteraction = inputField.gameObject.AddComponent<EventTrigger>();
        if (_playHoverSound) uiInteraction.triggers.Add(m_hover);
        uiInteraction.triggers.Add(m_click);
        uiInteraction.triggers.Add(m_deselect);
    }
}
