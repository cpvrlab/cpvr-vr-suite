using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using VR;

public class Lobby : NetworkBehaviour
{
    [SerializeField] LobbyEntry m_lobbyEntryPrefab;
    [SerializeField] Transform m_content;
    NetworkList<LobbyEntityState> m_lobbyEntities;
    readonly List<LobbyEntry> m_lobbyEntries = new();

    void Awake()
    {
        m_lobbyEntities = new();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            m_lobbyEntities.OnListChanged += HandleLobbyEntitiesChanged;

            Debug.Log(m_lobbyEntities.Count);
            foreach (var entity in m_lobbyEntities)
            {
                HandleLobbyEntitiesChanged(new NetworkListEvent<LobbyEntityState>
                {
                    Type = NetworkListEvent<LobbyEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if (!IsServer) return;

        m_lobbyEntities.Clear();
        VRPlayerBehaviour[] players = FindObjectsByType<VRPlayerBehaviour>(FindObjectsSortMode.None);
        foreach (var player in players)
            HandlePlayerSpawn(player);

        VRPlayerBehaviour.OnPlayerSpawned += HandlePlayerSpawn;
        VRPlayerBehaviour.OnPlayerDespawned += HandlePlayerDespawn;
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            m_lobbyEntities.OnListChanged -= HandleLobbyEntitiesChanged;
            for (int i = 0; i < m_lobbyEntries.Count; i++)
            {
                m_lobbyEntries[i].transform.SetParent(null);
                Destroy(m_lobbyEntries[i].gameObject);
            }
        }


        if (!IsServer) return;

        m_lobbyEntries.Clear();
        VRPlayerBehaviour.OnPlayerSpawned -= HandlePlayerSpawn;
        VRPlayerBehaviour.OnPlayerDespawned -= HandlePlayerDespawn;
    }

    void HandlePlayerSpawn(VRPlayerBehaviour player)
    {
        m_lobbyEntities.Add(new LobbyEntityState
        {
            ClientId = player.OwnerClientId,
            IsHost = IsHost && player.OwnerClientId == NetworkManager.Singleton.LocalClientId
        });
    }

    void HandlePlayerDespawn(VRPlayerBehaviour player)
    {
        if (m_lobbyEntities == null) return;

        foreach (var entity in m_lobbyEntities)
        {
            if (entity.ClientId != player.OwnerClientId) continue;
            m_lobbyEntities.Remove(entity);
            break;
        }
    }

    void HandleLobbyEntitiesChanged(NetworkListEvent<LobbyEntityState> changeEvent)
    {
        if (!gameObject.scene.isLoaded) return;

        switch (changeEvent.Type)
        {
            case NetworkListEvent<LobbyEntityState>.EventType.Add:
                if (!m_lobbyEntries.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                    var entry = Instantiate(m_lobbyEntryPrefab, m_content);
                    entry.Initialise(changeEvent.Value.ClientId,
                        NetworkManager.Singleton.LocalClientId == changeEvent.Value.ClientId,
                        changeEvent.Value.IsHost);
                    m_lobbyEntries.Add(entry);
                    entry.SetName("Player " + changeEvent.Value.ClientId);

                    if (IsHost)
                    {
                        entry.SetKickButtonVisibility(NetworkManager.Singleton.LocalClientId != changeEvent.Value.ClientId);
                        entry.AddListener(() => NetworkManager.Singleton.DisconnectClient(entry.ClientId));
                    }
                }
                break;
            case NetworkListEvent<LobbyEntityState>.EventType.Remove:
                var entryToRemove = m_lobbyEntries.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (entryToRemove != null)
                {
                    entryToRemove.transform.SetParent(null);
                    Destroy(entryToRemove.gameObject);
                    m_lobbyEntries.Remove(entryToRemove);
                }
                break;
            default:
                break;
        }

        m_lobbyEntries.Sort((x, y) => y.ClientId.CompareTo(x.ClientId));

        for (int i = 0; i < m_lobbyEntries.Count; i++)
        {
            m_lobbyEntries[i].transform.SetSiblingIndex(i);
        }
    }
}
