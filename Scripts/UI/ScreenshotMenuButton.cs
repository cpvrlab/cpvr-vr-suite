using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotMenuButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ValidateEmail();
    }

    public void ValidateEmail()
    {
        GetComponent<Button>().interactable = !string.IsNullOrEmpty(PlayerPrefs.GetString("emailAddress"));
    }
}
