using System;
using cpvr_vr_suite.Scripts.Runtime.Util;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace cpvr_vr_suite.Scripts.Runtime.Network
{
    public class NetworkController : Singleton<NetworkController>
    {
        public static event Action OnNetworkSessionStarted;
        public static event Action OnNetworkSessionEnded;

        [SerializeField] NetworkManager m_networkManagerPrefab;
        [SerializeField] NetworkSceneController m_networkSceneControllerPrefab;
        public NetworkSceneController NetworkSceneController { get; set; }
        [SerializeField] GroupedTeleportationManager m_groupedTeleportationManagerPrefab;
        public GroupedTeleportationManager GroupedTeleportationManager { get; set; }

        void Start()
        {
            //Spawn Network prefabs
            Instantiate(m_networkManagerPrefab);        
            NetworkManager.Singleton.OnConnectionEvent += OnClientConnected;
            NetworkManager.Singleton.OnClientStarted += OnClientStarted;
            NetworkManager.Singleton.OnClientStopped += _ => OnNetworkSessionEnded?.Invoke();

            SceneManager.LoadScene("XRRigBootstrap");
        }

        void OnClientStarted()
        {
            if (NetworkManager.Singleton.IsServer)
                SpawnNetworkObjects();
        }

        void OnClientConnected(NetworkManager manager, ConnectionEventData data)
        {
            if (data.EventType == ConnectionEvent.ClientConnected &&
                data.ClientId == manager.LocalClientId)
                OnNetworkSessionStarted?.Invoke();
        }

        void SpawnNetworkObjects()
        {
            if (NetworkSceneController == null)
            {
                NetworkSceneController = Instantiate(m_networkSceneControllerPrefab);
                NetworkSceneController.GetComponent<NetworkObject>().Spawn(false);
            }

            if (GroupedTeleportationManager == null)
            {
                GroupedTeleportationManager = Instantiate(m_groupedTeleportationManagerPrefab);
                GroupedTeleportationManager.GetComponent<NetworkObject>().Spawn(false);
            }
        }
        
        void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnConnectionEvent -= OnClientConnected;
                NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
                NetworkManager.Singleton.OnClientStopped -= _ => OnNetworkSessionEnded?.Invoke();
            }
        }
    }
}
