using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class RigManager : Singleton<RigManager>
{
    public UnityEvent enableInteractor;
    public UnityEvent enableTeleport;
    
    [SerializeField] GameObject m_xrOrigin;
    public GameObject XrOrigin { get => m_xrOrigin; }

    void Start()
    {
        if (SceneManager.sceneCountInBuildSettings <= 1) return;
        SceneManager.LoadSceneAsync(1);
    }
}
