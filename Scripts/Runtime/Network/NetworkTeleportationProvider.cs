using Serializable;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace Network
{
    public class NetworkTeleportationProvider : TeleportationProvider
    {
        // The current teleportation ray if there is one
        LineRenderer m_currentRayRenderer;
        public GroupedTeleportationManager GroupedTeleportationManager { get; set; }

        [field: SerializeField] public bool LocalTeleportation { get; set; } = true;
        [field: SerializeField] public GameObject LeftHandTeleport { get; private set; }
        bool m_leftPreviousState;
        [field: SerializeField] public GameObject RightHandTeleport { get; private set; }
        bool m_rightPreviousState;

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

            if (!LocalTeleportation && 
                GroupedTeleportationManager != null &&
                m_currentRayRenderer != null)
            {
                var positionArray = new Vector3[m_currentRayRenderer.positionCount];
                m_currentRayRenderer.GetPositions(positionArray);

                GroupedTeleportationManager.SetPositionsData(new PositionsData
                {
                    Positions = positionArray
                });
            }

            Vector3 xrOriginPosition = mediator.xrOrigin.transform.position;

            // Let the TeleportationProvider do his work.
            base.Update();

            if (LocalTeleportation) return;
            if (locomotionState != LocomotionState.Ended) return;

            // If we have teleported inside the base.Update() we revert
            mediator.xrOrigin.transform.position = xrOriginPosition;

            // We tell the TeleportationManager to start a teleportation.
            if (GroupedTeleportationManager != null)
                GroupedTeleportationManager.ReleaseOwnership(true);
        }

        /// <summary>
        /// When an interaction started. 
        /// </summary>
        /// <param name="interactorObject">Interactor that started an interaction.</param>
        void OnInteractionStarted(GameObject interactorObject)
        {
            if (LocalTeleportation) return;
            if (!interactorObject.name.Contains("Teleport")) return;
            if (NetworkManager.Singleton == null) return;
            if (GroupedTeleportationManager != null && GroupedTeleportationManager.ClaimOwnership()) return;

            interactorObject.TryGetComponent(out m_currentRayRenderer);
        }

        /// <summary>
        /// When an interaction ended without teleportation. 
        /// </summary>
        /// <param name="interactorObject">Interactor that ended an interaction.</param>
        void OnInteractionEnded(GameObject interactorObject)
        {
            if (LocalTeleportation) return;
            if (!interactorObject.name.Contains("Teleport")) return;
            if (NetworkManager.Singleton == null) return;

            m_currentRayRenderer = null;
            if (GroupedTeleportationManager != null)
                GroupedTeleportationManager.ReleaseOwnership(false);
        }
    }
}
