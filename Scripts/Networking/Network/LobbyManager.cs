using System;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Util;

namespace Network
{
    public class LobbyManager : NetworkBehaviour
    {
        [SerializeField] NetworkPanel m_networkPanel;
        [SerializeField] Lobby m_lobby;

        readonly NetworkVariable<FixedString32Bytes> joincode = new(string.Empty);
        readonly NetworkVariable<ulong> hostId = new();

        public override void OnNetworkSpawn()
        {
            Debug.Log("Subscribing..");
            joincode.OnValueChanged += (_, newValue) => m_networkPanel.SetJoincode(newValue.ToString());
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.OnClientConnectedCallback += OnClientDisconnected;
            NetworkManager.OnClientStarted += OnClientStarted;
        }

        public override void OnNetworkDespawn()
        {
            Debug.Log("Unsubscribing..");
            joincode.OnValueChanged -= (_, newValue) => m_networkPanel.SetJoincode(newValue.ToString());
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.OnClientConnectedCallback -= OnClientDisconnected;
            NetworkManager.OnClientStarted -= OnClientStarted;
        }

        void OnClientStarted()
        {
            if (IsHost)
            {
                joincode.Value = NetworkUtil.GetLocalIpAddress().Split(".").Last();
                hostId.Value = OwnerClientId;
            }
            
            m_networkPanel.SetJoincode(joincode.Value.ToString());
            PopulateLobby();
        }

        void OnClientConnected(ulong clientId)
        {
            if (clientId == OwnerClientId) return;
            var entry = m_lobby.AddEntry(clientId, false, false);
            SetupKickButton(clientId, entry);
        }

        void OnClientDisconnected(ulong clientId)
        {
            if (clientId == OwnerClientId) return;
            m_lobby.RemoveEntry(clientId);
        }

        void PopulateLobby()
        {
            Debug.Log("Populating lobby entries.");
            m_lobby.AddEntry(OwnerClientId, true, IsHost);
            foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Debug.Log(id);
                if (id == OwnerClientId) continue;
                var entry = m_lobby.AddEntry(id, false, id == hostId.Value);
                SetupKickButton(id, entry);
            }
        }

        void SetupKickButton(ulong id, LobbyEntry entry)
        {
            if (IsHost)
            {
                entry.SetKickButtonVisibility(true);
                entry.AddListener(() =>
                {
                    NetworkManager.Singleton.DisconnectClient(id);
                    m_lobby.RemoveEntry(id);
                });
            }
        }
    }
}
