using UnityEngine;

public enum PanelType
{
    Static,
    Dynamic
}

public class MenuPanel : MonoBehaviour
{
    protected HandMenuController _handMenuController;
    [SerializeField] private PanelType _panelType = PanelType.Static;
    [SerializeField] private Sprite _sprite;

    public Sprite Sprite { get => _sprite; private set => _sprite = value; }
    public PanelType PanelType { get => _panelType; private set => _panelType = value; }

    protected virtual void Start()
    {
        Init();
    }

    public void OnBackClicked()
    {
        if (_handMenuController == null) return;
        _handMenuController.OpenMainPanel();
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Init()
    {
        if (_panelType == PanelType.Static)
        {
            _handMenuController = transform.parent.GetComponent<HandMenuController>();
        }

        if (_panelType == PanelType.Dynamic)
        {
            _handMenuController = FindFirstObjectByType<HandMenuController>();
            if (_handMenuController == null)
            {
                Debug.LogError($"[PANEL {transform.name}]: No HandMenuController found to attach panel to.");
                return;
            }
            _handMenuController.RegisterPanel(this);
        }
    }
}
