using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using VR;

namespace Util
{
    /// <summary>
    /// Used to manage passthrough.
    /// </summary>
    public class Passthrough : Singleton<Passthrough> {
        // Scenes were passthrough is enabled by default
        public List<String> passthroughScenes = new();

        // Passthrough default value for current scene
        private bool _passthroughInScene;
        
        // Used to notify the menu panel for passthrough toggle value
        public event Action<bool> PassthroughValueChanged;
        
        // mainCamera
        private Camera _camera;
        // camera script that enable passthrough
        private ARCameraManager _arCameraManager;

        protected override void Awake() {
            base.Awake();
            
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void Start() {
            // Get the camera
            _camera = GetComponent<XROrigin>().Camera;
            // Get the script
            _arCameraManager = _camera.GetComponent<ARCameraManager>();
        }
        
        /// <summary>
        /// Event when the scene has changed.
        /// Used to enable or disable passthrough.
        /// </summary>
        /// <param name="_">Not used.</param>
        /// <param name="next">Newly loaded scene.</param>
        private void OnActiveSceneChanged(Scene _, Scene next) {
            _passthroughInScene = passthroughScenes.Contains(next.name);
            ActivePassthrough(_passthroughInScene);
        }
    
        /// <summary>
        /// Enable/Disable the passthrough and hide/show the scene.
        /// </summary>
        /// <param name="value">If the passthrough is activated.</param>
        /// <param name="showScene">If the scene has to be displayed or hidden.</param>
        public void ActivePassthrough(bool value, bool showScene = true) {
            // if we try to set a value that is already set
            if (_arCameraManager.enabled == value) return;
            
            if (value) {
                SceneVisibility(showScene && _passthroughInScene);
            } else {
                SceneVisibility(true);
            }
            
            // If we are trying to deactivate passthrough in a passthrough scene
            if (!value && _passthroughInScene) return;
            
            // Change the camera settings to make the passthrough work
            if (value) {
                _camera.clearFlags = CameraClearFlags.SolidColor;
                _camera.backgroundColor = new Color(0, 0, 0, 0);
            } else {
                _camera.clearFlags = CameraClearFlags.Skybox;
                _camera.backgroundColor = new Color(0, .2f, .5f, 0);
            }
            
            // Enable or Disable the passthrough
            _arCameraManager.enabled = value;

            // Trigger the event
            PassthroughValueChanged?.Invoke(value);
        } 
        
        /// <summary>
        /// Hide/Show the scene.
        /// </summary>
        /// <param name="visible">If the scene must be visible.</param>
        private void SceneVisibility(bool visible) {
            GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots) {
                if(root.TryGetComponent<VRPlayerBehaviour>(out _)) continue;
                foreach (var childRenderer in root.GetComponentsInChildren<Renderer>())
                    childRenderer.enabled = visible;
            }
        }
    }
}
