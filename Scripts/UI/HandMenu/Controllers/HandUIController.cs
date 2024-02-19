using UnityEngine;
using UnityEngine.EventSystems;

public abstract class HandUIController : Controller
{
    protected new readonly HandMenuManager canvasManager;
    public HandUIController(View view, HandMenuManager canvasManager) : base(view, canvasManager)
    {
        this.canvasManager = canvasManager;
    }

    public abstract void AddUIElementSoundFeedback(EventTrigger.Entry hover, EventTrigger.Entry click, EventTrigger.Entry deselect);

    protected void AddSoundFeedback(GameObject go, params EventTrigger.Entry[] eventtriggers)
    {
        if (go == null || eventtriggers == null || eventtriggers.Length == 0) return;

        if (!go.TryGetComponent<EventTrigger>(out var uiInteraction))
            uiInteraction = go.AddComponent<EventTrigger>();

        foreach (var trigger in eventtriggers)
            if (trigger != null)
                uiInteraction.triggers.Add(trigger);
    }

    protected void Back() => canvasManager.OpenView<MainHandUIController>();
}
