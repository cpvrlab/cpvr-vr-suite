using System.Collections;
using cpvrlab_vr_suite.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace cpvrlab_vr_suite.Scripts.VR
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [Header("Left Hand Actions")] 
        [SerializeField] private InputActionProperty menuAction;
        [SerializeField] private InputActionProperty leftTeleportAction;

        [Header("Right Hand Actions")]
        [SerializeField] private InputActionProperty rightTeleportAction;

        [Header("User Interface")]
        [SerializeField] private GameObject menu;

        private Transform _headTransform;
        private Transform _leftHandTransform;
        private Transform _rightHandTransform;
        private InputDevice _inputDeviceScript;
        private Teleporting _teleportScript;
        private MenuController _menuController;
        private float _menuLastOpened;

        private void Awake()
        {
            _headTransform = transform.GetChild(0).GetChild(0);
            if (!_headTransform.CompareTag("MainCamera"))
                Debug.LogError("Camera is not positioned setup or placed correctly!");

            _inputDeviceScript = GetComponent<InputDevice>();
            _teleportScript = GetComponent<Teleporting>();
            _menuController = menu.GetComponent<MenuController>();
        }

        private void Start()
        {
            menuAction.action.performed += ToggleMenu;
            leftTeleportAction.action.performed += StartLeftTeleport;
            leftTeleportAction.action.canceled += EndLeftTeleport;
            rightTeleportAction.action.performed += StartRightTeleport;
            rightTeleportAction.action.canceled += EndRightTeleport;
            SceneManager.activeSceneChanged += ChangedActiveScene;
            StartCoroutine(MenuInitialization());
        }

        private void Update()
        {
            if (_inputDeviceScript.controllerInput || _leftHandTransform == null) return;
            var dot = Vector3.Dot(_headTransform.forward,_leftHandTransform.up);
            Debug.Log($"Lefthand Dot: {dot}");
        }

        public void RegisterHand(Transform handTransform, bool isRightHand)
        {
            if (isRightHand)
                _rightHandTransform = handTransform;
            else
                _leftHandTransform = handTransform;
        }

        private void StartRightTeleport(InputAction.CallbackContext obj)
        {
            if (menu.activeSelf) return;
            if (!_inputDeviceScript.controllerInput && PalmFacesHead(true)) return;
            _teleportScript.Teleport(true);
        }

        private void StartLeftTeleport(InputAction.CallbackContext obj)
        {
            if (menu.activeSelf) return;
            if (!_inputDeviceScript.controllerInput && PalmFacesHead(false)) return;
            _teleportScript.Teleport(false);
        }

        private bool PalmFacesHead(bool rightHand)
        {
            var dot = Vector3.Dot(_headTransform.forward, rightHand ? _rightHandTransform.up : _leftHandTransform.up);
            return dot > 0.5f;
        }

        private void EndRightTeleport(InputAction.CallbackContext obj)
        {
            if (!_inputDeviceScript.controllerInput && PalmFacesHead(true))
                _teleportScript.CancelTeleport();
            else
                _teleportScript.DisableTeleport();
        }

        private void EndLeftTeleport(InputAction.CallbackContext obj)
        {
            if (!_inputDeviceScript.controllerInput && PalmFacesHead(false))
                _teleportScript.CancelTeleport();
            else
                _teleportScript.DisableTeleport();
        }

        private void ToggleMenu(InputAction.CallbackContext obj)
        {
            if (Time.time - _menuLastOpened < 0.25f) return;
            if (!_inputDeviceScript.controllerInput && !PalmFacesHead(false)) return;
            _menuLastOpened = Time.time;
            if (menu.activeSelf)
            {
                _menuController.CloseMenu();
            }
            else
            {
                SetMenuPositionAndRotation();
                menu.SetActive(true);
            }
        }
    
        private void SetMenuPositionAndRotation()
        {
            var currentHeadPosition = _headTransform.position;
            var origin = new Vector3(currentHeadPosition.x, transform.position.y, currentHeadPosition.z);
            var position = origin + 1.5f * Vector3.ProjectOnPlane(_headTransform.forward, Vector3.up).normalized +
                           new Vector3(0, 1f, 0);
            var rotation = Quaternion.Euler(0, _headTransform.rotation.eulerAngles.y, 0);
            menu.transform.SetPositionAndRotation(position, rotation);
        }

        private IEnumerator MenuInitialization()
        {
            menu.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            ToggleMenu(default);
        }

        private void ChangedActiveScene(Scene arg0, Scene arg1) => transform.position = Vector3.zero;

        private void OnDisable()
        {
            menuAction.action.performed -= ToggleMenu;
            leftTeleportAction.action.performed -= StartLeftTeleport;
            leftTeleportAction.action.canceled -= EndLeftTeleport;
            rightTeleportAction.action.performed -= StartRightTeleport;
            rightTeleportAction.action.canceled -= EndRightTeleport;
            SceneManager.activeSceneChanged -= ChangedActiveScene;
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
