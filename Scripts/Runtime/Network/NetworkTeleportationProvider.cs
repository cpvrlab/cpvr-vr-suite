using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace cpvr_vr_suite.Scripts.Runtime.Network
{
    public class NetworkTeleportationProvider : TeleportationProvider
    {
        LineRenderer m_currentRayRenderer;
        public GroupedTeleportationManager GroupedTeleportationManager { get; set; }

        [field: SerializeField] public GameObject LeftHandTeleport { get; private set; }
        bool m_leftPreviousState;
        [field: SerializeField] public GameObject RightHandTeleport { get; private set; }
        bool m_rightPreviousState;

        protected override void Update()
        {
            if (GroupedTeleportationManager != null)
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

                if (GroupedTeleportationManager.OwnsTeleportRay() &&
                    m_currentRayRenderer != null)
                {
                    var positionArray = new Vector3[m_currentRayRenderer.positionCount];
                    m_currentRayRenderer.GetPositions(positionArray);

                    GroupedTeleportationManager.SetPositionsData(new PositionsData
                    {
                        Positions = positionArray
                    });
                }
            }

            base.Update();
        }

        public override bool QueueTeleportRequest(TeleportRequest teleportRequest)
        {
            var condition = GroupedTeleportationManager == null || GroupedTeleportationManager.OwnsTeleportRay() || GroupedTeleportationManager.LocalTeleportation;
            Debug.Log($"Teleport request success: {condition}");
            return condition && base.QueueTeleportRequest(teleportRequest);
        }

        void OnInteractionStarted(GameObject interactorObject)
        {
            if (!interactorObject.name.Contains("Teleport")) return;
            if(!GroupedTeleportationManager.ClaimOwnership()) return;

            interactorObject.TryGetComponent(out m_currentRayRenderer);
        }

        void OnInteractionEnded(GameObject interactorObject)
        {
            if (!interactorObject.name.Contains("Teleport")) return;
            
            GroupedTeleportationManager.ReleaseOwnership();
        }
    }
}
