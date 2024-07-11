using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HideObjectWhenRayInteractorVisualDisabled : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The XRRayInteractor that this component monitors for blockages.")]
    XRRayInteractor m_Interactor;

    [SerializeField]
    [Tooltip("The GameObject to hide when the XRRayInteractor is blocked.")]
    GameObject m_ObjectToHide;

    XRInteractorLineVisual m_lineVisual;

    void OnEnable()
    {
        if (m_Interactor == null || m_ObjectToHide == null)
            enabled = false;
        else if (!m_Interactor.TryGetComponent(out m_lineVisual))
            enabled = false;
    }

    void Update()
    {
        m_ObjectToHide.SetActive(m_Interactor.isActiveAndEnabled &&
            !m_Interactor.IsBlockedByInteractionWithinGroup() &&
            m_lineVisual.isActiveAndEnabled);
    }
}
