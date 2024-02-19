public abstract class Controller
{
    protected View view;
    public View View { get => view; }
    protected CanvasManager canvasManager;

    public Controller(View view, CanvasManager canvasManager)
    {
        this.view = view;
        this.canvasManager = canvasManager;
        this.canvasManager.RegisterController(this);
    }

    public virtual void SetViewActiveState(bool state) => view.gameObject.SetActive(state);

    protected void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
