using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    [SerializeField] LobbyEntry m_lobbyEntryPrefab;
    [SerializeField] Transform m_content;
    readonly List<LobbyEntry> m_lobbyEntries = new();

    void OnEnable()
    {
        NetworkManager.Singleton.OnConnectionEvent += HandleConnectionEvent;
    }

    void HandleConnectionEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (!manager.IsHost && data.EventType == ConnectionEvent.ClientConnected)
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
            case ConnectionEvent.ClientConnected:
                HandlePlayerSpawn(manager.LocalClientId);
                foreach (var peerId in data.PeerClientIds)
                    HandlePlayerSpawn(peerId);
                break;
            case ConnectionEvent.ClientDisconnected:
                if (data.ClientId != manager.LocalClientId)
                    break;
                foreach (var entry in m_lobbyEntries)
                {
                    entry.transform.SetParent(null);
                    Destroy(entry.gameObject);
                }
                m_lobbyEntries.Clear();
                break;
            default:
                break;
        }
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnConnectionEvent -= HandleConnectionEvent;
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

            if (NetworkManager.Singleton.IsHost)
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
