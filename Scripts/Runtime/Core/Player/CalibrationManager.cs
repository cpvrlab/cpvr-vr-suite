using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using VR;

namespace cpvr_vr_suite.Scripts.VR
{
    public class CalibrationManager : MonoBehaviour
    {
        [SerializeField] Collider m_planeCollider;
        [SerializeField] GameObject m_marker;
        bool m_isCalibrating;

        void Start()
        {
            m_isCalibrating = false;
            m_marker.SetActive(false);

            if (MarkerPrefs.LoadPosition(out var pos1))
                m_marker.transform.localPosition = pos1;
        }

        public void Calibrate()
        {
            m_isCalibrating = !m_isCalibrating;

            var passthrough = RigManager.Instance.RigOrchestrator.Camera.GetComponent<Passthrough>();
            passthrough.ActivePassthrough(m_isCalibrating, !m_isCalibrating);

            SetInteractables(!m_isCalibrating);

            m_marker.gameObject.SetActive(m_isCalibrating);
            m_planeCollider.enabled = m_isCalibrating;

            if (!m_isCalibrating)
                SaveCalibration();
        }

        void SaveCalibration()
        {
            MarkerPrefs.SavePositon(m_marker.transform.localPosition);
            NetworkController.Instance.GroupedTeleportationManager.Marker = m_marker;
            NetworkController.Instance.GroupedTeleportationManager.RecenterXROrigin();
        }

        void SetInteractables(bool value)
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.TryGetComponent<VRPlayerBehaviour>(out _)) continue;
                if (root.TryGetComponent<RigManager>(out _)) continue;
                foreach (var interactable in root.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>())
                    interactable.enabled = value;
                foreach (var snapVolume in root.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRInteractableSnapVolume>())
                    snapVolume.enabled = value;
            }
        }
    }
}

