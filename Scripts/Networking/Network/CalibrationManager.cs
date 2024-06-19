using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Network
{
    /// <summary>
    /// Manage the calibration.
    /// </summary>
    public class CalibrationManager : MonoBehaviour
    {
        public Button calibrationButton;

        public Toggle passthroughToggle;

        [SerializeField] PlaneInteractable planeInteractable;

        bool _isCalibrating;
        Passthrough m_passthrough;

        void Awake()
        {
            DontDestroyOnLoad(this);

            m_passthrough = RigManager.Instance.RigOrchestrator.Camera.GetComponent<Passthrough>();
        }

        // Loads inputSystem of localPlayer
        void Start()
        {
            if (!planeInteractable.LoadPrefs()) return;

            DoCalibration();

            Debug.Log("MarkerPrefs Loaded.");
        }

        void OnEnable()
        {
            m_passthrough.PassthroughValueChanged += OnPassthroughValueChanged;
        }

        void OnDisable()
        {
            m_passthrough.PassthroughValueChanged -= OnPassthroughValueChanged;
        }

        public void OnCalibrationButtonClicked()
        {
            _isCalibrating = !_isCalibrating;

            if (!_isCalibrating)
            {
                // Finish calibration
                DoCalibration();
                calibrationButton.GetComponentInChildren<TMP_Text>().text = "Start calibration.";
            }
            else
            {
                // Start Calibration
                calibrationButton.GetComponentInChildren<TMP_Text>().text = "Finish calibration.";
            }

            // Block/Unblock the interaction mode to/from ray
            if (RigManager.Instance.RigOrchestrator.TryGetInteractorManager(out HandManager handManager))
            {
                handManager.InteractionModeLocked = _isCalibrating;
            }

            // Enable/Disable Passthrough, plane to interact with and player visuals.
            m_passthrough.ActivePassthrough(_isCalibrating, false);
            planeInteractable.Active(_isCalibrating);

            SetPlayersVisibility(!_isCalibrating);
        }

        /// <summary>
        /// Used by the toggle to hide/display the markers.
        /// </summary>
        /// <param name="value">If markers are visible or not.</param>
        public void SetMarkersVisibility(bool value)
        {
            planeInteractable.SetMarkersVisibility(value);
        }

        /// <summary>
        /// Used by the toggle to hide/display the player's avatars.
        /// </summary>
        /// <param name="value">If avatars are visible of not.</param>
        public void SetPlayersVisibility(bool value)
        {
            // Enable/disable the hands renderer from XRRig
            RigManager.Instance.RigOrchestrator.Visualizer.drawMeshes = !value;

            GameObject[] avatars = GameObject.FindGameObjectsWithTag("Avatar");

            foreach (GameObject avatar in avatars)
            {
                foreach (var childRenderer in avatar.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = value;
                }
            }

            Debug.Log(avatars.Length + " " + (value ? "avatars displayed" : "avatars hidden"));
        }

        /// <summary>
        /// Used by the toggle to enable/disable passthrough.
        /// </summary>
        /// <param name="value">If passthrough is activated or not.</param>
        public void SetPassthrough(bool value)
        {
            m_passthrough.ActivePassthrough(value, false);
        }

        /// <summary>
        /// Used to update the UI toggle value.
        /// </summary>
        /// <param name="newValue">New passthrough value.</param>
        void OnPassthroughValueChanged(bool newValue)
        {
            passthroughToggle.isOn = newValue;
        }

        /// <summary>
        /// Perform the calibration.
        /// </summary>
        void DoCalibration()
        {
            Vector3[] coordinates = planeInteractable.GetMarkerPositions();

            MarkerPrefs.SavePrefs(coordinates[0], coordinates[1]);

            GroupedTeleportationManager.Instance.SetMarkers(coordinates);
            GroupedTeleportationManager.Instance.RecenterXROrigin();
        }
    }
}
