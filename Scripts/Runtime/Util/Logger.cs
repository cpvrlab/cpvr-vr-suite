using System;
using System.Collections.Generic;
using UnityEngine;

namespace cpvr_vr_suite.Scripts.Util
{
    public class Logger : MonoBehaviour
    {
        public event Action<string> OnLogEntry;
        [SerializeField] Color m_color = Color.white;
        public readonly Dictionary<string, string> m_logEntries = new();

        public void Log(string key, string msg)
        {
            if (!m_logEntries.ContainsKey(key))
            {
                // Add entry
                m_logEntries.Add(key, msg);
            }
            else
            {
                // Update entry
                m_logEntries[key] = msg;
            }

            OnLogEntry?.Invoke($"<color=#{ColorUtility.ToHtmlStringRGBA(m_color)}>{key}</color>: {msg}");
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(m_color)}>{key}</color>: {msg}");
        }

        public Dictionary<string, string> GetLogs() => m_logEntries;
    }
}
