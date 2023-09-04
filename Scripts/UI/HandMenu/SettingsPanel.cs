using System;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MenuPanel
{
    [SerializeField] private Toggle _fpsToggle;
    [SerializeField] private Toggle _debugToggle;
    [SerializeField] private TMP_InputField _inputField;

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

        _inputField.onDeselect.AddListener(value => 
        {
            if (!IsValidEmail(value)) return;
            PlayerPrefs.SetString("emailAddress", value);
        });

        _fpsToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("showFPS") == 1);
        _debugToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("showDebug") == 1);
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
