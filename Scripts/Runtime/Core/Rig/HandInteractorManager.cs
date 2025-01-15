using UnityEngine;
using UnityEngine.XR.Hands;

public class HandInteractorManager : MonoBehaviour
{
    [field: SerializeField] public SkinnedMeshRenderer HandMeshRenderer { get; private set; }
    [field: SerializeField] public XRHandTrackingEvents HandTrackingEvents { get; private set; }
    [SerializeField] InteractorControl[] m_interactorControls;

    void Update()
    {
        foreach (var control in m_interactorControls)
            control.interactor.SetActive(control.IsActive);
    }

    public void BlockTeleport(bool value)
    {
        foreach (var item in m_interactorControls)
        {
            if (item.interactor.name == "Teleport Interactor")
            {
                item.blocked = value;
                Debug.Log($"Teleport Interactor: the interactor on {transform.name} is now blocked: {value}");
            }
        }
    }

    public bool TryGetHandPosition(out Transform handTransform)
    {
        handTransform = HandMeshRenderer.rootBone.transform;
        return HandTrackingEvents.handIsTracked;
    }
}
