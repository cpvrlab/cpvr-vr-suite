using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : HandView
{
    [SerializeField] Toggle m_fpsToggle;
    [SerializeField] Toggle m_debugToggle;
    [SerializeField] Toggle m_gazeToggle;
    [SerializeField] Toggle m_viewToggle;
    [SerializeField] TMP_InputField m_emailInputField;
    [SerializeField] TMP_Text m_infoText;
    [SerializeField] Button m_clearDebugButton;
    [SerializeField] Button m_backButton;
    [SerializeField] Button m_quitButton;

    public override void Initialize(CanvasManager canvasManager)
    {
        Controller = new SettingsController(this,
                                            (HandMenuManager) canvasManager,
                                            m_fpsToggle,
                                            m_debugToggle,
                                            m_gazeToggle,
                                            m_viewToggle,
                                            m_emailInputField,
                                            m_infoText,
                                            m_clearDebugButton,
                                            m_backButton,
                                            m_quitButton);
    }
}
