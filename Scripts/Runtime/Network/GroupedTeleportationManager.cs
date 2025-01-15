using System;
using System.Collections.Generic;
using System.Linq;
using Serializable;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Network
{
    /// <summary>
    /// Everything related to the network side of teleportation management.
    /// Gesture and local side is managed by NetworkTeleportationProvider.cs.
    /// </summary>
    public class GroupedTeleportationManager : NetworkBehaviour
    {
        [field: SerializeField] public bool LocalTeleportation { get; private set; }
        [SerializeField] LineRenderer m_lineRenderer;

        NetworkTeleportationProvider m_networkTeleportationProvider;
        readonly NetworkVariable<ulong> m_ownerId = new(ulong.MaxValue);
        readonly NetworkVariable<PositionsData> m_positionsData = new(new PositionsData
        {
            Positions = Array.Empty<Vector3>()
        });

        Vector3[] m_markers = new Vector3[2];

        public override void OnNetworkSpawn()
        {
            NetworkController.Instance.GroupedTeleportationManager = this;
            
            if (MarkerPrefs.LoadPrefs(out var pos1, out var pos2))
            {
                m_markers[0] = pos1; 
                m_markers[1] = pos2;
                RecenterXROrigin();
            }

            m_networkTeleportationProvider = RigManager.Instance.RigOrchestrator.NetworkTeleportationProvider;
            m_networkTeleportationProvider.LocalTeleportation = false;
            m_networkTeleportationProvider.GroupedTeleportationManager = this;

            m_ownerId.OnValueChanged += (_, newOwnerId) =>
            {
                Debug.Log($"Ray owner id: {newOwnerId}");
                if (newOwnerId == NetworkManager.Singleton.LocalClientId)
                {
                    Debug.Log($"Teleport Ray: I am the owner of the teleport ray");
                    m_lineRenderer.enabled = false;
                    RigManager.Instance.RigOrchestrator.BlockTeleport(false);
                }
                else if (newOwnerId == ulong.MaxValue)
                {
                    Debug.Log($"Teleport Ray: Ownership of the ray has been reset");
                    m_lineRenderer.enabled = false;
                    RigManager.Instance.RigOrchestrator.BlockTeleport(false);
                }
                else
                {
                    Debug.Log($"Teleport Ray: I am not the owner of the ray so i cannot teleport");
                    m_lineRenderer.enabled = true;
                    RigManager.Instance.RigOrchestrator.BlockTeleport(true);
                }
            };

            // Update the line renderer points
            m_positionsData.OnValueChanged += (_, newValue) => UpdateLineRenderer(newValue.Positions);

            // On network spawn, subscribe to TeleportOccuredMessage
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("TeleportPositionMessage",
                ReceiveTeleportPosition);

            NetworkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        }

        public override void OnNetworkDespawn()
        {
            m_networkTeleportationProvider.GroupedTeleportationManager = null;
        }

        void ReceiveTeleportPosition(ulong senderId, FastBufferReader messagePayload)
        {
            messagePayload.ReadValueSafe(out ForceNetworkSerializeByMemcpy<Vector3> teleportPosition);

            SetNewPosition(teleportPosition, !LocalTeleportation);
        }

        void OnLoadEventCompleted(string _, LoadSceneMode __, List<ulong> ___, List<ulong> ____)
        {
            Debug.Log("OnLoadEventCompleted()");
            SetNewPosition(Vector3.zero, true);
        }

        public void SetMarkers(Vector3[] markers) => m_markers = markers;

        /// <summary>
        /// Set the new position of the object.
        /// </summary>
        /// <param name="position">The new object position.</param>
        /// <param name="withRecenter">If we should place the player correctly while taking care of calibration.</param>
        void SetNewPosition(Vector3 position, bool withRecenter)
        {
            transform.position = position;

            if (withRecenter)
                RecenterXROrigin();
        }

        public void RecenterXROrigin()
        {
            Vector3 localOrigin = m_markers[0];
            Vector3 localForward = (m_markers[1] - m_markers[0]).normalized;
            float angle = Vector3.SignedAngle(localForward, Vector3.forward, Vector3.up);

            var xrOriginTransform = RigManager.Instance.RigOrchestrator.Origin;

            xrOriginTransform.SetPositionAndRotation(transform.position, Quaternion.AngleAxis(angle, Vector3.up));
            xrOriginTransform.position = xrOriginTransform.TransformPoint(-localOrigin);
        }

        /// <summary>
        /// Update the teleport ray with new positions.
        /// </summary>
        /// <param name="positions">The new positions of the ray.</param>
        void UpdateLineRenderer(Vector3[] positions)
        {
            if (!m_lineRenderer.enabled) return;

            m_lineRenderer.positionCount = positions.Length;
            m_lineRenderer.SetPositions(positions);
        }

        public void SetLocalTeleportation(bool status)
        {
            LocalTeleportation = status;
            m_networkTeleportationProvider.LocalTeleportation = status;
            if (!status)
                RecenterXROrigin();
        }

        /// <summary>
        /// Ask for the teleportation ray.
        /// </summary>
        public bool ClaimOwnership()
        {
            if (m_ownerId.Value == ulong.MaxValue)
            {
                ClaimOwnershipRpc();
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Release the ray and teleport or not.
        /// </summary>
        /// <param name="valid">If the teleportation is valid and thus, should be done or not.</param>
        public void ReleaseOwnership(bool valid)
        {
            if (m_ownerId.Value == NetworkManager.Singleton.LocalClientId)
            {
                if (m_positionsData.Value.Positions.Count() > 0)
                {
                    var position = m_positionsData.Value.Positions.Last() + GetOffset();
                    ReleaseOwnershipRpc(valid, position);
                }
            }
        }

        /// <summary>
        /// Set the positions of the ray.
        /// </summary>
        /// <param name="data">New positions.</param>
        public void SetPositionsData(PositionsData data)
        {
            if (m_ownerId.Value == NetworkManager.Singleton.LocalClientId)
                SetPositionsDataRpc(data);
        }

        /*
         * RPCs
         * Functions called by the host/server.
         */

        /// <summary>
        /// Ask for the ownership of the ray.
        /// </summary>
        /// <param name="rpcParams">Contains the ClientID of who is asking.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        void ClaimOwnershipRpc(RpcParams rpcParams = default)
        {
            if (m_ownerId.Value == ulong.MaxValue)
                m_ownerId.Value = rpcParams.Receive.SenderClientId;
        }

        /// <summary>
        /// Set the new positions of the ray for everyone.
        /// </summary>
        /// <param name="data">New positions of the ray.</param>
        /// <param name="rpcParams">Contains the ClientID of who is trying to set the new position.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        void SetPositionsDataRpc(PositionsData data, RpcParams rpcParams = default)
        {
            if (m_ownerId.Value == rpcParams.Receive.SenderClientId)
                m_positionsData.Value = data;
        }

        /// <summary>
        /// Release the ray and teleport if needed.
        /// </summary>
        /// <param name="valid">If the teleportation is valid and thus, should be done or not.</param>
        /// <param name="rpcParams">Contains the ClientID of who is trying to teleport.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        void ReleaseOwnershipRpc(bool valid, Vector3 position, RpcParams rpcParams = default)
        {
            if (m_ownerId.Value == rpcParams.Receive.SenderClientId)
            {
                // We reset the ownership of the ray.
                m_ownerId.Value = ulong.MaxValue;

                if (valid)
                    SendTeleportPositionRpc(position);
            }
        }

        Vector3 GetOffset()
        {
            var pos = transform.position;
            var camera = RigManager.Instance.RigOrchestrator.Camera.transform.position;
            camera.y = pos.y;
            return pos - camera;
        }

        /// <summary>
        /// Send the teleportation position to all connected player.
        /// </summary>
        /// <param name="teleportPosition">Teleport position.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        void SendTeleportPositionRpc(Vector3 teleportPosition)
        {
            var messageContent = new ForceNetworkSerializeByMemcpy<Vector3>(teleportPosition);
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                writer.WriteValueSafe(messageContent);
                customMessagingManager.SendNamedMessageToAll("TeleportPositionMessage", writer);
            }
        }
    }
}