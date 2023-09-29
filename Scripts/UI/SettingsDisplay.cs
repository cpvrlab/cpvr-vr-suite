using System;
using System.Collections.Generic;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace cpvr_vr_suite.Scripts.UI
{
    [DefaultExecutionOrder(1000)]
    public class SettingsDisplay : MonoBehaviour
    {
        [SerializeField] private MenuController menuController;
        [SerializeField] private TMP_Text fpsText;
        [SerializeField] private FpsCounter fpsCounter;
        [SerializeField] private TMP_Text debugLog;
        [SerializeField] private Toggle fpsToggle;
        [SerializeField] private Toggle debugToggle;
        [SerializeField] private TMP_InputField emailAddress;
        [SerializeField] private Button screenshotButton;
    
        private Dictionary<string, string> _debugLogs = new ();

        private void Awake()
        {
            fpsToggle.isOn = PlayerPrefs.GetInt("showFPS") == 1;
            debugToggle.isOn = PlayerPrefs.GetInt("showDebug") == 1;
            emailAddress.text = PlayerPrefs.GetString("emailAddress");
        }

        public void OnFpsToggle()
        {
            PlayerPrefs.SetInt("showFPS", fpsToggle.isOn ? 1 : 0);
            fpsText.gameObject.SetActive(fpsToggle.isOn);
            fpsCounter.gameObject.SetActive(fpsToggle.isOn);
        }

        public void OnDebugToggle()
        {
            PlayerPrefs.SetInt("showDebug", debugToggle.isOn ? 1 : 0);
            debugLog.gameObject.SetActive(debugToggle.isOn);
            if (debugToggle.isOn)
                Application.logMessageReceived += HandleLog;
            else
                Application.logMessageReceived -= HandleLog;
        }

        public void OnEmailFieldExited()
        {
            if (IsValidEmail(emailAddress.text))
            {
                PlayerPrefs.SetString("emailAddress", emailAddress.text);
                screenshotButton.interactable = true;
            }
        }

        public void OnBackClicked() => menuController.OpenMainPanel();

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Log)
            {
                var splitString = logString.Split(char.Parse(":"));
                var debugKey = splitString[0];
                var debugValue = splitString.Length > 1 ? splitString[1] : "";

                if (_debugLogs.ContainsKey(debugKey))
                    _debugLogs[debugKey] = debugValue;
                else
                    _debugLogs.Add(debugKey, debugValue);
            }

            var displayText = "";
            foreach (KeyValuePair<string, string> log in _debugLogs)
            {
                if (log.Value == "")
                    displayText += log.Key + "\n";
                else
                    displayText += log.Key + ": " + log.Value + "\n";
            }

            debugLog.text = displayText;
        }
        
        private static bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException e)
            {
                Debug.Log(e);
                return false;
            }
        }
    }
}
