using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Util;

namespace Network
{
    public class PlaneInteractable : XRBaseInteractable
    {
        [SerializeField] GameObject m_markerPrefab;
        [SerializeField] GameObject m_reticlePrefab;

        GameObject m_reticle;
        int m_markerIndex;
        readonly GameObject[] m_markers = new GameObject[2];
        bool m_calibrating;
        bool m_markerVisible;

        void Start()
        {
            if (m_reticle == null)
            {
                m_reticle = Instantiate(m_reticlePrefab, transform);
                m_reticle.SetActive(false);
            }

            m_reticle.GetComponent<MarkerBehaviour>().IsReticle = true;
        }

        void Update()
        {
            UpdateReticle();
        }

        void UpdateReticle()
        {
            if (!m_calibrating) return;

            if (m_reticle == null)
                m_reticle = Instantiate(m_reticlePrefab, transform);

            var interactor = interactorsHovering.FirstOrDefault(it => it is XRRayInteractor) as XRRayInteractor;

            if (interactor == null || !interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                m_reticle.SetActive(false);
                return;
            }

            m_reticle.transform.position = hit.point;

            m_reticle.SetActive(true);
        }

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            if (args.interactorObject is not XRRayInteractor interactor) return;
            if (!interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit)) return;

            AddMarker(hit.point);
        }

        public bool LoadPrefs()
        {
            if (!MarkerPrefs.LoadPrefs(out Vector3 first, out Vector3 second)) return false;

            CreateMarker(first, 0);
            CreateMarker(second, 1);

            UpdateMarkersVisibility();
            return true;
        }

        void CreateMarker(Vector3 position, int index)
        {
            var marker = Instantiate(m_markerPrefab, RigManager.Instance.RigOrchestrator.Origin);
            marker.transform.position = position;

            var markerBehaviour = marker.GetComponent<MarkerBehaviour>();
            markerBehaviour.SetNumber(index + 1);
            markerBehaviour.IsReticle = false;
            m_markers[index] = marker;
        }

        void AddMarker(Vector3 position)
        {
            if (m_markers[m_markerIndex] == null)
                CreateMarker(position, m_markerIndex);
            else
                m_markers[m_markerIndex].transform.position = position;

            m_markerIndex = (m_markerIndex + 1) % 2;

            m_reticle.GetComponent<MarkerBehaviour>().SetNumber(m_markerIndex + 1);

            UpdateMarkersVisibility();
        }

        /// <summary>
        /// Enable/Disable the collider of the object.
        /// </summary>
        /// <param name="status">Collider's status.</param>
        public void Active(bool status)
        {
            m_calibrating = status;

            GetComponent<Collider>().enabled = status;

            UpdateMarkersVisibility();
        }

        /// <summary>
        /// Hide/Show the markers.
        /// </summary>
        /// <param name="visible">If the markers are visible or not.</param>
        public void SetMarkersVisibility(bool visible)
        {
            m_markerVisible = visible;

            UpdateMarkersVisibility();
        }

        /// <summary>
        /// Update the markers visibility.
        /// </summary>
        void UpdateMarkersVisibility()
        {
            // If we are calibrating or if the player wants the markers to be shown.
            bool visible = m_calibrating || m_markerVisible;

            foreach (var marker in m_markers)
            {
                if (marker == null) continue;
                marker.SetActive(visible);
            }
        }

        /// <summary>
        /// Return the coordinates of the markers
        /// </summary>
        /// <returns>An Vector3[2] array containing the markers position or an empty array if the two markers are not yet defined.</returns>
        public Vector3[] GetMarkerPositions()
        {
            if (m_markers.Any(it => it == null)) return null;

            return new[] {
                m_markers[0].transform.localPosition,
                m_markers[1].transform.localPosition
            };
        }
    }
}