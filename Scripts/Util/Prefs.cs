using UnityEngine;

namespace cpvrlab_vr_suite.Scripts.Util
{
    public class Prefs : MonoBehaviour
    {
        public bool saveSettings;
        public bool showFPS;
        public bool showDebug;
        public string emailAddress;

        public void Load()
        {
            saveSettings = PlayerPrefs.GetInt("saveSettings", 0) > 0 ? true : false;
            showFPS = PlayerPrefs.GetInt("showFPS", 0) > 0 ? true : false;
            showDebug = PlayerPrefs.GetInt("showDebug", 0) > 0 ? true : false;
            emailAddress = PlayerPrefs.GetString("emailAddress", "");
            Debug.Log("Prefs.Loaded: " + Time.time);
        }

        public void Save()
        {
            PlayerPrefs.SetInt("saveSettings", saveSettings ? 1 : 0);
            PlayerPrefs.SetInt("showFPS", showFPS ? 1 : 0);
            PlayerPrefs.SetInt("showDebug", showDebug ? 1 : 0);
            PlayerPrefs.SetString("emailAddress", emailAddress);
            Debug.Log("Prefs.Saved: " + Time.time);
        }
    }
}
