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
        if (NetworkController.Instance != null)
        {
            NetworkController.Instance.NetworkManager.OnConnectionEvent += HandleConnectionEvent;
            UpdateLobby();
        }
    }

    void OnDisable()
    {
        if (NetworkController.Instance != null && NetworkController.Instance.NetworkManager != null)
            NetworkController.Instance.NetworkManager.OnConnectionEvent -= HandleConnectionEvent;
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

    void UpdateLobby()
    {
        var connectedClientIds = NetworkController.Instance.NetworkManager.ConnectedClientsIds;
        var existingClientIds = m_lobbyEntries.Select(x => x.ClientId);
        var missingIds = connectedClientIds.Except(existingClientIds);
        var removedIds = existingClientIds.Except(connectedClientIds);

        foreach (var missingId in missingIds)
            HandlePlayerSpawn(missingId);
        
        foreach (var removedId in removedIds)
            HandlePlayerDespawn(removedId);
    }

    void HandlePlayerSpawn(ulong clientId)
    {
        if (!m_lobbyEntries.Any(x => x.ClientId == clientId))
        {
            var entry = Instantiate(m_lobbyEntryPrefab, m_content);
            entry.Initialise(clientId,
                NetworkController.Instance.NetworkManager.LocalClientId == clientId,
                clientId == 0);
            m_lobbyEntries.Add(entry);
            entry.SetName("Player " + clientId);

            if (NetworkController.Instance.NetworkManager.IsHost)
            {
                entry.SetKickButtonVisibility(NetworkController.Instance.NetworkManager.LocalClientId != clientId);
                entry.AddListener(() => NetworkController.Instance.NetworkManager.DisconnectClient(entry.ClientId, "Kicked by host"));
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
