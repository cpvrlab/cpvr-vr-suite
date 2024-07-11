using System;
using System.Linq;
using cpvr_vr_suite.Scripts.VR;
using Network;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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
    [SerializeField] GroupedTeleportationManager m_groupedTeleportationManagerPrefab;

    [Header("Lobby Content")]
    [SerializeField] GameObject m_lobbyContent;
    [SerializeField] Lobby m_lobby;
    [SerializeField] CalibrationManager m_calibrationManager;
    [SerializeField] TMP_Text m_joincodeText;
    [SerializeField] Button m_calibrationButton;
    [SerializeField] Button m_exitButton;
    [SerializeField] Toggle m_localTeleportToggle;

    bool m_isCalibrating;
    bool m_isConnected;
    bool m_connecting;

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

        m_localTeleportToggle.isOn = false;
        m_localTeleportToggle.onValueChanged.AddListener(ToggleLocalTeleport);
    }

    protected override void Start()
    {
        base.Start();

        if (m_isConnected)
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

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientStopped += OnClientStopped;
    }

    void FixedUpdate()
    {
        ConnectingStateChange();
    }

    void StartHost()
    {
        UpdateInfoText("Starthost clicked!");
        if (NetworkManager.Singleton.StartHost())
        {
            m_mainContent.SetActive(false);
            m_lobbyContent.SetActive(true);
            var ip = NetworkUtil.GetLocalIpAddress();
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = ip;
            m_isConnected = true;

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
        if (!int.TryParse(m_joincodeInputField.text, out var number) || number <= 0 || number >= 256)
        {
            UpdateInfoText("Game Code '" + number + "' invalid.");
            return;
        }

        SetIpAddress(number.ToString());
        if (NetworkManager.Singleton.StartClient())
            UpdateInfoText("Starting client");
        else
            UpdateInfoText("Failed to join session.");
    }

    void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
        if (GroupedTeleportationManager.Instance != null)
        {
            m_localTeleportToggle.onValueChanged.RemoveAllListeners();
            Destroy(GroupedTeleportationManager.Instance.gameObject);
        }
        m_mainContent.SetActive(true);
        m_lobbyContent.SetActive(false);
        m_title.text = "Multiplayer";
        UpdateInfoText(string.Empty);
    }

    void Calibrate()
    {
        m_isCalibrating = !m_isCalibrating;
        var buttonText = m_calibrationButton.transform.GetChild(0).GetComponent<TMP_Text>();

        if (m_isCalibrating)
            buttonText.text = "Finish calibration";
        else
            buttonText.text = "Calibrate";

        if (RigManager.Instance.RigOrchestrator.TryGetInteractorManager(out HandManager handManager))
            handManager.InteractionModeLocked = m_isCalibrating;

        m_calibrationManager.Calibrate();
    }

    void ToggleLocalTeleport(bool value)
    {
        if (GroupedTeleportationManager.Instance == null) return;
        GroupedTeleportationManager.Instance.SetLocalTeleportation(value);
    }
    
    void UpdateInfoText(string content) => m_infoText.text = content;

    void SetIpAddress(string gameCode)
    {
        // Get our ip and format it to the subnet.
        string currentIP = NetworkUtil.GetLocalIpAddress();
        string[] numberArray = currentIP.Split(".");
        string subnet = string.Join(".", numberArray.Take(numberArray.Length - 1).ToArray());

        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.ConnectionData.Address = subnet + "." + gameCode;
    }

    public void SetJoincode(string content) => m_joincodeText.text = "Lobby Code: " + content;

    void OnClientConnected(ulong obj)
    {
        if (obj != NetworkManager.Singleton.LocalClientId) return;
        
        m_isConnected = true;
        m_lobbyContent.SetActive(true);
        m_mainContent.SetActive(false);
    }

    void OnClientStopped(bool obj)
    {
        m_isConnected = false;
        m_lobbyContent.SetActive(false);
        m_mainContent.SetActive(true);
    }

    void ConnectingStateChange()
    {
        bool newState = NetworkManager.Singleton.IsListening;

        if (m_connecting == newState) return;

        m_connecting = newState;

        m_clientButton.interactable = !m_connecting;
        m_clientButton.interactable = !m_connecting;

        UpdateInfoText(m_connecting ? "Connecting..." : "Connection failed.");
    }
}
