using cpvr_vr_suite.Scripts.Runtime.Core;
using cpvr_vr_suite.Scripts.Runtime.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace cpvr_vr_suite.Scripts.Runtime.UI
{
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] HandMenuController m_handmenuController;
        [SerializeField] DebugDisplay m_debugDisplay;
        [SerializeField] Toggle m_fpsToggle;
        [SerializeField] Toggle m_debugToggle;
        [SerializeField] Toggle m_gazeToggle;
        [SerializeField] Toggle m_panelToggle;
        [SerializeField] Toggle m_passthroughToggle;
        [SerializeField] TMP_Text m_characterHeightText;
        [SerializeField] Button m_calibrateHeightButton;
        [SerializeField] TMP_InputField m_inputField;
        [SerializeField] Button m_clearDebugLogButton;
        [SerializeField] TMP_Text m_infoText;
        [SerializeField] Passthrough m_passthrough;

        public string InfoText
        {
            get => m_infoText.text;
            set => m_infoText.text = value;
        }


        void Awake()
        {
            RigManager.InstanceReady += Init;

            if (string.IsNullOrEmpty(InfoText))
                InfoText = "Version: " + Application.version;
        }

        void Start()
        {
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
                if (m_handmenuController == null) return;
                m_handmenuController.OpenLastPanel = value;
                PlayerPrefs.SetInt("reopenPanel", value ? 1 : 0);
            });

            m_passthroughToggle.SetIsOnWithoutNotify(m_passthrough.IsEnabled);
            m_passthroughToggle.onValueChanged.AddListener(value => m_passthrough.ActivePassthrough(value));

            if (RigManager.Instance.HeightCalculated)
                SetHeightText(RigManager.Instance.Height);
            m_calibrateHeightButton.onClick.AddListener(RigManager.Instance.CalibrateHeight);
            m_calibrateHeightButton.onClick.AddListener(() => Debug.Log("Calibration button clicked!"));

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

        void Init()
        {
            m_passthrough = RigManager.Instance.RigOrchestrator.Camera.GetComponent<Passthrough>();
            m_passthrough.PassthroughValueChanged += value => m_passthroughToggle.SetIsOnWithoutNotify(value);
            RigManager.Instance.OnHeightCalibrationStarted += () => m_calibrateHeightButton.interactable = false;
            RigManager.Instance.OnHeightCalibrationEnded += SetHeightText;
            RigManager.Instance.OnHeightCalibrationEnded += (_) => m_calibrateHeightButton.interactable = true;
        }

        void OnDestroy()
        {
            RigManager.InstanceReady -= Init;

            if (m_passthrough != null)
                m_passthrough.PassthroughValueChanged -= value => m_passthroughToggle.SetIsOnWithoutNotify(value);

            if (RigManager.Instance != null)
            {
                RigManager.Instance.OnHeightCalibrationStarted -= () => m_calibrateHeightButton.interactable = false;
                RigManager.Instance.OnHeightCalibrationEnded -= SetHeightText;
                RigManager.Instance.OnHeightCalibrationEnded -= (_) => m_calibrateHeightButton.interactable = true;
            }
        }

        void SetHeightText(float height) => m_characterHeightText.text = "Character height: " + height.ToString("0.00") + "m";
    }
}
