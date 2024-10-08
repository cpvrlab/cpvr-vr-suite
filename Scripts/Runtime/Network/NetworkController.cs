using System;
using Network;
using Unity.Netcode;
using UnityEngine;

public class NetworkController : Singleton<NetworkController>
{
    public static event Action OnNetworkSessionStarted;
    public static event Action OnNetworkSessionEnded;

    [field: SerializeField] public NetworkManager NetworkManager { get; private set; }
    [SerializeField] NetworkSceneController m_networkSceneController;
    public NetworkSceneController NetworkSceneController { get; set; }
    [SerializeField] GroupedTeleportationManager m_groupedTeleportationManager;
    public GroupedTeleportationManager GroupedTeleportationManager { get; set; }

    public void OnEnable()
    {
        NetworkManager.OnClientStarted += OnClientStarted;
        NetworkManager.OnClientStopped += _ => OnNetworkSessionEnded?.Invoke();
    }

    public void OnDisable()
    {
        if (NetworkManager != null)
        {
            NetworkManager.OnClientStarted -= OnClientStarted;
            NetworkManager.OnClientStopped += _ => OnNetworkSessionEnded?.Invoke();
        }
    }

    void OnClientStarted()
    {
        if (NetworkManager.IsServer)
            SpawnNetworkObjects();

        OnNetworkSessionStarted?.Invoke();
    }

    void SpawnNetworkObjects()
    {
        if (GroupedTeleportationManager == null)
        {
            GroupedTeleportationManager = Instantiate(m_groupedTeleportationManager);
            GroupedTeleportationManager.GetComponent<NetworkObject>().Spawn();
        }

        if (NetworkSceneController == null)
        {
            NetworkSceneController = Instantiate(m_networkSceneController);
            NetworkSceneController.GetComponent<NetworkObject>().Spawn();
        }
    }
}
