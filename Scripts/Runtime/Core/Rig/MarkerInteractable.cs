using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    public class MarkerInteractable : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
    {
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            if (args.interactorObject is not UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor) return;
            Debug.Log(interactor.transform.name);
            if (interactor.TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual>(out var lineVisual) && interactor.TryGetComponent<LineRenderer>(out var renderer))
            {
                Debug.Log("Disabling renderer");
                lineVisual.enabled = false;
                renderer.enabled = false;
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (args.interactorObject is not UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor) return;
            Debug.Log(interactor.transform.name);
            if (interactor.TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual>(out var lineVisual) && interactor.TryGetComponent<LineRenderer>(out var renderer))
            {
                Debug.Log("Enabling renderer");
                lineVisual.enabled = true;
                renderer.enabled = true;
            }
        }
    }
}
