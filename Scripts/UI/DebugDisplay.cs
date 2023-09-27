using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _fpsText;
    [SerializeField] private TMP_Text _debugLogText;

    private Dictionary<string, string> _debugLogs = new ();

    private void Start()
    {
        activateFpsText(PlayerPrefs.GetInt("showFPS") == 1);
        activateDebugLogText(PlayerPrefs.GetInt("showDebug") == 1);
    }

    public void activateFpsText(bool value) => _fpsText.gameObject.SetActive(value);

    public void activateDebugLogText(bool value)
    {
        _debugLogText.gameObject.SetActive(value);
        if (value)
            Application.logMessageReceived += HandleLog;
        else
            Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Log || type == LogType.Exception)
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

            _debugLogText.text = displayText;
        }
}
