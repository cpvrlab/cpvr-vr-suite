using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    [SerializeField] LobbyEntry m_lobbyEntryPrefab;
    [SerializeField] Transform m_content;
    readonly List<LobbyEntry> m_lobbyEntries = new();

    public LobbyEntry AddEntry(ulong clientId, bool isSelf = false, bool isHost = false)
    {
        var entry = Instantiate(m_lobbyEntryPrefab, m_content);
        entry.Initialise(clientId, isSelf, isHost);
        m_lobbyEntries.Add(entry);
        Debug.Log($"Entry {entry.gameObject.name} created with parent {entry.transform.parent}");
        return entry;
    }

    public void RemoveEntry(ulong clientId)
    {
        var entry = m_lobbyEntries.FirstOrDefault(e => e.ClientId == clientId);
        if (entry != default)
        {
            m_lobbyEntries.Remove(entry);
            Debug.Log($"Removed entry with id {entry.ClientId}");
            Destroy(entry.gameObject);
        }
    }

    public void RemoveAllEntries()
    {
        foreach (var entry in m_lobbyEntries)
        {
            Debug.Log("Destroying entry with ID: " + entry.ClientId);
            Destroy(entry.gameObject);
        }
        
        m_lobbyEntries.Clear();
    }
}
