using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Network {
    /// <summary>
    /// Manage the calibration.
    /// </summary>
    public class CalibrationManager : MonoBehaviour {
        public Button calibrationButton;
        
        public Toggle passthroughToggle;
        
        [SerializeField] private PlaneInteractable planeInteractable;

        // If we are calibrating or not.
        private bool _calibrating;

        private void Awake() {
            DontDestroyOnLoad(this);
        }

        // Loads inputSystem of localPlayer
        private void Start() {
            Passthrough.Instance.PassthroughValueChanged += OnPassthroughValueChanged;
            
            if (!planeInteractable.LoadPrefs()) return;

            DoCalibration();
            
            Debug.Log("MarkerPrefs Loaded.");
        }
        
        public void OnCalibrationButtonClicked() {
            _calibrating = !_calibrating;
            
            if (!_calibrating) {
                // Finish calibration
                DoCalibration();
                calibrationButton.GetComponentInChildren<TMP_Text>().text = "Start calibration.";
            } else {
                // Start Calibration
                calibrationButton.GetComponentInChildren<TMP_Text>().text = "Finish calibration.";
            }

            // Block/Unblock the interaction mode to/from ray
            if (RigManager.Instance.RigOrchestrator.TryGetInteractorManager(out HandManager handManager)) {
                handManager.InteractionModeLocked = _calibrating;
            }
            
            // Enable/Disable Passthrough, plane to interact with and player visuals.
            Passthrough.Instance.ActivePassthrough(_calibrating, false);
            planeInteractable.Active(_calibrating);

            SetPlayersVisibility(!_calibrating);
        }
        
        /// <summary>
        /// Used by the toggle to hide/display the markers.
        /// </summary>
        /// <param name="value">If markers are visible or not.</param>
        public void SetMarkersVisibility(bool value) {
            planeInteractable.SetMarkersVisibility(value);
        }

        /// <summary>
        /// Used by the toggle to hide/display the player's avatars.
        /// </summary>
        /// <param name="value">If avatars are visible of not.</param>
        public void SetPlayersVisibility(bool value) {
            // Enable/disable the hands renderer from XRRig
            RigManager.Instance.RigOrchestrator.Visualizer.drawMeshes = !value;
            
            GameObject[] avatars = GameObject.FindGameObjectsWithTag("Avatar");
            
            foreach(GameObject avatar in avatars) {
                foreach( var childRenderer in avatar.GetComponentsInChildren<Renderer>()) {
                    childRenderer.enabled = value;
                }
            }

            Debug.Log(avatars.Length + " " + (value ? "avatars displayed" : "avatars hidden"));
        }

        /// <summary>
        /// Used by the toggle to enable/disable passthrough.
        /// </summary>
        /// <param name="value">If passthrough is activated or not.</param>
        public void SetPassthrough(bool value) {
            Passthrough.Instance.ActivePassthrough(value, false);
        }
        
        /// <summary>
        /// Used to update the UI toggle value.
        /// </summary>
        /// <param name="newValue">New passthrough value.</param>
        private void OnPassthroughValueChanged(bool newValue) {
            passthroughToggle.isOn = newValue;
        }
        
        /// <summary>
        /// Perform the calibration.
        /// </summary>
        private void DoCalibration() {            
            Vector3[] coordinates = planeInteractable.GetMarkerPositions();
            
            MarkerPrefs.SavePrefs(coordinates[0], coordinates[1]);
            
            GroupedTeleportationManager.Instance.SetMarkers(coordinates);
            GroupedTeleportationManager.Instance.RecenterXROrigin();
        }
    }
}
