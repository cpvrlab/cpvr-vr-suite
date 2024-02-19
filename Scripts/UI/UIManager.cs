using System.Collections.Generic;
using UnityEngine;

public sealed class UIManager : MonoBehaviour
{
    [SerializeField] List<CanvasManager> m_canvasManagers = new();

    public bool TryGetController<T>(CanvasManager caller, out T controller) where T : Controller
    {
        controller = null;
        var result = false;
        foreach (var cm in m_canvasManagers)
        {
            if (cm == caller) continue;
            if (cm.TryGetController(out controller))
            {
                result = true;
                break;
            }
        }
        return result;
    }
}
