using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandMenuController : MonoBehaviour
{
    [SerializeField] private SoundClip _hoverClip;
    [SerializeField] private SoundClip _clickClip;
    [SerializeField] private bool _playHoverSound;

    [Header("Events")]
    [SerializeField] private UnityEvent _onEnable;
    [SerializeField] private UnityEvent _onDisable;

    [Header("UI Panels")] 
    [SerializeField] private List<MenuPanel> _panels;
    
    [HideInInspector] public bool openLastPanel;
    private MenuPanel _lastPanel = null;
    private EventTrigger.Entry _hover;
    private EventTrigger.Entry _click;
    private EventTrigger.Entry _deselect;

    private void Awake()
    {
        _hover = new EventTrigger.Entry();
        _hover.eventID = EventTriggerType.PointerEnter;
        _hover.callback.AddListener(_ => { PlaySound(_hoverClip); });

        _click = new EventTrigger.Entry();
        _click.eventID = EventTriggerType.Select;
        _click.callback.AddListener(_ => { PlaySound(_clickClip); });

        _deselect = new EventTrigger.Entry();
        _deselect.eventID = EventTriggerType.Deselect;
        _deselect.callback.AddListener(_ => { PlaySound(_clickClip); });
    }

    private void Start()
    {
        _panels.ForEach(panel => 
        {
            RegisterPanel(panel);
            AddUiElementSoundFeedback(panel);
        });

        openLastPanel = PlayerPrefs.GetInt("reopenPanel") == 1;
    }

    private void OnEnable()
    {
        _onEnable?.Invoke();
        if (openLastPanel && _lastPanel != null && _panels.Contains(_lastPanel))
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

        // Dirty solution to edge case. Scale gets set to 0 if the panel is added to an inactive menu
        // because the parent has a scale of 0 when it deactivates
        if (!gameObject.activeInHierarchy)
        {
            panel.transform.localScale = Vector3.one;
            panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 50, 0);
        }

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
        }
    }

    private void CloseAllPanels()
    {
        foreach (var panel in _panels) panel.gameObject.SetActive(false);
    }

    private void PlaySound(SoundClip clip)
    {
        var go = Instantiate(new GameObject("SoundFX"), transform);
        var source = go.AddComponent<AudioSource>();
        source.clip = clip.clip;
        source.volume = clip.volume;
        source.pitch = clip.pitch + Random.Range(-0.025f, 0.025f);
        source.Play();
        Destroy(go, clip.clip.length * 1.1f);
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
        if (_playHoverSound) uiInteraction.triggers.Add(_hover);
        uiInteraction.triggers.Add(_click);
    }

    private void AddToggleSoundFeedback(Toggle toggle)
    {
        var uiInteraction = toggle.gameObject.AddComponent<EventTrigger>();
        if (_playHoverSound) uiInteraction.triggers.Add(_hover);
        uiInteraction.triggers.Add(_click); 
    }

    private void AddInputFieldSoundFeedback(TMP_InputField inputField)
    {
        var uiInteraction = inputField.gameObject.AddComponent<EventTrigger>();
        if (_playHoverSound) uiInteraction.triggers.Add(_hover);
        uiInteraction.triggers.Add(_click);
        uiInteraction.triggers.Add(_deselect);
    }
}
