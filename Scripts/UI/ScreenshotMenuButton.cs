using UnityEngine;
using UnityEngine.UI;

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
