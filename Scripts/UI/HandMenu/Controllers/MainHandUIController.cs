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
    bool m_initialized = false;

    public MainHandUIController(HandView view,
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
        m_screenshotButton.onClick.AddListener(async () =>
        {
            if (canvasManager.TryGetController<InfoController>(out var controller, true))
            {
                m_screenshotButton.interactable = false;
                await controller.TakeScreenshot(PlayerPrefs.GetString("emailAddress"));
                m_screenshotButton.interactable = true;
            }
        });
        m_sceneSelectionButton = sceneSelectionButton;
        m_sceneSelectionButton.onClick.AddListener(() => canvasManager.OpenView<SceneSelectionController>());
        m_settingsButton = settingsButton;
        m_settingsButton.onClick.AddListener(() => canvasManager.OpenView<SettingsController>());
        m_quitButton = quitButton;
        m_quitButton.onClick.AddListener(() => QuitGame());
    }

    public void Initialize()
    {
        if (m_initialized) return;
        
        SetButtonSprite(m_sceneSelectionButton, m_settingsButton);
        m_initialized = true;
    }

    public void SetScreenshotButtonState() => m_screenshotButton.interactable = !string.IsNullOrEmpty(PlayerPrefs.GetString("emailAddress"));
    
    public override void AddUIElementSoundFeedback(EventTrigger.Entry hover, EventTrigger.Entry click, EventTrigger.Entry deselect)
    {
        AddSoundFeedback(m_screenshotButton.gameObject, click, hover);
        AddSoundFeedback(m_sceneSelectionButton.gameObject, click, hover);
        AddSoundFeedback(m_settingsButton.gameObject, click, hover);
        AddSoundFeedback(m_quitButton.gameObject, click, hover);   
    }

    void SetButtonSprite(params Button[] buttons)
    {
        if (buttons.Length == 0) return;

        foreach (var button in buttons)
        {
            if (button.transform.GetChild(0).TryGetComponent<Image>(out var image) &&
                canvasManager.TryGetController<SceneSelectionController>(out var controller))
                image.sprite = controller.View.Sprite;
        }
    }
}
