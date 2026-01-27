using UnityEngine;
using UnityEngine.UI;

namespace cpvr_vr_suite.Scripts.Runtime.UI
{
    [RequireComponent(typeof(Button))]
    public class ScreenshotMenuButton : MonoBehaviour
    {
        void OnEnable()
        {
            ValidateEmail();
        }

        public void ValidateEmail()
        {
            GetComponent<Button>().interactable = !string.IsNullOrEmpty(PlayerPrefs.GetString("emailAddress"));
        }
    }
}
