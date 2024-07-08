using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Util;

namespace Network
{
    /// <summary>
    /// A plane used to place markers on it using RayInteractor.
    /// </summary>
    public class PlaneInteractable : XRBaseInteractable
    {
        // Prefab of the markers
        [SerializeField] GameObject markerPrefab;

        // GameObject used as reticle.
        [SerializeField] GameObject reticlePrefab;
        GameObject reticle;

        int _markerIndex;

        // The marker list
        readonly GameObject[] _markers = new GameObject[2];

        bool _calibrating;

        // Marker's visibility
        bool _markerVisible;

        void Start()
        {
            if (reticle == null)
            {
                reticle = Instantiate(reticlePrefab, transform);
                reticle.SetActive(false);
            }

            reticle.GetComponent<MarkerBehaviour>().IsReticle = true;
        }

        void Update()
        {
            UpdateReticle();
        }

        /// <summary>
        /// Hide/Show and place the reticle if a RayInteractor is hovering with the plane.
        /// </summary>
        void UpdateReticle()
        {
            if (!_calibrating) return;

            if (reticle == null)
                reticle = Instantiate(reticlePrefab, transform);

            XRRayInteractor interactor = interactorsHovering.FirstOrDefault(it => it is XRRayInteractor) as XRRayInteractor;

            if (interactor == null || !interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                reticle.SetActive(false);
                return;
            }

            reticle.transform.position = hit.point;

            reticle.SetActive(true);
        }

        /// <summary>
        /// When an interactor ray is interacting the plane and a marker should be added.
        /// </summary>
        /// <param name="args">Contains the ray that interacted.</param>
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            // If it is not a RayInteractor we quit
            if (args.interactorObject is not XRRayInteractor interactor) return;
            // If we can't get the position of the raycasthit we quit
            if (!interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit)) return;

            AddMarker(hit.point);
        }

        /// <summary>
        /// Load the markers for the local storage.
        /// </summary>
        /// <returns>If markers have been loaded.</returns>
        public bool LoadPrefs()
        {
            if (!MarkerPrefs.LoadPrefs(out Vector3 first, out Vector3 second)) return false;

            CreateMarker(first, 0);
            CreateMarker(second, 1);

            UpdateMarkersVisibility();
            return true;
        }

        /// <summary>
        /// Create a marker at a certain position and index.
        /// </summary>
        /// <param name="position">Position of the marker.</param>
        /// <param name="index">Index of the marker (0 or 1) inside the array.</param>
        void CreateMarker(Vector3 position, int index)
        {
            // Create a marker and place it
            GameObject marker = Instantiate(markerPrefab, RigManager.Instance.RigOrchestrator.Origin);
            marker.transform.position = position;

            // Set the text on the marker
            MarkerBehaviour markerBehaviour = marker.GetComponent<MarkerBehaviour>();
            markerBehaviour.SetNumber(index + 1);

            // Not a reticle
            markerBehaviour.IsReticle = false;

            // Save it
            _markers[index] = marker;
        }

        /// <summary>
        /// Add a marker at the current index.
        /// </summary>
        /// <param name="position">Position of the marker.</param>
        void AddMarker(Vector3 position)
        {
            // Spawning a marker at the hit position
            if (_markers[_markerIndex] == null)
            {
                CreateMarker(position, _markerIndex);
            }
            else
            {
                _markers[_markerIndex].transform.position = position;
            }

            _markerIndex = (_markerIndex + 1) % 2;

            reticle.GetComponent<MarkerBehaviour>().SetNumber(_markerIndex + 1);

            UpdateMarkersVisibility();
        }

        /// <summary>
        /// Enable/Disable the collider of the object.
        /// </summary>
        /// <param name="status">Collider's status.</param>
        public void Active(bool status)
        {
            _calibrating = status;

            GetComponent<Collider>().enabled = status;

            UpdateMarkersVisibility();
        }

        /// <summary>
        /// Hide/Show the markers.
        /// </summary>
        /// <param name="visible">If the markers are visible or not.</param>
        public void SetMarkersVisibility(bool visible)
        {
            _markerVisible = visible;

            UpdateMarkersVisibility();
        }

        /// <summary>
        /// Update the markers visibility.
        /// </summary>
        void UpdateMarkersVisibility()
        {
            // If we are calibrating or if the player wants the markers to be shown.
            bool visible = _calibrating || _markerVisible;

            foreach (var marker in _markers)
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
            if (_markers.Any(it => it == null)) return null;

            return new[] {
                _markers[0].transform.localPosition,
                _markers[1].transform.localPosition
            };
        }
    }
}