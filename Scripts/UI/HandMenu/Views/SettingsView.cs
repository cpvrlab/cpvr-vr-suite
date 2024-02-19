using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : HandView
{
    [SerializeField] Button m_backButton;
    [SerializeField] Button m_quitButton;

    public override void Initialize(CanvasManager canvasManager)
    {
        Controller = new SettingsController(this,
                                            (HandMenuManager) canvasManager,
                                            m_backButton,
                                            m_quitButton);
    }
}
