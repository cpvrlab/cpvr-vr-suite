using UnityEngine;
using UnityEngine.UI;

public class SceneSelectionView : HandView
{
    [SerializeField] bool m_fadeOnSceneChange;
    [SerializeField] GameObject m_buttonPrefab;
    [SerializeField] Transform m_content;
    [SerializeField] Button m_backButton;
    [SerializeField] Button m_quitButton;

    public override void Initialize(CanvasManager canvasManager)
    {
        Controller = new SceneSelectionController(this,
                                                (HandMenuManager) canvasManager,
                                                m_fadeOnSceneChange,
                                                m_buttonPrefab,
                                                m_content,
                                                m_backButton,
                                                m_quitButton);
    }
}
