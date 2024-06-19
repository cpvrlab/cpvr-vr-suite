using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using VR;

namespace Util
{
    public class Passthrough : MonoBehaviour
    {
        public event Action<bool> PassthroughValueChanged;

        [SerializeField] List<string> m_passthroughScenes = new();
        // Passthrough default value for current scene
        bool m_passthroughInScene;
        Camera m_camera;
        ARCameraManager m_arCameraManager;

        void Awake()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            var cameraGO = RigManager.Instance.RigOrchestrator.Camera;
            m_camera = cameraGO.GetComponent<Camera>();
            m_arCameraManager = cameraGO.GetComponent<ARCameraManager>();
        }

        void OnActiveSceneChanged(Scene _, Scene next)
        {
            m_passthroughInScene = m_passthroughScenes.Contains(next.name);
            ActivePassthrough(m_passthroughInScene);
        }

        /// <summary>
        /// Enable/Disable the passthrough and hide/show the scene.
        /// </summary>
        /// <param name="value">If the passthrough is activated.</param>
        /// <param name="showScene">If the scene has to be displayed or hidden.</param>
        public void ActivePassthrough(bool value, bool showScene = true)
        {
            // if we try to set a value that is already set
            if (m_arCameraManager.enabled == value) return;

            if (value)
                SceneVisibility(showScene && m_passthroughInScene);
            else
                SceneVisibility(true);

            // If we are trying to deactivate passthrough in a passthrough scene
            if (!value && m_passthroughInScene) return;

            // Change the camera settings to make the passthrough work
            if (value)
            {
                m_camera.clearFlags = CameraClearFlags.SolidColor;
                m_camera.backgroundColor = new Color(0, 0, 0, 0);
            }
            else
            {
                m_camera.clearFlags = CameraClearFlags.Skybox;
                m_camera.backgroundColor = new Color(0, .2f, .5f, 0);
            }

            m_arCameraManager.enabled = value;

            PassthroughValueChanged?.Invoke(value);
        }

        /// <summary>
        /// Hide/Show the scene.
        /// </summary>
        /// <param name="visible">If the scene must be visible.</param>
        void SceneVisibility(bool visible)
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.TryGetComponent<VRPlayerBehaviour>(out _)) continue;
                foreach (var childRenderer in root.GetComponentsInChildren<Renderer>())
                    childRenderer.enabled = visible;
            }
        }
    }
}
