using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class NetworkPanel : MenuPanel
{
    [Header("Main Content")]
    [SerializeField] GameObject m_mainContent;
    [SerializeField] TMP_Text m_title;
    [SerializeField] Button m_hostButton;
    [SerializeField] Toggle m_lanToggle;
    [SerializeField] TMP_InputField m_joincodeInputField;
    [SerializeField] Button m_clientButton;
    [SerializeField] TMP_Text m_infoText;
    [Header("Lobby Content")]
    [SerializeField] GameObject m_lobbyContent;
    [SerializeField] GameObject m_playerContainer;
    [SerializeField] GameObject m_playerEntryPrefab;
    [SerializeField] TMP_Text m_joincodeText;
    [SerializeField] Button m_calibrationButton;
    [SerializeField] Button m_exitButton;


    void Awake()
    {
        // Setup UI Elements
        m_hostButton.onClick.AddListener(StartHost);

        m_lanToggle.isOn = true;
        m_lanToggle.interactable = false;

        m_clientButton.onClick.AddListener(StartClient);

        UpdateInfoText(string.Empty);

        m_calibrationButton.onClick.AddListener(Calibrate);
        m_exitButton.onClick.AddListener(Shutdown);
    }

    void OnEnable()
    {
        // Check whether the player is already in a lobby or not
        if (NetworkManager.Singleton.IsListening)
        {
            m_mainContent.SetActive(false);
            m_lobbyContent.SetActive(true);
            m_title.text = "Lobby";
        }
        else
        {
            m_mainContent.SetActive(true);
            m_lobbyContent.SetActive(false);
            m_title.text = "Multiplayer";
            UpdateInfoText(string.Empty);
        }
    }

    void StartHost()
    {
        UpdateInfoText("Starthost clicked!");
        if (NetworkManager.Singleton.StartHost())
        {
            m_mainContent.SetActive(false);
            m_lobbyContent.SetActive(true);

            if (m_lanToggle.isOn)
            {
                m_joincodeText.text = "Joincode: " + NetworkUtil.GetLocalIpAddress().Split(".").Last();
            }
        }
        else
        {
            UpdateInfoText("Failed to start session as host.");
        }
    }

    void StartClient()
    {
        UpdateInfoText("StartClient clicked!");
        if (NetworkManager.Singleton.StartClient())
        {
            m_mainContent.SetActive(false);
            m_lobbyContent.SetActive(true);
        }
        else
        {
            UpdateInfoText("Failed to join session.");
        }
    }

    void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
        m_mainContent.SetActive(true);
        m_lobbyContent.SetActive(false);
        m_title.text = "Multiplayer";
        UpdateInfoText(string.Empty);     
    }

    void Calibrate()
    {

    }

    void UpdateInfoText(string content) => m_infoText.text = content;
}
