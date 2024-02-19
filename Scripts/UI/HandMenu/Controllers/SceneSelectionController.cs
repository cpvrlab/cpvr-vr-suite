using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneSelectionController : HandUIController
{
    readonly Button m_backButton;
    readonly Button m_quitButton;

    public SceneSelectionController(View view,
                                    HandMenuManager canvasManager,
                                    Button backButton,
                                    Button quitButton) 
        : base(view, canvasManager)
    {
        m_backButton = backButton;
        m_quitButton = quitButton;
        m_backButton.onClick.AddListener(() => Back());
        m_quitButton.onClick.AddListener(() => QuitGame());
    }

    public override void AddUIElementSoundFeedback(EventTrigger.Entry hover, EventTrigger.Entry click, EventTrigger.Entry deselect)
    {
        AddSoundFeedback(m_backButton.gameObject, hover, click);
        AddSoundFeedback(m_quitButton.gameObject, hover, click);
    }
}
