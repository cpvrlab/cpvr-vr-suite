using System;
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

    [Header("Events")]
    [SerializeField] private UnityEvent _onEnable;
    [SerializeField] private UnityEvent _onDisable;

    [Header("UI Panels")] 
    [SerializeField] private List<MenuPanel> panels;
    private AudioSource _audioSource;

    private void Start()
    {
        if (!TryGetComponent(out _audioSource))
        {
            Debug.LogWarning("No AudioSource attached. Adding one.");
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        panels.ForEach(panel => AddUiElementSoundFeedback(panel));
    }

    private void OnEnable()
    {
        _onEnable?.Invoke();
        OpenMainPanel();
    }

    private void OnDisable()
    {
        _onDisable?.Invoke();
    }

    public void OpenPanel(int index)
    {
        CloseAllPanels();
        try
        {
            panels[index].gameObject.SetActive(true);
        }
        catch (ArgumentOutOfRangeException e)
        {
            panels.First().gameObject.SetActive(true);
            throw new IndexOutOfRangeException(
                $"Panel index {index} not within bounds of panel list with length {panels.Count}.\n{e.Message}");
        }
    }

    public void OpenMainPanel()
    {
        CloseAllPanels();
        OpenPanel(0);
    }

    private void CloseAllPanels()
    {
        foreach (var panel in panels) panel.gameObject.SetActive(false);
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
        var hover = new EventTrigger.Entry();
        hover.eventID = EventTriggerType.PointerEnter;
        hover.callback.AddListener(_ => { PlaySound(_hoverClip); });
        
        var click = new EventTrigger.Entry();
        click.eventID = EventTriggerType.PointerClick;
        click.callback.AddListener(_ => { PlaySound(_clickClip); });

        var deselect = new EventTrigger.Entry();
        deselect.eventID = EventTriggerType.Deselect;
        deselect.callback.AddListener(_ => { PlaySound(_clickClip); });
        
        var buttons = panel.GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            var uiInteraction = button.gameObject.AddComponent<EventTrigger>();
            uiInteraction.triggers.Add(hover);
            uiInteraction.triggers.Add(click);
        }

        var toggles = panel.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            var uiInteraction = toggle.gameObject.AddComponent<EventTrigger>();
            uiInteraction.triggers.Add(hover);
            uiInteraction.triggers.Add(click);                
        }

        var inputFields = panel.GetComponentsInChildren<TMP_InputField>();
        foreach (var inputField in inputFields)
        {
            var uiInteraction = inputField.gameObject.AddComponent<EventTrigger>();
            uiInteraction.triggers.Add(hover);
            uiInteraction.triggers.Add(click);
            uiInteraction.triggers.Add(deselect);
        }
    }
}
