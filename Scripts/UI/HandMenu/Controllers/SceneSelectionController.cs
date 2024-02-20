using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelectionController : HandUIController
{
    readonly bool m_fadeOnSceneChange;
    readonly GameObject m_buttonPrefab;
    readonly Transform m_content;
    readonly Button m_backButton;
    readonly Button m_quitButton;
    readonly List<Button> m_buttonList = new();

    public SceneSelectionController(HandView view,
                                    HandMenuManager canvasManager,
                                    bool fadeOnSceneChange,
                                    GameObject buttonPrefab,
                                    Transform content,
                                    Button backButton,
                                    Button quitButton)
        : base(view, canvasManager)
    {
        m_fadeOnSceneChange = fadeOnSceneChange;
        m_buttonPrefab = buttonPrefab;
        m_content = content;
        m_backButton = backButton;
        m_quitButton = quitButton;
        m_backButton.onClick.AddListener(() => Back());
        m_quitButton.onClick.AddListener(() => QuitGame());
    }

    public override void AddUIElementSoundFeedback(EventTrigger.Entry hover, EventTrigger.Entry click, EventTrigger.Entry deselect)
    {
        AddSoundFeedback(m_backButton.gameObject, hover, click);
        AddSoundFeedback(m_quitButton.gameObject, hover, click);
    }
}
