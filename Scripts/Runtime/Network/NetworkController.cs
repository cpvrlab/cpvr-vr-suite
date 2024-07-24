using Network;
using Unity.Netcode;
using UnityEngine;

public class NetworkController : Singleton<NetworkController>
{
    [field: SerializeField] public NetworkManager NetworkManager { get; private set; }
    [SerializeField] NetworkSceneController m_networkSceneController;
    [SerializeField] GroupedTeleportationManager m_groupedTeleportationManager;

    public void OnEnable()
    {
        NetworkManager.OnClientStarted += OnClientStarted;
    }

    public void OnDisable()
    {
        if (NetworkManager != null)
            NetworkManager.OnClientStarted -= OnClientStarted;
    }

    void OnClientStarted()
    {
        if (NetworkManager.IsServer)
            SpawnNetworkObjects();
    }

    void SpawnNetworkObjects()
    {
        if (GroupedTeleportationManager.Instance == null)
        {
            Instantiate(m_groupedTeleportationManager);
            GroupedTeleportationManager.Instance.GetComponent<NetworkObject>().Spawn();
        }

        if (NetworkSceneController.Instance == null)
        {
            Instantiate(m_networkSceneController);
            NetworkSceneController.Instance.GetComponent<NetworkObject>().Spawn();
        }
    }
}
