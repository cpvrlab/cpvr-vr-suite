using Serializable;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Network {
    /// <summary>
    /// Get the data from the XRRig Teleportation RayInteractors
    /// </summary>
    public class NetworkTeleportationProvider : TeleportationProvider {
        // The current teleportation ray if there is one
        private LineRenderer _currentRayRenderer;

        // Whether or not we are teleporting only locally
        // This value will be overriden by the GroupedTeleportationManager.localTeleportation if present
        [SerializeField]
        public bool localTeleportation = true;

        private void Start() {
            if (!RigManager.Instance.RigOrchestrator.TryGetInteractorManager<HandManager>(out var handManager))
                return;

            handManager.OnInteractionStarted += OnInteractionStarted;
            handManager.OnInteractionEnded += OnInteractionEnded;

            handManager.OnInteractionModeChanged.AddListener(OnInteractionModeChanged);
        }

        protected override void Update() {
            // Send the data of the ray
            if (!localTeleportation) {
                if (_currentRayRenderer != null && NetworkManager.Singleton != null) {
                    Vector3[] positionArray = new Vector3[_currentRayRenderer.positionCount];
                    _currentRayRenderer.GetPositions(positionArray);
            
                    GroupedTeleportationManager.Instance.SetPositionsData(new PositionsData {
                        Positions = positionArray
                    });
                }
            }
            
            // We save the position of the XROrigin.
            Vector3 xrOriginPosition = system.xrOrigin.transform.position;
            
            // We let the TeleportationProvider do his work.
            base.Update();

            // If we are teleporting locally then we don't do anything.
            if (localTeleportation) return;
            
            if (locomotionPhase != LocomotionPhase.Done) return;
        
            // If we have teleported inside the base.Update() we revert
            system.xrOrigin.transform.position = xrOriginPosition;
            
            // We tell the TeleportationManager to start a teleportation.
            GroupedTeleportationManager.Instance.ReleaseOwnership(true);
        }

        /// <summary>
        /// When an interaction started. 
        /// </summary>
        /// <param name="interactorObject">Interactor that started an interaction.</param>
        private void OnInteractionStarted(GameObject interactorObject) {
            if (localTeleportation) return;
            
            if (!interactorObject.name.Contains("Teleport"))
                return;
            
            if (NetworkManager.Singleton == null)
                return;

            _currentRayRenderer = interactorObject.GetComponent<LineRenderer>();

            GroupedTeleportationManager.Instance.ClaimOwnership();
        }

        /// <summary>
        /// When an interaction ended without teleportation. 
        /// </summary>
        /// <param name="interactorObject">Interactor that ended an interaction.</param>
        private void OnInteractionEnded(GameObject interactorObject) {
            if (localTeleportation) return;
            
            if (!interactorObject.name.Contains("Teleport"))
                return;
            
            if (NetworkManager.Singleton == null)
                return;

            _currentRayRenderer = null;
            
            GroupedTeleportationManager.Instance.ReleaseOwnership(false);
        }

        /// <summary>
        /// When the interaction mode changed
        /// </summary>
        private void OnInteractionModeChanged(InteractionMode _) {
            _currentRayRenderer = null;
            
            GroupedTeleportationManager.Instance.ReleaseOwnership(false);
        }
    }
}
