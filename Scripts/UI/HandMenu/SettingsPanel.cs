using cpvrlab_vr_suite.Scripts.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MenuPanel
{
    [SerializeField] DebugDisplay m_debugDisplay;
    [SerializeField] Toggle m_fpsToggle;
    [SerializeField] Toggle m_debugToggle;
    [SerializeField] Toggle m_gazeToggle;
    [SerializeField] Toggle m_panelToggle;
    [SerializeField] TMP_InputField m_inputField;
    [SerializeField] Button m_clearDebugLogButton;
    [SerializeField] TMP_Text m_infoText;
    public string InfoText 
    { 
        get => m_infoText.text;
        set => m_infoText.text = value;
    }

    void Awake()
    {
        if (string.IsNullOrEmpty(InfoText))
            InfoText = "Version: " + Application.version;
    }
    
    protected override void Start()
    {
        base.Start();
        
        m_fpsToggle.onValueChanged.AddListener(value => 
        {
            PlayerPrefs.SetInt("showFPS", value ? 1 : 0);
            m_debugDisplay.ActivateFpsText(value);
        });

        m_debugToggle.onValueChanged.AddListener(value => 
        {
            PlayerPrefs.SetInt("showDebug", value ? 1 : 0);
            m_debugDisplay.ActivateDebugLogText(value);
        });

        m_gazeToggle.onValueChanged.AddListener(value =>
        {
            if (RigManager.Instance != null && 
                RigManager.Instance.RigOrchestrator.TryGetInteractorManager<GazeManager>(out var gazeManager))
                gazeManager.SetActiveState(value);
        });

        m_panelToggle.onValueChanged.AddListener(value =>
        {
            if (handMenuController == null) return;
            handMenuController.openLastPanel = value;
            PlayerPrefs.SetInt("reopenPanel", value ? 1 : 0);
        });

        m_inputField.onDeselect.AddListener(value => 
        {
            if (MailSender.IsValidEmail(value))
                PlayerPrefs.SetString("emailAddress", value);
            else if (MailSender.IsValidEmail(PlayerPrefs.GetString("emailAddress")))
                m_inputField.text = PlayerPrefs.GetString("emailAddress");
        });

        m_clearDebugLogButton.onClick.AddListener(() => m_debugDisplay.ClearDebugLog());

        m_fpsToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("showFPS") == 1);
        m_debugToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("showDebug") == 1);
        m_gazeToggle.SetIsOnWithoutNotify(RigManager.Instance != null && 
                                        RigManager.Instance.RigOrchestrator.TryGetInteractorManager<GazeManager>(out var gazeManager) && 
                                        gazeManager.Operative);
        m_panelToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("reopenPanel") == 1);
        m_inputField.text = PlayerPrefs.GetString("emailAddress");
    }
}
