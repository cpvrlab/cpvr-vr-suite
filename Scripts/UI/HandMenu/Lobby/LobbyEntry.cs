using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyEntry : MonoBehaviour
{
    [SerializeField] TMP_Text m_playernameText;
    [SerializeField] Button m_kickButton;

    public ulong ClientId { get; private set; }
    bool m_isSelf;
    bool m_isHost;

    public void Initialise(ulong clientId, bool isSelf = false, bool isHost = false)
    {
        ClientId = clientId;
        m_isSelf = isSelf;
        m_isHost = isHost;

        m_kickButton.onClick.AddListener(() => Destroy(gameObject));
    }

    public void SetName(string name)
    {
        string text = "- ";
        if (string.IsNullOrEmpty(name))
            text += "Player";
        else
            text += name;

        if (m_isSelf)
            text += " (You)";
        else if (m_isHost)
            text += " (Host)";

        m_playernameText.text = text;
    }

    public void SetKickButtonVisibility(bool value) => m_kickButton.gameObject.SetActive(value);

    public void AddListener(Action action) => m_kickButton.onClick.AddListener(new UnityAction(action));
}
