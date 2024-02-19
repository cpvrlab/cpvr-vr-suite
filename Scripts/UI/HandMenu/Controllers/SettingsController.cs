using cpvrlab_vr_suite.Scripts.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsController : HandUIController
{
    readonly Toggle m_fpsToggle;
    readonly Toggle m_debugToggle;
    readonly Toggle m_gazeToggle;
    readonly Toggle m_viewToggle;
    readonly TMP_InputField m_emailInputField;
    readonly TMP_Text m_infoText;
    readonly Button m_clearDebugButton;
    readonly Button m_backButton;
    readonly Button m_quitButton;

    public SettingsController(HandView view,
                            HandMenuManager canvasManager,
                            Toggle fpsToggle,
                            Toggle debugToggle,
                            Toggle gazeToggle,
                            Toggle viewToggle,
                            TMP_InputField emailInputField,
                            TMP_Text infoText,
                            Button clearDebugButton,
                            Button backButton,
                            Button quitButton) 
        : base(view, canvasManager)
    {
        m_fpsToggle = fpsToggle;
        m_debugToggle = debugToggle;
        m_gazeToggle = gazeToggle;
        m_viewToggle = viewToggle;
        m_emailInputField = emailInputField;
        m_infoText = infoText;
        m_clearDebugButton = clearDebugButton;
        m_backButton = backButton;
        m_quitButton = quitButton;
        m_fpsToggle.interactable = false;
        m_debugToggle.interactable = false;
        m_gazeToggle.onValueChanged.AddListener(value => PlayerPrefs.SetInt("useGaze", value ? 1 : 0));
        m_viewToggle.onValueChanged.AddListener(value =>
        {
            canvasManager.OpenLastView = value;
            PlayerPrefs.SetInt("reopenPanel", value ? 1 : 0);
        });
        m_emailInputField.onDeselect.AddListener(value => 
        {
            if (MailSender.IsValidEmail(value))
                PlayerPrefs.SetString("emailAddress", value);
            else if (MailSender.IsValidEmail(PlayerPrefs.GetString("emailAddress")))
                m_emailInputField.text = PlayerPrefs.GetString("emailAddress");
        });
        m_clearDebugButton.interactable = false;
        m_backButton.onClick.AddListener(() => Back());
        m_quitButton.onClick.AddListener(() => QuitGame());
    }

    public override void AddUIElementSoundFeedback(EventTrigger.Entry hover, EventTrigger.Entry click, EventTrigger.Entry deselect)
    {
        AddSoundFeedback(m_fpsToggle.gameObject, hover, click);
        AddSoundFeedback(m_debugToggle.gameObject, hover, click);
        AddSoundFeedback(m_gazeToggle.gameObject, hover, click);
        AddSoundFeedback(m_viewToggle.gameObject, hover, click);
        AddSoundFeedback(m_emailInputField.gameObject, hover, click, deselect);
        AddSoundFeedback(m_clearDebugButton.gameObject, hover, click);
        AddSoundFeedback(m_backButton.gameObject, hover, click);
        AddSoundFeedback(m_quitButton.gameObject, hover, click);
    }
}
