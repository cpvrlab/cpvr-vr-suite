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
        bool m_inSession;

        [field: SerializeField] public bool LocalTeleportation { get; set; } = true;

        void Start()
        {
            NetworkController.OnNetworkSessionStarted += () => m_inSession = true;
            NetworkController.OnNetworkSessionEnded += () => m_inSession = false;
        }

        protected override void Update()
        {
            if (!LocalTeleportation && 
                m_inSession &&
                m_currentRayRenderer != null)
            {
                var positionArray = new Vector3[m_currentRayRenderer.positionCount];
                m_currentRayRenderer.GetPositions(positionArray);

                NetworkController.Instance.GroupedTeleportationManager.SetPositionsData(new PositionsData
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
            NetworkController.Instance.GroupedTeleportationManager.ReleaseOwnership(true);
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

            m_currentRayRenderer = interactorObject.GetComponent<LineRenderer>();

            NetworkController.Instance.GroupedTeleportationManager.ClaimOwnership();
        }

        /// <summary>
        /// When an interaction ended without teleportation. 
        /// </summary>
        /// <param name="interactorObject">Interactor that ended an interaction.</param>
        void OnInteractionEnded(GameObject interactorObject)
        {
            if (LocalTeleportation) return;

            if (!interactorObject.name.Contains("Teleport"))
                return;

            if (NetworkManager.Singleton == null)
                return;

            m_currentRayRenderer = null;

            NetworkController.Instance.GroupedTeleportationManager.ReleaseOwnership(false);
        }

        void OnInteractionModeChanged()
        {
            m_currentRayRenderer = null;

            if (NetworkController.Instance.GroupedTeleportationManager != null)
                NetworkController.Instance.GroupedTeleportationManager.ReleaseOwnership(false);
        }
    }
}
