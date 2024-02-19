using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainHandUIController : HandUIController
{
    Transform m_content;
    GameObject m_buttonPrefab;
    readonly Button m_screenshotButton;
    readonly Button m_sceneSelectionButton;
    readonly Button m_settingsButton;
    readonly Button m_quitButton;

    public MainHandUIController(View view,
                                HandMenuManager canvasManager,
                                Transform scrollviewContent,
                                GameObject buttonPrefab,
                                Button screenshotButton,
                                Button sceneSelectionButton,
                                Button settingsButton,
                                Button quitButton)
        : base(view, canvasManager)
    {
        m_content = scrollviewContent;
        m_buttonPrefab = buttonPrefab;
        m_screenshotButton = screenshotButton;
        SetScreenshotButtonState();
        m_sceneSelectionButton = sceneSelectionButton;
        m_sceneSelectionButton.onClick.AddListener(() =>
        {
            if (canvasManager.TryGetController<SceneSelectionController>(out var controller))
                canvasManager.OpenView(controller);
        });
        m_settingsButton = settingsButton;
        m_settingsButton.onClick.AddListener(() =>
        {
            if (canvasManager.TryGetController<SettingsController>(out var controller))
                canvasManager.OpenView(controller);
        });
        m_quitButton = quitButton;
        m_quitButton.onClick.AddListener(() => QuitGame());
    }

    public void SetScreenshotButtonState() => m_screenshotButton.interactable = !string.IsNullOrEmpty(PlayerPrefs.GetString("emailAddress"));
    
    public override void AddUIElementSoundFeedback(EventTrigger.Entry hover, EventTrigger.Entry click, EventTrigger.Entry deselect)
    {
        AddSoundFeedback(m_screenshotButton.gameObject, click, hover);
        AddSoundFeedback(m_sceneSelectionButton.gameObject, click, hover);
        AddSoundFeedback(m_settingsButton.gameObject, click, hover);
        AddSoundFeedback(m_quitButton.gameObject, click, hover);
        
        Debug.Log("Soundfeedback added!");
    }
}
