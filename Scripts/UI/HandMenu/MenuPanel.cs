using UnityEngine;
using UnityEngine.UI;

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

    protected virtual void Start()
    {
        Init();
    }

    public void OnBackClicked() => _handMenuController?.OpenMainPanel();

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
        if (transform.parent == null || !transform.parent.TryGetComponent(out _handMenuController))
        {
            if (_panelType == PanelType.Static)
            {
                Debug.LogError($"[PANEL {transform.name}]: Static panel does not have a parent with a HandMenuController attached.");
            }
            else if (_panelType == PanelType.Dynamic)
            {
                _handMenuController = FindFirstObjectByType<HandMenuController>();
                if (_handMenuController != null)
                {
                    _handMenuController.RegisterPanel(this);
                }
                else
                {
                    Debug.LogError($"[PANEL {transform.name}]: No HandMenuController found to attach dynamic panel to.");
                }
            }
        }
    }
}
