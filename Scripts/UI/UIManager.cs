using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class UIManager : Singleton<UIManager>
{
    [SerializeField] List<CanvasManager> m_canvasManagers = new();

    protected override void Awake()
    {
        base.Awake();
        SceneManager.activeSceneChanged += (_, _) => GatherAllCanvasManagers();
    }

    void GatherAllCanvasManagers()
    {
        CleanUpManagers();
        foreach (var cm in FindObjectsOfType<CanvasManager>())
        {
            if (cm != null && !m_canvasManagers.Contains(cm))
                m_canvasManagers.Add(cm);
        }
    }

    void CleanUpManagers()
    {
        foreach (var cm in m_canvasManagers)
        {
            if (cm == null)
                m_canvasManagers.Remove(cm);
        }
    }
}
