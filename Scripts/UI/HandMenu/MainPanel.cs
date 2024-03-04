using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MenuPanel
{
    [SerializeField] TMP_Text m_title;
    public string Title
    {
        get
        {
            if (m_title != null)
                return m_title.text;
            else
                return string.Empty;
        }
        set
        {
            if (m_title != null)
                m_title.text = value;
        }
    }
    [SerializeField] Transform m_scrollviewContent;
    [SerializeField] GameObject m_buttonPrefab;
    readonly Dictionary<MenuPanel, Button> m_menuButtonDictionary = new ();

    protected override void Start() => handMenuController = transform.parent.GetComponent<HandMenuController>();

    public void AddPanelButton(MenuPanel panel)
    {
        if (m_menuButtonDictionary.ContainsKey(panel)) return;
        if (m_buttonPrefab == null)
        {
            Debug.LogError("ButtonPrefab not assigned!");
            return;
        }


        var button = Instantiate(m_buttonPrefab, m_scrollviewContent).GetComponent<Button>();

        if (button.transform.GetChild(0).TryGetComponent<TMP_Text>(out var text))
            text.text = "Toggle Scene Objects";

        button.onClick.AddListener(() => handMenuController.OpenPanel(panel));
        handMenuController.AddButtonSoundFeedback(button);
        m_menuButtonDictionary.Add(panel, button);
    }

    public void RemovePanelButton(MenuPanel panel)
    {
        if (!m_menuButtonDictionary.TryGetValue(panel, out var button)) return;
        m_menuButtonDictionary.Remove(panel);
        Destroy(button.gameObject);
    }

    public void SetHandMenuController(HandMenuController controller) => handMenuController = controller;
}
