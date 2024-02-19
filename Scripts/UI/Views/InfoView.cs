using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoView : View
{
    [Header("Screenshot references")]
    [SerializeField] TMP_Text m_countdownText;
    [SerializeField] TMP_Text m_resultText;
    [SerializeField] Image m_flashImage;

    [Header("Debug references")]
    [SerializeField] TMP_Text m_fpsText;
    [SerializeField] TMP_Text m_debugText;
    public new InfoController Controller { get; set; }

    public override void Initialize(CanvasManager canvasManager)
    {
        Controller = new InfoController(this,
                                        canvasManager,
                                        m_countdownText,
                                        m_resultText,
                                        m_flashImage,
                                        m_fpsText,
                                        m_debugText);
    }

    void Update() => Controller.UpdateFPSText();
}
