using Serializable;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace Network
{
    public class NetworkTeleportationProvider : TeleportationProvider
    {
        LineRenderer m_currentRayRenderer;
        public GroupedTeleportationManager GroupedTeleportationManager { get; set; }

        [field: SerializeField] public bool LocalTeleportation { get; set; }
        [field: SerializeField] public GameObject LeftHandTeleport { get; private set; }
        bool m_leftPreviousState;
        [field: SerializeField] public GameObject RightHandTeleport { get; private set; }
        bool m_rightPreviousState;

        void Start()
        {
            LocalTeleportation = true;
        }

        protected override void Update()
        {
            if (m_leftPreviousState != LeftHandTeleport.activeSelf)
            {
                if (LeftHandTeleport.activeSelf)
                    OnInteractionStarted(LeftHandTeleport);
                else
                    OnInteractionEnded(LeftHandTeleport);
            }

            if (m_rightPreviousState != RightHandTeleport.activeSelf)
            {
                if (RightHandTeleport.activeSelf)
                    OnInteractionStarted(RightHandTeleport);
                else
                    OnInteractionEnded(RightHandTeleport);
            }

            m_leftPreviousState = LeftHandTeleport.activeSelf;
            m_rightPreviousState = RightHandTeleport.activeSelf;

            if (GroupedTeleportationManager != null &&
                GroupedTeleportationManager.OwnsTeleportRay() &&
                m_currentRayRenderer != null)
            {
                var positionArray = new Vector3[m_currentRayRenderer.positionCount];
                m_currentRayRenderer.GetPositions(positionArray);

                GroupedTeleportationManager.SetPositionsData(new PositionsData
                {
                    Positions = positionArray
                });
            }

            // Let the TeleportationProvider do his work.
            base.Update();

            if (GroupedTeleportationManager != null)
                GroupedTeleportationManager.StartTeleportation();
        }

        public override bool QueueTeleportRequest(TeleportRequest teleportRequest)
        {
            Debug.Log($"Teleport request. {LocalTeleportation || (GroupedTeleportationManager != null && GroupedTeleportationManager.OwnsTeleportRay())}");
            return (LocalTeleportation || (GroupedTeleportationManager != null && GroupedTeleportationManager.OwnsTeleportRay())) && base.QueueTeleportRequest(teleportRequest);
        }

        void OnInteractionStarted(GameObject interactorObject)
        {
            if (LocalTeleportation) return;
            if (!interactorObject.name.Contains("Teleport")) return;
            if (NetworkManager.Singleton == null) return;
            if (GroupedTeleportationManager != null && GroupedTeleportationManager.ClaimOwnership()) return;

            interactorObject.TryGetComponent(out m_currentRayRenderer);
        }

        void OnInteractionEnded(GameObject interactorObject)
        {
            if (LocalTeleportation) return;
            if (!interactorObject.name.Contains("Teleport")) return;
            if (NetworkManager.Singleton == null) return;

            if (GroupedTeleportationManager != null)
                GroupedTeleportationManager.ReleaseOwnership();
        }
    }
}
