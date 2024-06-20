using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPanel : MenuPanel
{
    [SerializeField] GameObject m_mainContent;
    [SerializeField] GameObject m_lobbyContent;
    [SerializeField] TMP_Text m_title;
    [SerializeField] Button m_hostButton;
    [SerializeField] Toggle m_lanToggle;
    [SerializeField] TMP_InputField m_joincodeInputField;
    [SerializeField] Button m_clientButton;
    [SerializeField] TMP_Text m_infoText;

    void Awake()
    {
        // Setup UI Elements
        m_hostButton.onClick.AddListener(StartHost);

        m_lanToggle.isOn = true;
        m_lanToggle.interactable = false;

        m_clientButton.onClick.AddListener(StartClient);

        UpdateInfoText(string.Empty);
    }

    void OnEnable()
    {
        // Check whether the player is already in a lobby or not
        m_mainContent.SetActive(true);
        m_lobbyContent.SetActive(false);
        m_title.text = "Multiplayer";
        UpdateInfoText(string.Empty);
    }

    void StartHost()
    {
        UpdateInfoText("Starthost clicked!");
    }

    void StartClient()
    {
        UpdateInfoText("StartClient clicked!");
    }

    void UpdateInfoText(string content) => m_infoText.text = content;
}
