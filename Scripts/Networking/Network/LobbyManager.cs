using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.UI;
using Util;

namespace Network
{
    /// <summary>
    /// Manage the Lobby.
    /// </summary>
    public class LobbyManager : NetworkBehaviour
    {
        public string firstSceneName = "BasicScene";

        public TMP_Text gameCodeLabel;
        public TMP_Text playersLabel;
        public Button startButton;

        private readonly NetworkVariable<FixedString32Bytes> _gameCodeLabelValue = new("Game Code");
        private readonly NetworkVariable<FixedString32Bytes> _playersLabelValue = new("Players");

        public override void OnNetworkSpawn()
        {
            gameCodeLabel.text = _gameCodeLabelValue.Value.ToString();
            playersLabel.text = _playersLabelValue.Value.ToString();

            _gameCodeLabelValue.OnValueChanged += (_, newValue) =>
            {
                gameCodeLabel.text = newValue.ToString();
            };

            _playersLabelValue.OnValueChanged += (_, newValue) =>
            {
                playersLabel.text = newValue.ToString();
            };

            if (!IsHost)
            {
                startButton.GetComponentInChildren<TMP_Text>().text = "Waiting on the host";
                startButton.interactable = false;
                return;
            }

            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.OnClientConnectedCallback += OnClientDisconnected;

            // Display Game Code
            string gameCode = NetworkUtil.GetLocalIpAddress().Split(".").Last();
            _gameCodeLabelValue.Value = "Game Code: " + gameCode;

            UpdatePlayerNumber();
        }

        private void OnClientConnected(ulong _)
        {
            UpdatePlayerNumber();
        }

        private void OnClientDisconnected(ulong _)
        {
            UpdatePlayerNumber();
        }

        private void UpdatePlayerNumber()
        {
            int clientNumber = NetworkManager.ConnectedClients.Count;

            string player = clientNumber > 1 ? "players" : "player";

            _playersLabelValue.Value = clientNumber + " " + player + " connected.";
        }

        /// <summary>
        /// Only startable by the host using the startButton.
        /// </summary>
        public void StartGame()
        {
            SendGameStartMessageRpc();

            MyNetworkSceneManager.Instance.GameStartRpc();
            MyNetworkSceneManager.Instance.LoadScene(firstSceneName);

            GroupedTeleportationManager.Instance.GameStartRpc();
        }

        [Rpc(SendTo.Server, RequireOwnership = true)]
        private void SendGameStartMessageRpc()
        {
            var messageContent = new ForceNetworkSerializeByMemcpy<bool>(true);
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.Singleton.CustomMessagingManager;
            using (writer)
            {
                writer.WriteValueSafe(messageContent);
                customMessagingManager.SendNamedMessageToAll("GameStartMessage", writer);
            }
        }
    }
}
