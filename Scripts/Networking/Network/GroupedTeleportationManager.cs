using System;
using System.Collections.Generic;
using System.Linq;
using Serializable;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

namespace Network {
    /// <summary>
    /// Everything related to the network side of teleportation management.
    /// Gesture and local side is managed by NetworkTeleportationProvider.cs.
    /// </summary>
    public class GroupedTeleportationManager : NetworkSingleton<GroupedTeleportationManager> {
        [SerializeField] private bool blockTeleportOnSpawn = true;

        // If we are teleporting locally.
        // Should be 'true' if no calibration is used.
        [SerializeField] private bool localTeleportation;

        [SerializeField] private Toggle localTeleportationToggle;

        // The teleport ray shared between players.
        [SerializeField] private LineRenderer lineRenderer;
        
        // The NetworkTeleportationProvider that gets the input from the rig.
        private NetworkTeleportationProvider _networkTeleportationProvider;

        // If the game has not started yet we block the teleportation.
        private readonly NetworkVariable<bool> _gameStarted = new();
        
        // If someone is currently teleporting.
        private readonly NetworkVariable<bool> _owned = new ();

        // Who is currently teleporting.
        private readonly NetworkVariable<ulong> _ownerId = new (ulong.MaxValue);
        
        // Points that define the ray (Sync across network)
        private readonly NetworkVariable<PositionsData> _positionsData = new (new PositionsData {
            Positions = Array.Empty<Vector3>()
        });

        // Calibration Data
        Vector3[] _markers = new Vector3[2];

        /*
         * STARTS METHODS 
         */

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();

            RigManager.Instance.RigOrchestrator.BlockTeleport(blockTeleportOnSpawn);

            ComponentLocatorUtility<NetworkTeleportationProvider>.TryFindComponent(out _networkTeleportationProvider);
            _networkTeleportationProvider.localTeleportation = false;
            
            localTeleportationToggle.interactable = true;
            localTeleportationToggle.isOn = localTeleportation;

            _ownerId.OnValueChanged += (_, newOwnerId) => {
                // Disable the LineRenderer for the owner and enable/disable the renderer for the others
                lineRenderer.enabled = newOwnerId != NetworkManager.Singleton.LocalClientId && 
                                       newOwnerId != ulong.MaxValue;
            };
            
            _owned.OnValueChanged += (_, newValue) => {
                RigManager.Instance.RigOrchestrator.BlockTeleport(newValue);
            };

            // Update the line renderer points
            _positionsData.OnValueChanged += (_, newValue) => UpdateLineRenderer(newValue.Positions);
            
            // On network spawn, subscribe to TeleportOccuredMessage
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("TeleportPositionMessage",
                ReceiveTeleportPosition);

            NetworkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        }
        
        /*
         * TELEPORTATION MOVEMENTS METHODS
         */ 

        /// <summary>
        /// Message handler for TeleportOccured message.
        /// </summary>
        /// <param name="senderId">Not used.</param>
        /// <param name="messagePayload">Where to teleport.</param>
        private void ReceiveTeleportPosition(ulong senderId, FastBufferReader messagePayload) {
            messagePayload.ReadValueSafe(out ForceNetworkSerializeByMemcpy<Vector3> teleportPosition);
            
            SetNewPosition(teleportPosition, !localTeleportation);
        }

        /// <summary>
        /// Place the players at (0,0,0) on scene changed.
        /// </summary>
        private void OnLoadEventCompleted(String _, LoadSceneMode __, List<ulong> ___, List<ulong> ____) {
            Debug.Log("OnLoadEventCompleted()");
            SetNewPosition(Vector3.zero, true);
        }

        public void SetMarkers(Vector3[] markers) {
            _markers = markers;
        }

        /// <summary>
        /// Set the new position of the object.
        /// </summary>
        /// <param name="position">The new object position.</param>
        /// <param name="withRecenter">If we should place the player correctly while taking care of calibration.</param>
        private void SetNewPosition(Vector3 position, bool withRecenter) {
            Debug.Log("SetNewPosition()");
            transform.position = position;
            
            if(withRecenter)
                RecenterXROrigin();
        }
        
