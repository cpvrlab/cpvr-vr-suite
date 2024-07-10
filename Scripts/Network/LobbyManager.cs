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

        public override void OnNetworkSpawn()
        {
            joincode.OnValueChanged += (_, newValue) => m_networkPanel.SetJoincode(newValue.ToString());
            NetworkManager.OnClientStarted += OnClientStarted;
        }

        public override void OnNetworkDespawn()
        {
            joincode.OnValueChanged -= (_, newValue) => m_networkPanel.SetJoincode(newValue.ToString());
            NetworkManager.OnClientStarted -= OnClientStarted;
        }

        void OnClientStarted()
        {
            if (IsHost)
                joincode.Value = NetworkUtil.GetLocalIpAddress().Split(".").Last();
            
            m_networkPanel.SetJoincode(joincode.Value.ToString());
        }
    }
}
