using Unity.Netcode;

namespace VR {
    /// <summary>
    /// Basic logic for a network player.
    /// </summary>
    public class VRPlayerBehaviour : NetworkBehaviour {
        public override void OnNetworkSpawn() {
            if (IsOwner) {
                // We subscribe to GameStartMessage
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("GameStartMessage",
                    ReceiveGameStart);
            }
        }
        
        /// <summary>
        /// Used to unblock the teleport mode on game start.
        /// </summary>
        /// <param name="senderId">Who send it (here not used).</param>
        /// <param name="messagePayload">Content of the message (here empty and not needed).</param>
        private void ReceiveGameStart(ulong senderId, FastBufferReader messagePayload) {
            messagePayload.ReadValueSafe(out ForceNetworkSerializeByMemcpy<bool> _);
            
            RigManager.Instance.RigOrchestrator.BlockTeleport(false);
        }
    }
}