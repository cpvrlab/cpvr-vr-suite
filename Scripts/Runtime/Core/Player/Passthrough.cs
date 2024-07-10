using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using VR;

namespace Util
{
    [RequireComponent(typeof(Camera), typeof(ARCameraManager))]
    public class Passthrough : MonoBehaviour
    {
        public event Action<bool> PassthroughValueChanged;

        // Passthrough default value for current scene
        public bool IsEnabled { get; private set; }

        [SerializeField] List<string> m_passthroughScenes = new();

        Camera m_camera;
        ARCameraManager m_arCameraManager;

        void Awake()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            m_camera = GetComponent<Camera>();
            m_arCameraManager = GetComponent<ARCameraManager>();
            IsEnabled = m_arCameraManager.enabled;
        }

        void OnActiveSceneChanged(Scene _, Scene next) => ActivePassthrough(m_passthroughScenes.Contains(next.name));

        /// <summary>
        /// Enable/Disable the passthrough and hide/show the scene.
        /// </summary>
        /// <param name="value">If the passthrough is activated.</param>
        /// <param name="showScene">If the scene has to be displayed or hidden.</param>
        public void ActivePassthrough(bool value, bool showScene = true)
        {
            // if we try to set a value that is already set
            if (IsEnabled == value) return;

            if (value)
                SceneVisibility(showScene && value);
            else
                SceneVisibility(true);

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
            IsEnabled = value;

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
