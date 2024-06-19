using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

namespace UI
{
    /// <summary>
    /// Manage the game join connection.
    /// </summary>
    public class NetworkMenuPanel : MenuPanel
    {
        // Input field for the IP
        public TMP_InputField gameCodeInputField;

        public string firstScene = "LobbyScene";

        // Buttons to be deactivated during connection
        public Button startClientButton;
        public Button startHostButton;

        bool _connecting;

        void FixedUpdate()
        {
            // Detecting connection in progress.
            ConnectingStateChange();
        }

        /// <summary>
        /// Perform actions on connectionStart and connectionFinished.
        /// </summary>
        void ConnectingStateChange()
        {
            bool newState = NetworkManager.Singleton.IsListening;

            if (_connecting == newState) return;

            _connecting = newState;

            startClientButton.interactable = !_connecting;
            startHostButton.interactable = !_connecting;

            Debug.Log(_connecting ? "Connecting..." : "Connection failed.");
        }

        /// <summary>
        /// Hosting a game.
        /// </summary>
        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            // Change Scene
            NetworkManager.Singleton.SceneManager.LoadScene(firstScene, LoadSceneMode.Single);
        }

        /// <summary>
        /// Joining a game as client.
        /// </summary>
        public void StartClient()
        {
            // Game Code valid?
            if (!int.TryParse(gameCodeInputField.text, out var number) || number <= 0 || number >= 256)
            {
                Debug.Log("Game Code '" + number + "' invalid.");
                return;
            }

            // Set the connection IP Address to be the one from the inputField
            SetIpAddress(number.ToString());

            NetworkManager.Singleton.StartClient();
        }

        /// <summary>
        /// Format the Game Code to an IP using our subnet IP.
        /// 
        ///     OUR IP   : WWW.XXX.YYY.ZZZ
        ///     GAMECODE :             GGG
        ///     LOBBY IP : WWW.XXX.YYY.GGG
        /// 
        /// </summary>
        /// <param name="gameCode">GameCode of the lobby we want to join.</param>
        void SetIpAddress(string gameCode)
        {
            // Get our ip and format it to the subnet.
            string currentIP = NetworkUtil.GetLocalIpAddress();
            string[] numberArray = currentIP.Split(".");
            string subnet = string.Join(".", numberArray.Take(numberArray.Length - 1).ToArray());

            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.ConnectionData.Address = subnet + "." + gameCode;
        }
    }
}