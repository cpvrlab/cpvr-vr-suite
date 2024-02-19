public abstract class Controller
{
    protected View view;
    protected CanvasManager canvasManager;

    public Controller(View view, CanvasManager canvasManager)
    {
        this.view = view;
        this.canvasManager = canvasManager;
    }

    public void SetViewActiveState(bool state) => view.gameObject.SetActive(state);
}
