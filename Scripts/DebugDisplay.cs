using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace cpvrlab_vr_suite.Scripts
{
    public class DebugDisplay : MonoBehaviour
    {
        private Dictionary<string, string> _debugLogs = new ();

        public Text display;
        public Text fps;
        [SerializeField] private Toggle fpsToggle;

        private void Start()
        {
            fpsToggle.isOn = false;
            OnFpsToggle();
        }

        private void OnEnable() => Application.logMessageReceived += HandleLog;

        private void OnDisable() => Application.logMessageReceived -= HandleLog;
    
        public void OnFpsToggle() => fps.gameObject.SetActive(fpsToggle.isOn);

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

            display.text = displayText;
        }
    }
}

