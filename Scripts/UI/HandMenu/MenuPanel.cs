using UnityEngine;

public enum PanelType
{
    Static,
    Dynamic
}

public class MenuPanel : MonoBehaviour
{
    protected HandMenuController handMenuController;
    [SerializeField] PanelType m_panelType = PanelType.Static;
    [SerializeField] Sprite m_sprite;

    public Sprite Sprite { get => m_sprite; private set => m_sprite = value; }
    public PanelType PanelType { get => m_panelType; private set => m_panelType = value; }

    protected virtual void Start()
    {
        Init();
    }

    public void OnBackClicked()
    {
        if (handMenuController == null) return;
        handMenuController.OpenMainPanel();
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
        if (m_panelType == PanelType.Static)
        {
            handMenuController = transform.parent.GetComponent<HandMenuController>();
        }

        if (m_panelType == PanelType.Dynamic)
        {
            handMenuController = FindFirstObjectByType(typeof(HandMenuController), FindObjectsInactive.Include) as HandMenuController;
            if (handMenuController == null)
            {
                Debug.LogError($"[PANEL {transform.name}]: No HandMenuController found to attach panel to.");
                return;
            }
            handMenuController.RegisterPanel(this);
        }
    }
}
