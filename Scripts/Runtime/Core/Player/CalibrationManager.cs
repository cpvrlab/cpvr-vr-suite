using Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Util;
using VR;


namespace cpvr_vr_suite.Scripts.VR
{
    public class CalibrationManager : MonoBehaviour
    {
        [SerializeField] PlaneInteractable m_planeInteractable;
        bool m_isCalibrating;

        public void Calibrate()
        {
            m_isCalibrating = !m_isCalibrating;

            if (RigManager.Instance.RigOrchestrator.TryGetInteractorManager(out HandManager handManager))
                handManager.InteractionModeLocked = m_isCalibrating;

            var passthrough = RigManager.Instance.RigOrchestrator.Camera.GetComponent<Passthrough>();
            passthrough.ActivePassthrough(m_isCalibrating, !m_isCalibrating);

            SetInteractables(!m_isCalibrating);

            m_planeInteractable.Active(m_isCalibrating);
            m_planeInteractable.SetMarkersVisibility(m_isCalibrating);

            if (!m_isCalibrating)
                SaveCalibration();
        }

        void SaveCalibration()
        {
            Vector3[] coordinates = m_planeInteractable.GetMarkerPositions();

            MarkerPrefs.SavePrefs(coordinates[0], coordinates[1]);

            GroupedTeleportationManager.Instance.SetMarkers(coordinates);
            GroupedTeleportationManager.Instance.RecenterXROrigin();
        }

        void SetInteractables(bool value)
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.TryGetComponent<VRPlayerBehaviour>(out _)) continue;
                if (root.TryGetComponent<RigManager>(out _)) continue;
                foreach (var interactable in root.GetComponentsInChildren<XRBaseInteractable>())
                    interactable.enabled = value;
                foreach (var snapVolume in root.GetComponentsInChildren<XRInteractableSnapVolume>())
                    snapVolume.enabled = value;
            }
        }
    }
}

