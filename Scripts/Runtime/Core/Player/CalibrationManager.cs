using UnityEngine;
using UnityEngine.SceneManagement;

using Util;
using VR;

namespace cpvr_vr_suite.Scripts.VR
{
    public class CalibrationManager : MonoBehaviour
    {
        [SerializeField] Collider m_planeCollider;
        [SerializeField] Transform m_firstMarker;
        [SerializeField] Transform m_secondMarker;

        bool m_isCalibrating;

        void Start()
        {
            m_isCalibrating = false;
            m_firstMarker.gameObject.SetActive(false);
            m_secondMarker.gameObject.SetActive(false);

            if (MarkerPrefs.LoadPrefs(out var pos1, out var pos2))
            {
                m_firstMarker.position = pos1;
                m_secondMarker.position = pos2;
            }
        }

        public void Calibrate()
        {
            m_isCalibrating = !m_isCalibrating;

            var passthrough = RigManager.Instance.RigOrchestrator.Camera.GetComponent<Passthrough>();
            passthrough.ActivePassthrough(m_isCalibrating, !m_isCalibrating);

            SetInteractables(!m_isCalibrating);

            m_firstMarker.gameObject.SetActive(m_isCalibrating);
            m_secondMarker.gameObject.SetActive(m_isCalibrating);
            m_planeCollider.enabled = m_isCalibrating;

            if (!m_isCalibrating)
                SaveCalibration();
        }

        void SaveCalibration()
        {
            Vector3[] coordinates = { m_firstMarker.localPosition, m_secondMarker.localPosition };

            MarkerPrefs.SavePrefs(coordinates[0], coordinates[1]);

            NetworkController.Instance.GroupedTeleportationManager.SetMarkers(coordinates);
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

