using System;
using Unity.Netcode;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    public class VRPlayerBehaviour : NetworkBehaviour
    {
        public static event Action<VRPlayerBehaviour> OnPlayerSpawned;
        public static event Action<VRPlayerBehaviour> OnPlayerDespawned;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                OnPlayerSpawned?.Invoke(this);
            }

            if (IsOwner)
            {
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("GameStartMessage",
                    ReceiveGameStart);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
                OnPlayerDespawned?.Invoke(this);
        }

        /// <summary>
        /// Used to unblock the teleport mode on game start.
        /// </summary>
        /// <param name="senderId">Who send it (here not used).</param>
        /// <param name="messagePayload">Content of the message (here empty and not needed).</param>
        void ReceiveGameStart(ulong senderId, FastBufferReader messagePayload)
        {
            messagePayload.ReadValueSafe(out ForceNetworkSerializeByMemcpy<bool> _);
        }
    }
}