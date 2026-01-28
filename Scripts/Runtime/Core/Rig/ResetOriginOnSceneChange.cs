using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    public class ResetOriginOnSceneChange : MonoBehaviour
    {
        public Vector3 ResetPosition { get; set; }

        private void OnEnable()
        {
            ResetPosition = Vector3.zero;
            SceneManager.activeSceneChanged += (_, _) =>
            {
                if (RigManager.Instance != null && RigManager.Instance.TryGet<XROrigin>(out var origin))
                {
                    origin.transform.position = ResetPosition;
                    origin.Camera.transform.position = Vector3.zero;
                }
            };
        }
    }
}
