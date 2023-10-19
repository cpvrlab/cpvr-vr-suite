using System;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MenuPanel
{
    [SerializeField] private Toggle _fpsToggle;
    [SerializeField] private Toggle _debugToggle;
    [SerializeField] private Toggle _gazeToggle;
    [SerializeField] private Toggle _panelToggle;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private GameObject _gazeInteractor;
    [SerializeField] private GameObject _gazeStabilizor;

    protected override void Start()
    {
        base.Start();
        
        _fpsToggle.onValueChanged.AddListener(value => 
        {
            PlayerPrefs.SetInt("showFPS", value ? 1 : 0);
        });

        _debugToggle.onValueChanged.AddListener(value => 
        {
            PlayerPrefs.SetInt("showDebug", value ? 1 : 0);
        });

        _gazeToggle.onValueChanged.AddListener(value =>
        {
            _gazeStabilizor.SetActive(value);
            _gazeInteractor.SetActive(value);
            PlayerPrefs.SetInt("useGaze", value ? 1 : 0);
        });

        _panelToggle.onValueChanged.AddListener(value =>
        {
            if (_handMenuController == null) return;
            _handMenuController.openLastPanel = value;
            PlayerPrefs.SetInt("reopenPanel", value ? 1 : 0);
        });

        _inputField.onDeselect.AddListener(value => 
        {
            if (!IsValidEmail(value)) return;
            PlayerPrefs.SetString("emailAddress", value);
        });

        _fpsToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("showFPS") == 1);
        _debugToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("showDebug") == 1);
        _gazeToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("useGaze") == 1);
        _panelToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("reopenPanel") == 1);
        _inputField.text = PlayerPrefs.GetString("emailAddress");
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return false;
        try
        {
            var mailAddress = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
