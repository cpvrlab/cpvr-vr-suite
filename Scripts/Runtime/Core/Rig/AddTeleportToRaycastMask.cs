using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    [RequireComponent(typeof(XRRayInteractor))]
    public class AddTeleportToRaycastMask : MonoBehaviour
    {
        void Start()
        {
            var interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
            if (interactor.interactionLayers == (interactor.interactionLayers | (1 << InteractionLayerMask.NameToLayer("Teleport"))))
            {
                interactor.raycastMask |= 1 << LayerMask.NameToLayer("Teleport");
                interactor.raycastMask |= 1 << LayerMask.NameToLayer("Non-Teleport");
            }
            Destroy(this);
        }
    }
}
