using System.Linq;
using cpvr_vr_suite.Scripts.Runtime.Core;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace cpvr_vr_suite.Scripts.Runtime.Misc
{
    public class DisableOtherCameras : MonoBehaviour
    {
        Camera m_camera;

        void Awake() => SceneManager.activeSceneChanged += (_, activeScene) => DisableCameras(activeScene);

        void DisableCameras(Scene activeScene)
        {
            // Resolve lazily: the first activeSceneChanged fires before Start.
            if (m_camera == null && RigManager.Instance != null && RigManager.Instance.TryGet<XROrigin>(out var origin))
                m_camera = origin.Camera;

            if (m_camera == null || !m_camera.CompareTag("MainCamera")) return;

            var rigCameraObject = m_camera.gameObject;
            var allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            var allOtherCameras = allGameObjects.Where(
                go => go.scene == activeScene &&
                go.TryGetComponent<Camera>(out var _) &&
                go != rigCameraObject);

            foreach (var item in allOtherCameras)
                item.SetActive(false);

            //Debug.Log($"{allOtherCameras.Count()} Cameras disabled.");
        }
    }
}
