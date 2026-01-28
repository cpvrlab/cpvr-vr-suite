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

        void Start() => m_camera = RigManager.Instance ? RigManager.Instance.Get<XROrigin>().Camera : null;

        void DisableCameras(Scene activeScene)
        {
            if (!m_camera.CompareTag("MainCamera")) return;

            var allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            var allOtherCameras = allGameObjects.Where(
                go => go.scene == activeScene &&
                go.TryGetComponent<Camera>(out var _) &&
                go != m_camera);

            foreach (var item in allOtherCameras)
                item.SetActive(false);

            //Debug.Log($"{allOtherCameras.Count()} Cameras disabled.");
        }
    }
}
