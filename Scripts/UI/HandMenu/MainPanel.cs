using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MenuPanel
{
    [SerializeField] private Transform _scrollviewContent;
    [SerializeField] private GameObject _buttonPrefab;
    private Dictionary<MenuPanel, Button> _menuButtonDictionary = new ();

    protected override void Start() => _handMenuController = transform.parent.GetComponent<HandMenuController>();

    public void AddPanelButton(MenuPanel panel, int index)
    {
        if (_menuButtonDictionary.ContainsKey(panel)) return;
        if (_buttonPrefab == null)
        {
            Debug.LogError("ButtonPrefab not assigned!");
            return;
        }


        var button = Instantiate(_buttonPrefab, _scrollviewContent).GetComponent<Button>();

        if (button.transform.GetChild(0).TryGetComponent<Image>(out var image) && panel.Sprite != null)
            image.sprite = panel.Sprite;

        button.onClick.AddListener(() => _handMenuController.OpenPanel(panel));
        _handMenuController.AddButtonSoundFeedback(button);
        _menuButtonDictionary.Add(panel, button);
    }

    public void RemovePanelButton(MenuPanel panel)
    {
        if (!_menuButtonDictionary.TryGetValue(panel, out var button)) return;
        _menuButtonDictionary.Remove(panel);
        Destroy(button.gameObject);
    }
}
