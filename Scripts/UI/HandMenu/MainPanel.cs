using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MenuPanel
{
    [SerializeField] Transform m_scrollviewContent;
    [SerializeField] GameObject m_buttonPrefab;
    readonly Dictionary<MenuPanel, Button> m_menuButtonDictionary = new ();

    protected override void Start() => _handMenuController = transform.parent.GetComponent<HandMenuController>();

    public void AddPanelButton(MenuPanel panel)
    {
        if (m_menuButtonDictionary.ContainsKey(panel)) return;
        if (m_buttonPrefab == null)
        {
            Debug.LogError("ButtonPrefab not assigned!");
            return;
        }


        var button = Instantiate(m_buttonPrefab, m_scrollviewContent).GetComponent<Button>();

        if (button.transform.GetChild(0).TryGetComponent<Image>(out var image) && panel.Sprite != null)
            image.sprite = panel.Sprite;

        button.onClick.AddListener(() => _handMenuController.OpenPanel(panel));
        _handMenuController.AddButtonSoundFeedback(button);
        m_menuButtonDictionary.Add(panel, button);
    }

    public void RemovePanelButton(MenuPanel panel)
    {
        if (!m_menuButtonDictionary.TryGetValue(panel, out var button)) return;
        m_menuButtonDictionary.Remove(panel);
        Destroy(button.gameObject);
    }
}
