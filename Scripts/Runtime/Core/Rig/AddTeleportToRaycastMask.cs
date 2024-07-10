using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRRayInteractor))]
public class AddTeleportToRaycastMask : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var interactor = GetComponent<XRRayInteractor>();
        if (interactor.interactionLayers == (interactor.interactionLayers | (1 << InteractionLayerMask.NameToLayer("Teleport"))))
        {
            interactor.raycastMask |= 1 << LayerMask.NameToLayer("Teleport");
            interactor.raycastMask |= 1 << LayerMask.NameToLayer("Non-Teleport");
        }
        Destroy(this);
    }
}
