using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandMenuController : MonoBehaviour
{
    [SerializeField] private bool _openLastPanel;
    private MenuPanel _lastPanel = null;
    [SerializeField] private SoundClip _hoverClip;
    [SerializeField] private SoundClip _clickClip;

    [Header("Events")]
    [SerializeField] private UnityEvent _onEnable;
    [SerializeField] private UnityEvent _onDisable;

    [Header("UI Panels")] 
    [SerializeField] private List<MenuPanel> _panels;
    private AudioSource _audioSource;
    private EventTrigger.Entry _hover;
    private EventTrigger.Entry _click;
    private EventTrigger.Entry _deselect;

    private void Awake()
    {
        _hover = new EventTrigger.Entry();
        _hover.eventID = EventTriggerType.PointerEnter;
        _hover.callback.AddListener(_ => { PlaySound(_hoverClip); });

        _click = new EventTrigger.Entry();
        _click.eventID = EventTriggerType.PointerClick;
        _click.callback.AddListener(_ => { PlaySound(_clickClip); });

        _deselect = new EventTrigger.Entry();
        _deselect.eventID = EventTriggerType.Deselect;
        _deselect.callback.AddListener(_ => { PlaySound(_clickClip); });
    }

    private void Start()
    {
        if (!TryGetComponent(out _audioSource))
        {
            Debug.LogWarning("No AudioSource attached. Adding one.");
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _panels.ForEach(panel => 
        {
            RegisterPanel(panel);
            AddUiElementSoundFeedback(panel);
        });
    }

    private void OnEnable()
    {
        _onEnable?.Invoke();
        if (_openLastPanel && _lastPanel != null && _panels.Contains(_lastPanel))
            OpenPanel(_lastPanel);
        else
            OpenMainPanel();
    }

    private void OnDisable()
    {
        _onDisable?.Invoke();
    }

    public void OpenPanel(MenuPanel panel)
    {
        CloseAllPanels();
        panel.gameObject.SetActive(true);
        _lastPanel = panel;
    }

    public void OpenMainPanel()
    {
        CloseAllPanels();
        OpenPanel(_panels.First());
    }

    public void RegisterPanel(MenuPanel panel)
    {
        if (panel.TryGetComponent<MainPanel>(out var _)) return;

        panel.transform.SetParent(transform);
        panel.transform.SetPositionAndRotation(_panels.First().transform.position, _panels.First().transform.rotation);
        AddUiElementSoundFeedback(panel);
        if (!_panels.Contains(panel))
        {
            _panels.First().GetComponent<MainPanel>().AddPanelButton(panel, _panels.Count-1);
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
        }
    }

    private void CloseAllPanels()
    {
        foreach (var panel in _panels) panel.gameObject.SetActive(false);
    }

    private void PlaySound(SoundClip clip)
    {
        _audioSource.clip = clip.clip;
        _audioSource.volume = clip.volume;
        _audioSource.pitch = clip.pitch;
        _audioSource.Play();
    }

    private void AddUiElementSoundFeedback(MenuPanel panel)
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
        uiInteraction.triggers.Add(_hover);
        uiInteraction.triggers.Add(_click);
    }

    private void AddToggleSoundFeedback(Toggle toggle)
    {
        var uiInteraction = toggle.gameObject.AddComponent<EventTrigger>();
        uiInteraction.triggers.Add(_hover);
        uiInteraction.triggers.Add(_click); 
    }

    private void AddInputFieldSoundFeedback(TMP_InputField inputField)
    {
        var uiInteraction = inputField.gameObject.AddComponent<EventTrigger>();
        uiInteraction.triggers.Add(_hover);
        uiInteraction.triggers.Add(_click);
        uiInteraction.triggers.Add(_deselect);
    }
}
