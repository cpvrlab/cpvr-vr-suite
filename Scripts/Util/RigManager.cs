using UnityEngine;
using UnityEngine.SceneManagement;

public class RigManager : Singleton<RigManager>
{
    [SerializeField] private GameObject _xrOrigin;
    public GameObject XrOrigin { get => _xrOrigin; }

    private void Start()
    {
        if (SceneManager.sceneCountInBuildSettings <= 1) return;
        SceneManager.LoadSceneAsync(1);
    }
}
