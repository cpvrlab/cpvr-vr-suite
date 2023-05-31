using UnityEngine;
using UnityEngine.UI;

namespace cpvr_vr_suite.Scripts.UI
{
    public class SettingsInitializer : MonoBehaviour
    {
        [SerializeField] private GameObject fpsText;
        [SerializeField] private GameObject debugText;
        [SerializeField] private Toggle fpsToggle;
        [SerializeField] private Toggle debugToggle;
        
        private void Awake()
        {
            var fpsActive = PlayerPrefs.GetInt("showFPS") == 1;
            fpsText.SetActive(fpsActive);
            fpsToggle.isOn = fpsActive;

            var debugActive = PlayerPrefs.GetInt("showDebug") == 1;
            debugText.SetActive(debugActive);
            debugToggle.isOn = debugActive;
        }
    }
}
