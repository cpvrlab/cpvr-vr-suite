using UnityEngine;

public enum PanelType
{
    Static,
    Dynamic
}

public sealed class HandmenuPanel : MonoBehaviour
{
    [field: SerializeField] public HandMenuController HandMenuController { get; private set; }
    [field: SerializeField] public PanelType PanelType { get; private set; } = PanelType.Static;
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public string PanelName { get; private set; }

    void Start() => Init();

    public void OnBackClicked()
    {
        if (HandMenuController == null) return;
        HandMenuController.OpenMainPanel();
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Init()
    {
        if (HandMenuController == null)
            HandMenuController = FindFirstObjectByType(typeof(HandMenuController), FindObjectsInactive.Include) as HandMenuController;
        if (HandMenuController != null)
            HandMenuController.RegisterPanel(this);
        else
            Debug.LogError($"[PANEL {transform.name}]: No HandMenuController found to attach panel to.");
    }
}
