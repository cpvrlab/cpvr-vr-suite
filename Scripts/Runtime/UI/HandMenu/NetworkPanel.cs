using System.Collections;
using System.Linq;
using cpvr_vr_suite.Scripts.VR;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class NetworkPanel : MonoBehaviour
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
    [SerializeField] Lobby m_lobby;
    [SerializeField] CalibrationManager m_calibrationManager;
    [SerializeField] TMP_Text m_joincodeText;
    [SerializeField] Button m_calibrationButton;
    [SerializeField] Button m_exitButton;
    [SerializeField] Toggle m_localTeleportToggle;

    bool m_isCalibrating;
    string m_joincode;

    void Awake()
    {
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

    void OnEnable()
    {
        if (NetworkManager.Singleton.IsConnectedClient)
            ClientConnected();
        else
            ClientDisconnected();

        NetworkController.OnNetworkSessionStarted += ClientConnected;
        NetworkController.OnNetworkSessionEnded += ClientDisconnected;
    }

    void OnDisable()
    {
        NetworkController.OnNetworkSessionStarted -= ClientConnected;
        NetworkController.OnNetworkSessionEnded -= ClientDisconnected;
    }

    void StartHost()
    {
        var ip = NetworkUtil.GetLocalIpAddress();
        if (m_lanToggle.isOn)
            m_joincode = ip.Split(".").Last();
            
        StartCoroutine(SuspendInteraction());

        if (NetworkManager.Singleton.StartHost())
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = ip;
        }
        else
        {
            UpdateInfoText("Failed to start session as host.");
        }
    }

    void StartClient()
    {
        StartCoroutine(SuspendInteraction());

        if (m_joincodeInputField.text == string.Empty)
        {
            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.ConnectionData.Address = "127.0.0.1";
            if (NetworkManager.Singleton.StartClient())
            {
                m_joincode = "localhost";
                UpdateInfoText("Starting client");
            }
            else
                UpdateInfoText("Failed to join session.");
            return;
        }
        if (!int.TryParse(m_joincodeInputField.text, out var number) || number <= 0 || number >= 256)
        {
            UpdateInfoText("Game Code '" + number + "' invalid.");
            return;
        }

        SetIpAddress(number.ToString());
        m_joincode = number.ToString();
        if (NetworkManager.Singleton.StartClient())
            UpdateInfoText("Starting client");
        else
            UpdateInfoText("Failed to join session.");
    }

    void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
        ClientDisconnected();
    }

    void Calibrate()
    {
        m_isCalibrating = !m_isCalibrating;
        var buttonText = m_calibrationButton.transform.GetChild(0).GetComponent<TMP_Text>();

        if (m_isCalibrating)
            buttonText.text = "Finish calibration";
        else
            buttonText.text = "Calibrate";

        RigManager.Instance.RigOrchestrator.BlockTeleport(m_isCalibrating);

        m_calibrationManager.Calibrate();
    }

    void ToggleLocalTeleport(bool value)
    {
        if (NetworkController.Instance.GroupedTeleportationManager == null) return;
        NetworkController.Instance.GroupedTeleportationManager.SetLocalTeleportation(value);
    }

    void UpdateInfoText(string content) => m_infoText.text = content;

    void SetIpAddress(string gameCode)
    {
        // Get our ip and format it to the subnet.
        string currentIP = NetworkUtil.GetLocalIpAddress();
        string[] numberArray = currentIP.Split(".");
        string subnet = string.Join(".", numberArray.Take(numberArray.Length - 1).ToArray());

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.ConnectionData.Address = subnet + "." + gameCode;
    }

    public void SetJoincode(string content) => m_joincodeText.text = "Lobby Code: " + content;

    void ClientConnected()
    {
        SetJoincode(m_joincode);
        m_lobbyContent.SetActive(true);
        m_mainContent.SetActive(false);
        m_title.text = "Lobby";
    }

    void ClientDisconnected()
    {
        UpdateInfoText(NetworkManager.Singleton.DisconnectReason);
        SetJoincode(string.Empty);
        m_lobbyContent.SetActive(false);
        m_mainContent.SetActive(true);
        m_title.text = "Multiplayer";
    }

    IEnumerator SuspendInteraction()
    {
        m_clientButton.interactable = false;
        m_hostButton.interactable = false;
        yield return new WaitForSeconds(0.5f);

        while (NetworkManager.Singleton.IsListening && !NetworkManager.Singleton.IsConnectedClient)
            yield return null;

        UpdateInfoText("Failed to start or join a session.");

        m_clientButton.interactable = true;
        m_hostButton.interactable = true;
    }
}
