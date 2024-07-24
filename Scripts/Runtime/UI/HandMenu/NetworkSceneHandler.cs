using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneHandler : MonoBehaviour, ISceneHandler
{
    public event Action SceneChangeStarted;
    public event Action SceneChangeCompleted;

    public void OnEnable()
    {
        NetworkController.Instance.NetworkManager.OnClientStarted += OnClientStarted;
        NetworkController.Instance.NetworkManager.OnClientStopped += OnClientStopped;
    }

    public void OnDisable()
    {
        if (NetworkController.Instance != null && NetworkController.Instance.NetworkManager != null)
        {
            NetworkController.Instance.NetworkManager.OnClientStarted -= OnClientStarted;
            NetworkController.Instance.NetworkManager.OnClientStopped -= OnClientStopped;
        }
    }

    void OnClientStarted()
    {
        NetworkController.Instance.NetworkManager.SceneManager.OnSceneEvent += HandleSceneEvent;
    }

    void OnClientStopped(bool value)
    {
        if (NetworkController.Instance != null &&
            NetworkController.Instance.NetworkManager != null &&
            NetworkController.Instance.NetworkManager.SceneManager != null)
        NetworkController.Instance.NetworkManager.SceneManager.OnSceneEvent -= HandleSceneEvent;
    }

    public void ChangeScene(int index)
    {
        if (index == SceneManager.GetActiveScene().buildIndex) return;

        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsConnectedClient)
        {
            SceneChangeStarted?.Invoke();
            var sceneChangeOperation = SceneManager.LoadSceneAsync(index);
            sceneChangeOperation.completed += _ => SceneChangeCompleted?.Invoke();
        }
        else
        {
            var sceneName = SceneUtility.GetScenePathByBuildIndex(index);
            NetworkSceneController.Instance.LoadSceneRpc(sceneName);
        }
    }

    void HandleSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.Load)
            SceneChangeStarted?.Invoke();
        else if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
            SceneChangeCompleted?.Invoke();
    }
}
