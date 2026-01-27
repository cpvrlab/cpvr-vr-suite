using System;
using System.Collections.Generic;
using System.Linq;
using cpvr_vr_suite.Scripts.Runtime.Core;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace cpvr_vr_suite.Scripts.Runtime.Network
{
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

            LocalTeleportation = false;
            m_networkTeleportationProvider = RigManager.Instance.RigOrchestrator.NetworkTeleportationProvider;
            m_networkTeleportationProvider.GroupedTeleportationManager = this;
            m_networkTeleportationProvider.locomotionEnded += (_) => StartTeleportation();

            m_ownerId.OnValueChanged += (_, newOwnerId) =>
            {
                Debug.Log($"Ray owner id: {newOwnerId}");
                var value = newOwnerId != NetworkManager.Singleton.LocalClientId && newOwnerId != ulong.MaxValue;
                m_lineRenderer.enabled = value;
                RigManager.Instance.RigOrchestrator.BlockTeleport(value);
                Debug.Log($"Teleport allowed: {!value}");
            };

            m_positionsData.OnValueChanged += (_, newValue) => UpdateLineRenderer(newValue.Positions);

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("TeleportPositionMessage",
                ReceiveTeleportPosition);

            NetworkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        }

        public override void OnNetworkDespawn()
        {
            m_networkTeleportationProvider.GroupedTeleportationManager = null;
            m_networkTeleportationProvider.locomotionEnded -= (_) => StartTeleportation();
            NetworkManager.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
        }

        public bool OwnsTeleportRay() => m_ownerId.Value == NetworkManager.Singleton.LocalClientId;

        void ReceiveTeleportPosition(ulong senderId, FastBufferReader messagePayload)
        {
            messagePayload.ReadValueSafe(out ForceNetworkSerializeByMemcpy<Vector3> teleportPosition);

            SetNewPosition(teleportPosition, !LocalTeleportation);
        }

        void OnLoadEventCompleted(string _, LoadSceneMode __, List<ulong> ___, List<ulong> ____)
        {
            //Debug.Log("OnLoadEventCompleted()");
            SetNewPosition(Vector3.zero, true);
        }

        public void SetMarkers(Vector3[] markers) => m_markers = markers;

        void SetNewPosition(Vector3 position, bool withRecenter)
        {
            Debug.Log($"Setting new position to: {position}");
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

        void UpdateLineRenderer(Vector3[] positions)
        {
            if (!m_lineRenderer.enabled) return;

            m_lineRenderer.positionCount = positions.Length;
            m_lineRenderer.SetPositions(positions);
        }

        public void SetLocalTeleportation(bool status)
        {
            LocalTeleportation = status;
            if (!status)
                RecenterXROrigin();
        }

        public bool ClaimOwnership()
        {
            if (m_ownerId.Value == ulong.MaxValue)
            {
                ClaimOwnershipRpc();
                return true;
            }
            else return false;
        }

        public void ReleaseOwnership()
        {
            if (OwnsTeleportRay())
                ReleaseOwnershipRpc();
        }

        public void StartTeleportation()
        {
            if (OwnsTeleportRay() &&
                m_positionsData.Value.Positions.Count() > 0)
            {
                var position = m_positionsData.Value.Positions.Last() + GetOffset();
                SendTeleportPositionRpc(position);
            }
        }

        public void SetPositionsData(PositionsData data)
        {
            if (OwnsTeleportRay())
                SetPositionsDataRpc(data);
        }

        Vector3 GetOffset()
        {
            var pos = transform.position;
            var camera = RigManager.Instance.RigOrchestrator.Camera.transform.position;
            camera.y = pos.y;
            return pos - camera;
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        void ClaimOwnershipRpc(RpcParams rpcParams = default)
        {
            if (m_ownerId.Value == ulong.MaxValue)
                m_ownerId.Value = rpcParams.Receive.SenderClientId;
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        void SetPositionsDataRpc(PositionsData data, RpcParams rpcParams = default)
        {
            if (m_ownerId.Value == rpcParams.Receive.SenderClientId)
                m_positionsData.Value = data;
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        void ReleaseOwnershipRpc(RpcParams rpcParams = default)
        {
            if (m_ownerId.Value == rpcParams.Receive.SenderClientId)
                m_ownerId.Value = ulong.MaxValue;
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
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