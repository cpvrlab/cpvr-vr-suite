using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkSceneHandler : NetworkBehaviour, ISceneHandler
{
    public event Action SceneChangeStarted;
    public event Action SceneChangeCompleted;

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneEvent;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleSceneEvent;
    }

    public void ChangeScene(int index)
    {
        if (index == SceneManager.GetActiveScene().buildIndex) return;

        if (NetworkManager.Singleton == null)
        {
            SceneChangeStarted?.Invoke();
            var sceneChangeOperation = SceneManager.LoadSceneAsync(index);
            sceneChangeOperation.completed += _ => SceneChangeCompleted?.Invoke();
        }
        else
        {
            LoadSceneRpc(index);
        }
    }

    void HandleSceneEvent(SceneEvent sceneEvent)
    {
        if (!IsClient || sceneEvent.ClientId != NetworkManager.Singleton.LocalClientId) return;
        if (sceneEvent.SceneEventType == SceneEventType.Load)
            SceneChangeStarted?.Invoke();
        else if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
            SceneChangeCompleted?.Invoke();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    void LoadSceneRpc(int index)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(SceneUtility.GetScenePathByBuildIndex(index), LoadSceneMode.Single);
    }
}
