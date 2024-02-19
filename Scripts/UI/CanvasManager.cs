using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CanvasManager : MonoBehaviour
{
    protected readonly List<Controller> controllers = new();
    public IEnumerable<Controller> Controllers { get => controllers.AsReadOnly(); }

    protected virtual void Start()
    {
        var views = GetComponentsInChildren<View>();
        foreach (var view in views)
        {
            view.Initialize(this);
        }
    }

    public bool TryGetController<T>(out T controller, bool fullLookup = false) where T : Controller
    {
        controller = controllers.OfType<T>().FirstOrDefault();
        // TODO: Add fullLookup
        return controller != null;
    }
}
