using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MenuPanel : MonoBehaviour
{
    protected HandMenuController _handMenuController;

    protected virtual void Start()
    {
        if (!transform.parent.TryGetComponent<HandMenuController>(out _handMenuController))
        {
            Debug.LogError($"[PANEL {transform.name}]: Parentobject does not have a HandMenuController attached.");
        }
    }

    public void OnBackClicked() => _handMenuController.OpenMainPanel();

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