        public void RecenterXROrigin() {
            Debug.Log("RecenterXROrigin()");
            Vector3 localOrigin = _markers[0];
            Vector3 localForward = (_markers[1] - _markers[0]).normalized;
            float angle = Vector3.SignedAngle(localForward, Vector3.forward, Vector3.up);
            
            Transform xrOriginTransform = XROriginRigReferences.Instance.Origin;
            
            xrOriginTransform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            xrOriginTransform.position = transform.position;
            xrOriginTransform.position = xrOriginTransform.TransformPoint(-localOrigin);
        }

        /// <summary>
        /// Update the teleport ray with new positions.
        /// </summary>
        /// <param name="positions">The new positions of the ray.</param>
        private void UpdateLineRenderer(Vector3[] positions) {
            if (!lineRenderer.enabled) return;
            
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }

        public bool IsOwned() {
            return _owned.Value;
        }

        /// <summary>
        /// Enable/Disable local teleporting.
        /// </summary>
        /// <param name="status">If we are teleporting locally or not.</param>
        public void SetLocalTeleportation(bool status) {
            localTeleportation = status;
            _networkTeleportationProvider.localTeleportation = status;
            if (!status) {
                RecenterXROrigin();
            }
        }
        
        /// <summary>
        /// Ask for the teleportation ray.
        /// </summary>
        public void ClaimOwnership() {
            if (!_gameStarted.Value) return;
            
            if (_owned.Value) return;
            
            ClaimOwnershipRpc();
        }

        /// <summary>
        /// Release the ray and teleport or not.
        /// </summary>
        /// <param name="valid">If the teleportation is valid and thus, should be done or not.</param>
        public void ReleaseOwnership(bool valid) {
            if (!_owned.Value || _ownerId.Value != NetworkManager.Singleton.LocalClientId) return;

            ReleaseOwnershipRpc(valid);
        }

        /// <summary>
        /// Set the positions of the ray.
        /// </summary>
        /// <param name="data">New positions.</param>
        public void SetPositionsData(PositionsData data) {
            if (!_owned.Value || NetworkManager.Singleton.LocalClientId != _ownerId.Value) return;
            
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
        private void ClaimOwnershipRpc(RpcParams rpcParams = default) {
            if (_owned.Value) return;

            _ownerId.Value = rpcParams.Receive.SenderClientId;
            _owned.Value = true;
        }
        
        /// <summary>
        /// Set the new positions of the ray for everyone.
        /// </summary>
        /// <param name="data">New positions of the ray.</param>
        /// <param name="rpcParams">Contains the ClientID of who is trying to set the new position.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void SetPositionsDataRpc(PositionsData data, RpcParams rpcParams = default) {
            // if the ray is not currently owned or if the clientId trying to set the new positions is not the owner of the ray we cancel.
            if (!_owned.Value || rpcParams.Receive.SenderClientId != _ownerId.Value) return;
            
            _positionsData.Value = data;
        }
        
        /// <summary>
        /// Release the ray and teleport if needed.
        /// </summary>
        /// <param name="valid">If the teleportation is valid and thus, should be done or not.</param>
        /// <param name="rpcParams">Contains the ClientID of who is trying to teleport.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void ReleaseOwnershipRpc(bool valid, RpcParams rpcParams = default) {
            // if the ray is not currently owned or if the clientId trying to set the new positions is not the owner of the ray we cancel.
            if (!_owned.Value || _ownerId.Value != rpcParams.Receive.SenderClientId) return;

            // We reset the ownership of the ray.
            _ownerId.Value = ulong.MaxValue;
            _owned.Value = false;

            if (!valid) return;

            // We send the teleport position.
            Vector3 teleportPosition = _positionsData.Value.Positions.Last();

            SendTeleportPositionRpc(teleportPosition);
        }
        

        /// <summary>
        /// Send the teleportation position to all connected player.
        /// </summary>
        /// <param name="teleportPosition">Teleport position.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void SendTeleportPositionRpc(Vector3 teleportPosition) {
            var messageContent = new ForceNetworkSerializeByMemcpy<Vector3>(teleportPosition);
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer) {
                writer.WriteValueSafe(messageContent);
                customMessagingManager.SendNamedMessageToAll("TeleportPositionMessage", writer);
            }
        }
        
        [Rpc(SendTo.Server)]
        public void GameStartRpc() {
            _gameStarted.Value = true;
        }
    }
}