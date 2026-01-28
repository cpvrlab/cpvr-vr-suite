using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace cpvr_vr_suite.Scripts.Runtime.UI
{
    public class MainPanel : MonoBehaviour
    {
        public string Title
        {
            get => m_title != null ? m_title.text : string.Empty;
            set
            {
                if (m_title != null)
                    m_title.text = value;
            }
        }

        [SerializeField] HandMenuController m_handmenuController;
        [SerializeField] TMP_Text m_title;
        [SerializeField] Transform m_scrollviewContent;
        [SerializeField] GameObject m_buttonPrefab;

        readonly Dictionary<HandmenuPanel, Button> m_menuButtonDictionary = new();

        void OnEnable()
        {
            Title = SceneManager.GetActiveScene().name;
        }

        public void AddPanelButton(HandmenuPanel panel)
        {
            if (m_buttonPrefab == null)
            {
                Debug.LogError("ButtonPrefab not assigned!");
                return;
            }
            if (m_menuButtonDictionary.ContainsKey(panel)) return;

            var button = Instantiate(m_buttonPrefab, m_scrollviewContent).GetComponent<Button>();

            if (button.transform.GetChild(0).TryGetComponent<TMP_Text>(out var text))
                text.text = !string.IsNullOrEmpty(panel.PanelName) ? panel.PanelName : "Toggle Scene Objects";

            button.onClick.AddListener(() => m_handmenuController.OpenPanel(panel));
            m_handmenuController.AddButtonSoundFeedback(button);
            m_menuButtonDictionary.Add(panel, button);
        }

        public void RemovePanelButton(HandmenuPanel panel)
        {
            if (!m_menuButtonDictionary.TryGetValue(panel, out var button)) return;
            m_menuButtonDictionary.Remove(panel);
            Destroy(button.gameObject);
        }

        public void SetHandMenuController(HandMenuController controller) => m_handmenuController = controller;
    }
}
