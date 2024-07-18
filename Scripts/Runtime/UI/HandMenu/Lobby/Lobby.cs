using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Lobby : NetworkBehaviour
{
    [SerializeField] LobbyEntry m_lobbyEntryPrefab;
    [SerializeField] Transform m_content;
    readonly List<LobbyEntry> m_lobbyEntries = new();

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnConnectionEvent += HandleConnectionEvent;
        HandlePlayerSpawn(NetworkManager.Singleton.LocalClientId);
    }

    void HandleConnectionEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (!IsHost && data.EventType == ConnectionEvent.ClientConnected)
            foreach (var peerId in data.PeerClientIds)
                HandlePlayerSpawn(peerId);

        switch (data.EventType)
        {
            case ConnectionEvent.PeerConnected:
                HandlePlayerSpawn(data.ClientId);
                break;
            case ConnectionEvent.PeerDisconnected:
                HandlePlayerDespawn(data.ClientId);
                break;
            default:
                break;
        }
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnConnectionEvent -= HandleConnectionEvent;

        foreach (var entry in m_lobbyEntries)
        {
            entry.transform.SetParent(null);
            Destroy(entry.gameObject);
        }
        m_lobbyEntries.Clear();
    }

    void HandlePlayerSpawn(ulong clientId)
    {
        if (!m_lobbyEntries.Any(x => x.ClientId == clientId))
        {
            var entry = Instantiate(m_lobbyEntryPrefab, m_content);
            entry.Initialise(clientId,
                NetworkManager.Singleton.LocalClientId == clientId,
                clientId == 0);
            m_lobbyEntries.Add(entry);
            entry.SetName("Player " + clientId);

            if (IsHost)
            {
                entry.SetKickButtonVisibility(NetworkManager.Singleton.LocalClientId != clientId);
                entry.AddListener(() => NetworkManager.Singleton.DisconnectClient(entry.ClientId));
            }

            m_lobbyEntries.Sort((x, y) => x.ClientId.CompareTo(y.ClientId));

            for (int i = 0; i < m_lobbyEntries.Count; i++)
            {
                m_lobbyEntries[i].transform.SetSiblingIndex(i);
            }
        }
    }

    void HandlePlayerDespawn(ulong clientId)
    {
        var entryToRemove = m_lobbyEntries.FirstOrDefault(x => x.ClientId == clientId);
        if (entryToRemove != null)
        {
            entryToRemove.transform.SetParent(null);
            Destroy(entryToRemove.gameObject);
            m_lobbyEntries.Remove(entryToRemove);
        }
    }
}
