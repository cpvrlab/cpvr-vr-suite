using UnityEngine;
using UnityEngine.InputSystem;

namespace cpvrlab_vr_suite.Scripts.VR
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [Header("Left Hand Actions")]
        [SerializeField] private InputActionProperty leftTeleportAction;
        [SerializeField] private InputActionProperty leftHandTrackingState;

        [Header("Right Hand Actions")]
        [SerializeField] private InputActionProperty rightTeleportAction;
        [SerializeField] private InputActionProperty rightHandTrackingState;

        private Transform _headTransform;
        private Transform _leftHandTransform;
        private Transform _rightHandTransform;
        private InputDevice _inputDeviceScript;
        private Teleporting _teleportScript;

        private void Awake()
        {
            _headTransform = transform.GetChild(0).GetChild(0);
            if (!_headTransform.CompareTag("MainCamera"))
                Debug.LogError("Camera is not positioned setup or placed correctly!");

            _inputDeviceScript = GetComponent<InputDevice>();
            _teleportScript = GetComponent<Teleporting>();
        }

        private void Start()
        {
            leftTeleportAction.action.performed += StartLeftTeleport;
            leftTeleportAction.action.canceled += EndLeftTeleport;
            leftHandTrackingState.action.canceled += CancelTeleport;
            rightTeleportAction.action.performed += StartRightTeleport;
            rightTeleportAction.action.canceled += EndRightTeleport;
            rightHandTrackingState.action.canceled += CancelTeleport;
        }

        public void RegisterHand(Transform handTransform, bool isRightHand)
        {
            if (isRightHand)
                _rightHandTransform = handTransform;
            else
                _leftHandTransform = handTransform;
        }

        private void StartRightTeleport(InputAction.CallbackContext _)
        {
            if (!_inputDeviceScript.controllerInput && PalmFacesHead(true)) return;
            _teleportScript.Teleport(true);
        }

        private void StartLeftTeleport(InputAction.CallbackContext _)
        {
            if (!_inputDeviceScript.controllerInput && PalmFacesHead(false)) return;
            _teleportScript.Teleport(false);
        }

        private bool PalmFacesHead(bool rightHand)
        {
            var dot = Vector3.Dot(_headTransform.forward, rightHand ? _rightHandTransform.up : _leftHandTransform.up);
            return dot > 0.5f;
        }

        private void EndRightTeleport(InputAction.CallbackContext _)
        {
            if (!_inputDeviceScript.controllerInput && 
                PalmFacesHead(true))
                _teleportScript.CancelTeleport();
            else
                _teleportScript.DisableTeleport();
        }

        private void EndLeftTeleport(InputAction.CallbackContext _)
        {
            if (!_inputDeviceScript.controllerInput && 
                PalmFacesHead(false))
                _teleportScript.CancelTeleport();
            else
                _teleportScript.DisableTeleport();
        }

        private void CancelTeleport(InputAction.CallbackContext obj) => _teleportScript.CancelTeleport();

        private void OnDisable()
        {
            leftTeleportAction.action.performed -= StartLeftTeleport;
            leftTeleportAction.action.canceled -= EndLeftTeleport;
            rightTeleportAction.action.performed -= StartRightTeleport;
            rightTeleportAction.action.canceled -= EndRightTeleport;
        }

        private void OnDrawGizmos()
        {
            if (_leftHandTransform == null || _rightHandTransform == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_leftHandTransform.position,_leftHandTransform.up);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_headTransform.position, _headTransform.forward);
        }
    }
}
