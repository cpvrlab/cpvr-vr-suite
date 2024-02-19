using UnityEngine;
using UnityEngine.UI;

public class MainHandUIView : HandView
{
    [SerializeField] Transform m_scrollviewContent;
    [SerializeField] GameObject m_buttonPrefab;
    [SerializeField] Button m_screenshotButton;
    [SerializeField] Button m_sceneselectionButton;
    [SerializeField] Button m_settingsButton;
    [SerializeField] Button m_quitButton;

    public override void Initialize(CanvasManager canvasManager)
    {
        Controller = new MainHandUIController(this,
                                            (HandMenuManager) canvasManager,
                                            m_scrollviewContent,
                                            m_buttonPrefab,
                                            m_screenshotButton,
                                            m_sceneselectionButton,
                                            m_settingsButton,
                                            m_quitButton);
    }
}
