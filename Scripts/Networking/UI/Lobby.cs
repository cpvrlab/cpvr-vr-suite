using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    [SerializeField] LobbyEntry m_lobbyEntryPrefab;
    [SerializeField] Transform m_content;

    public LobbyEntry AddEntry(ulong clientId, bool isSelf = false, bool isHost = false)
    {
        var entry = Instantiate(m_lobbyEntryPrefab, m_content);
        entry.Initialise(clientId, isSelf, isHost);
        return entry;
    }
}
