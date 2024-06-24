using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyEntry : MonoBehaviour
{
    [SerializeField] TMP_Text m_playernameText;
    [SerializeField] Button m_kickButton;
    
    ulong m_clientId;
    bool m_isSelf;
    bool m_isHost;

    public void Initialise(ulong clientId, bool isSelf = false, bool isHost = false)
    {
        m_clientId = clientId;
        m_isSelf = isSelf;
        m_isHost = isHost;

        // TODO: Setup Kick button callback
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
}
