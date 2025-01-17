using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text m_fpsText;
    [SerializeField] TMP_Text m_debugLogText;
    [SerializeField] bool m_addStackTrace = false;

    readonly Dictionary<string, string> m_debugLogs = new();

    void Start()
    {
        ActivateFpsText(PlayerPrefs.GetInt("showFPS") == 1);
        ActivateDebugLogText(PlayerPrefs.GetInt("showDebug") == 1);
    }

    public void ActivateFpsText(bool value) => m_fpsText.gameObject.SetActive(value);

    public void ActivateDebugLogText(bool value)
    {
        m_debugLogText.gameObject.SetActive(value);
        if (value)
            Application.logMessageReceived += HandleLog;
        else
            Application.logMessageReceived -= HandleLog;
    }

    public void ClearDebugLog()
    {
        m_debugLogs.Clear();
        m_debugLogText.text = "";
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log ||
        type == LogType.Exception ||
        type == LogType.Warning ||
        type == LogType.Error)
        {
            var splitString = logString.Split(char.Parse(":"));
            var debugKey = splitString[0];
            var debugValue = m_addStackTrace ? logString + " - " + stackTrace : logString; 

            if (m_debugLogs.ContainsKey(debugKey))
                m_debugLogs[debugKey] = debugValue;
            else
                m_debugLogs.Add(debugKey, debugValue);
        }

        var displayText = "";
        foreach (KeyValuePair<string, string> log in m_debugLogs)
            displayText += log.Value + "\n";

        m_debugLogText.text = displayText;
    }
}
