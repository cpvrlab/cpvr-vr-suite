using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MarkerInteractable : XRGrabInteractable
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (args.interactorObject is not XRRayInteractor interactor) return;
        Debug.Log(interactor.transform.name);
        if (interactor.TryGetComponent<XRInteractorLineVisual>(out var lineVisual) && interactor.TryGetComponent<LineRenderer>(out var renderer))
        {
            Debug.Log("Disabling renderer");
            lineVisual.enabled = false;
            renderer.enabled = false;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (args.interactorObject is not XRRayInteractor interactor) return;
        Debug.Log(interactor.transform.name);
        if (interactor.TryGetComponent<XRInteractorLineVisual>(out var lineVisual) && interactor.TryGetComponent<LineRenderer>(out var renderer))
        {
            Debug.Log("Enabling renderer");
            lineVisual.enabled = true;
            renderer.enabled = true;
        }
    }
}
